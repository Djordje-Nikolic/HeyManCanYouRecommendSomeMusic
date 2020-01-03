using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HeyManCanYouRecommendSomeMusic.Services
{
    public class WebCrawlerService
    {
        private HttpClient httpClient = new HttpClient();

        public async Task<string[]> GetSongName(string link)
        {
            HttpResponseMessage response = await httpClient.GetAsync(link);

            if (!response.IsSuccessStatusCode)
                throw new ArgumentException("The requested link cannot be fetched");

            string html = await response.Content.ReadAsStringAsync();
            return ProccessHtml(html);
        }

        private string[] ProccessHtml(string html)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);

            string titleXPath = "(//title)";
            string songTitle = document.DocumentNode.SelectSingleNode(titleXPath).InnerText;

            if (songTitle == null)
                throw new ArgumentException("Cannot find song title for selected link");

            return ProccessString(songTitle);
        }

        private string[] ProccessString(string songTitle)
        {
            string[] arr = new string[2];

            int dashIndex = songTitle.IndexOf('-');
            arr[0] = songTitle.Substring(0, dashIndex).Trim();

            string afterDash = songTitle.Substring(dashIndex);
            int delimiterIndex = afterDash.IndexOf('(');
            if (delimiterIndex == -1) delimiterIndex = afterDash.IndexOf('|');
            arr[1] = afterDash.Substring(1, delimiterIndex - 1).Trim();

            return arr;
        }
    }
}
