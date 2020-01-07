using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeyManCanYouRecommendSomeMusic.Models;
using HeyManCanYouRecommendSomeMusic.Models.Deezer;
using HeyManCanYouRecommendSomeMusic.Services;

namespace HeyManCanYouRecommendSomeMusic.Helpers
{
    public class DBPopulator
    {
        private IDBService dbService;
        private IDeezer deezerService;

        public DBPopulator(IDBService dBService, IDeezer deezerService)
        {
            this.dbService = dBService;
            this.deezerService = deezerService;
        }

        public async Task PopulateForAllGenres(int countPerGenre)
        {
            var result = await deezerService.FetchForEachGenre(countPerGenre);

            foreach (var track in result)
            {
                Song current = new Song();
                current.id = track.ID.ToString();
                current.name = track.Title;
                current.band = track.Artist.Name;
                current.genre = track.Album.Genres.Data.FirstOrDefault().Name;

                var relationships = SongAnalyzer.AnalyzeForRelationships(current); 
            }
        }
    }
}
