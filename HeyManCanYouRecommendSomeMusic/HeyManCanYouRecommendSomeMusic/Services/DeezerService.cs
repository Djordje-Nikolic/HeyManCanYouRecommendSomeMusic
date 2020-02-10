using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeyManCanYouRecommendSomeMusic.Helpers;
using HeyManCanYouRecommendSomeMusic.Models.Deezer;

namespace HeyManCanYouRecommendSomeMusic.Services
{
    public interface IDeezer
    {
        //IEnumerable<object> GetTopSongs(int count);
        Task<Track> GetTrackById(int id);
        Task<Album> GetAlbumById(int id);
        Task<Artist> GetArtistById(int id);
        Task<SearchResult<Artist>> GetRelatedArtists(int referenceArtistId);
        Task<SearchResult<Track>> GetTopTracks(int genreId, int count, int index = 0, bool expandTrack = false, bool expandAlbum = false, bool expandArtist = false);
        Task<SearchResult<Genre>> GetAllGenres();
        Task<ConcurrentSet<Track>> FetchForEachGenre(int countEachGenre);
        Task FetchForGenre(int genreId, int countForGenre, ISet<Track> tracks);
    }
    public class DeezerService : IDeezer, IDisposable
    {
        internal ExecutorService executorService;

        public DeezerService()
        {
            executorService = new ExecutorService();
        }

        public void Dispose()
        {
            executorService.Dispose();
        }

        public async Task<Album> GetAlbumById(int id)
        {
            var task = await executorService.ExecuteGet<Album>($"album/{id}");
            if (!task.IsValidObject())
                throw new ArgumentException("The received object is not a valid DeezerObject.");
            return task;
        }

        public async Task<Artist> GetArtistById(int id)
        {
            var task = await executorService.ExecuteGet<Artist>($"artist/{id}");
            if (!task.IsValidObject())
                throw new ArgumentException("The received object is not a valid DeezerObject.");
            return task;
        }

        public async Task<Track> GetTrackById(int id)
        {
            var task = await executorService.ExecuteGet<Track>($"track/{id}");
            if (!task.IsValidObject())
                throw new ArgumentException("The received object is not a valid DeezerObject.");
            return task;
        }

        public async Task<SearchResult<Artist>> GetRelatedArtists(int id)
        {
            return await executorService.ExecuteGet<SearchResult<Artist>>($"artist/{id}/related");
        }

        public async Task<SearchResult<Track>> GetTopTracks(int genreId, int count, int index = 0, bool expandTrack = false, bool expandAlbum = false, bool expandArtist = false)
        {
            SearchResult<Track> result = await executorService.ExecuteGet<SearchResult<Track>>($"chart/{genreId}/tracks?index={index}&limit={count}");

            if (result != null && result.Data != null)
            {
                foreach (Track track in result.Data)
                {
                    int trackId = track.ID;
                    int albumId = track.Album.ID;
                    int artistId = track.Artist.ID;

                    if (expandTrack)
                    {
                        Track trackExtended = GetTrackById(trackId).Result;
                        track.Expand(trackExtended);
                    }

                    if (expandAlbum)
                    {
                        Album album = GetAlbumById(albumId).Result;
                        track.Album.Expand(album);
                    }

                    if (expandArtist)
                    {
                        Artist artist = await GetArtistById(artistId);
                        track.Artist.Expand(artist);
                    }
                }
            }

            return result;
        }

        public async Task<SearchResult<Genre>> GetAllGenres()
        {
            return await executorService.ExecuteGet<SearchResult<Genre>>("genre").ConfigureAwait(false);
        }

        public async Task<ConcurrentSet<Track>> FetchForEachGenre(int countEachGenre)
        {
            SearchResult<Genre> allGenres = await GetAllGenres();
            ConcurrentSet<Track> foundTracks = new ConcurrentSet<Track>(Environment.ProcessorCount, allGenres.Data.Count * countEachGenre);

            IEnumerable<Task> tasks = allGenres.Data.Select(x => FetchForGenre(x.ID, countEachGenre, foundTracks));

            Task.WhenAll(tasks).Wait();

            return foundTracks;
        }

        public async Task FetchForGenre(int genreId, int countForGenre, ISet<Track> tracks)
        {
            SearchResult<Track> searchResult = await GetTopTracks(genreId, countForGenre, 0, expandTrack: true, expandAlbum: true);

            if (searchResult != null && searchResult.Data != null)
            {
                foreach (var track in searchResult.Data)
                {
                    tracks.Add(track);
                }
            }
        }
    }
}
