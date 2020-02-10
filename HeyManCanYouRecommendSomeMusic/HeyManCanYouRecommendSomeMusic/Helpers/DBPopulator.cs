using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeyManCanYouRecommendSomeMusic.Models;
using HeyManCanYouRecommendSomeMusic.Models.Deezer;
using HeyManCanYouRecommendSomeMusic.Services;
using HeyManCanYouRecommendSomeMusic.Models.Relationships;

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
                current.bpm = track.Bpm.ToString();
                current.duration = track.Duration.ToString();

                IEnumerable<Relationship> satisfiedRelationships = SongAnalyzer.AnalyzeForRelationships(current);
                List<Tuple<Relationship, Song>> relationshipPairsToAdd = new List<Tuple<Relationship, Song>>();

                foreach (var rel in satisfiedRelationships)
                {
                    Song foundSong = dbService.GetSongsInRelationship(rel, 1).FirstOrDefault();

                    if (foundSong == null)
                    {
                        Type type = rel.GetType();
                        var limits = rel.GetLimits();
                        int lowerLimit = limits.Item1;
                        int upperLimit = limits.Item2;

                        if (type == typeof(LengthRelationship))
                        {
                            foundSong = dbService.GetSongWithinDuration(lowerLimit, upperLimit).FirstOrDefault();
                        }
                        else if (type == typeof(TempoRelationship))
                        {
                            foundSong = dbService.GetSongWithinTempo(lowerLimit, upperLimit).FirstOrDefault();
                        }
                        else
                            throw new ArgumentException();
                    }

                    if (foundSong != null)
                    {
                        relationshipPairsToAdd.Add(new Tuple<Relationship, Song>(rel, foundSong));
                    }
                }

                dbService.AddNewSong(current);
                foreach (var pair in relationshipPairsToAdd)
                {
                    dbService.CreateRelationship(pair.Item2, current, pair.Item1);
                }
            }
        }
    }
}
