using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Threading;
using System.IO.Compression;
using System.Threading.Tasks;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using HeyManCanYouRecommendSomeMusic.Helpers;

namespace HeyManCanYouRecommendSomeMusic.Services
{
    internal class ExecutorService : IDisposable
    {
        public int RequestsCount { get { return requestCount; }}
        public static int RequestPerIntervalCount { get { return currentRequests;  }}

        private static object counterLocker = new object();
        private int requestCount = 0;
        private static int currentRequests = 0;
        private static int resetCount = 0;

        private const int DEFAULT_TIMEOUT = 30000; //30secs
        private const int INTERVAL = 7500;
        private const int MAX_REQUEST_PER_INTERVAL = 50;

        private static Lazy<Semaphore> requestIntervalSemaphore = new Lazy<Semaphore>(() =>
        {
            Semaphore temp = new Semaphore(MAX_REQUEST_PER_INTERVAL, MAX_REQUEST_PER_INTERVAL);
            timer = new Timer(ResetIntervalRequestCount, temp, 0, INTERVAL);
            return temp;
        });
        private static Timer timer;
        private readonly JsonSerializerOptions jsonOptions;
        private static readonly Lazy<HttpClient> client = new Lazy<HttpClient>(() => 
        {
            var handler = new HttpClientHandler();
            return new HttpClient(handler, disposeHandler: true);
        });
        private readonly CancellationTokenSource cancellationTokenSource;

        internal ExecutorService()
        {
            this.cancellationTokenSource = new CancellationTokenSource();
            this.jsonOptions = new JsonSerializerOptions();
            this.jsonOptions.IgnoreNullValues = true;
            this.jsonOptions.Converters.Add(new DateTimeConverterUsingDateTimeParseAsFallback());

            ConfigureHttpClient(client.Value);
        }

        internal CancellationToken CancellationToken { get { return cancellationTokenSource.Token; } }


        public Task<T> ExecuteGet<T>(string method)
        {
            requestIntervalSemaphore.Value.WaitOne();
            lock (counterLocker)
            {
                currentRequests++;
                requestCount++;
            }
            string url = BuildUrl(method);
            return client.Value.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, this.CancellationToken)
                         .ContinueWith(async t =>
                         {
                             if (t.IsFaulted)
                             {
                                 throw t.Exception.GetBaseException();
                             }

                             // Ensure we dispose of stuff should things go bad
                             using (t.Result)
                             {
                                 CheckHttpResponse(t.Result);

                                 return await GetJsonObjectFromResponse<T>(t.Result)
                                                .ConfigureAwait(false);
                             }

                         }, this.CancellationToken, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default)
                        .Unwrap();
        }

        private static void ResetIntervalRequestCount(object stateInfo)
        {
            Semaphore sem = (Semaphore)stateInfo;

            lock (counterLocker)
            {
                int temp = currentRequests;
                currentRequests = 0;

                if (temp > 0)
                {
                    resetCount++;
                    sem.Release(temp);
                }
            }
        }

        internal string BuildUrl(string url)
        {
            string trueUrl = url;
            var queryStrings = new List<string>();

            //Make sure we've filled all url segments...
            if (trueUrl.Contains("{") || trueUrl.Contains("}"))
            {
                throw new InvalidOperationException("Failed to fill out all url segment parameters. Perhaps they weren't all provided?");
            }

            // Ensure we request json output (not default)
            queryStrings.Add("output=json");

            return string.Format("{0}?{1}", trueUrl, string.Join("&", queryStrings));
        }


        private void ConfigureHttpClient(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri("https://api.deezer.com/");
            httpClient.Timeout = TimeSpan.FromMilliseconds(DEFAULT_TIMEOUT);

            // Allow us to deal with compressed content, should Deezer support it.
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        }

        private Stream GetDecompessionStreamForResponse(Stream responseStream, HttpContentHeaders contentHeaders)
        {
            // If header is not present the Deezer may not support this compression algorithm OR the
            // given HttpMessageHandler has support for automatic compression.
            if (contentHeaders != null && contentHeaders.ContentEncoding.Any())
            {
                foreach (var entry in contentHeaders.ContentEncoding)
                {
                    switch (entry.ToLowerInvariant())
                    {
                        case "gzip":
                            {
                                return new GZipStream(responseStream, CompressionMode.Decompress);
                            }
                        case "deflate":
                            {
                                return new DeflateStream(responseStream, CompressionMode.Decompress);
                            }
                    }
                }
            }

            return responseStream;
        }

        private async Task<T> DeserializeResponseStream<T>(HttpResponseMessage response)
        {
            using (var responseStream = await response.Content.ReadAsStreamAsync()
                                                              .ConfigureAwait(false))
            {
                using (var compressedStream = GetDecompessionStreamForResponse(responseStream, response.Content.Headers))
                {
                    return await System.Text.Json.JsonSerializer.DeserializeAsync<T>(compressedStream, jsonOptions);
                }
            }
        }

        private async Task<T> GetJsonObjectFromResponse<T>(HttpResponseMessage response)
        {
            using (response)
            {
                CheckHttpResponse(response);

                return await DeserializeResponseStream<T>(response)
                                .ConfigureAwait(false);
            }
        }

        //Checks a response for errors and exceptions
        private void CheckHttpResponse(HttpResponseMessage response)
        {
            if(!response.IsSuccessStatusCode)
            {
                string msg = $"Status: {response.StatusCode} :: {response.ReasonPhrase}";
                throw new HttpRequestException(msg);
            }

            if (response.Content == null)
            {
                throw new HttpRequestException("Request returned but there was no content attached.");
            }
        }



        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.cancellationTokenSource.Cancel();
                this.cancellationTokenSource.Dispose();

                timer.Dispose();

                client.Value.CancelPendingRequests();
                client.Value.Dispose();
            }
        }
    }
}
