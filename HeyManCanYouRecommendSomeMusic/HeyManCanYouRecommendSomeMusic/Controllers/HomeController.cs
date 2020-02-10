using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HeyManCanYouRecommendSomeMusic.Models;
using HeyManCanYouRecommendSomeMusic.Models.Deezer;
using HeyManCanYouRecommendSomeMusic.Services;
using System.IO;
using HeyManCanYouRecommendSomeMusic.Helpers;
using HeyManCanYouRecommendSomeMusic.Models.Relationships;

namespace HeyManCanYouRecommendSomeMusic.Controllers
{
    [Route("home")]
    public class HomeController : Controller
    {
        WebCrawlerService webCrawlerService = new WebCrawlerService();
        IDBService dBService = new DBService();
        IDeezer deezerService = new DeezerService();

        [Route("")]
        [Route("/")]
        [Route("index")]
        public IActionResult Index()
		{ 
			/*Deezer();*/
            return View();
        }

        [HttpPost("submit")]
        public async Task<JsonResult> Submit([FromBody] string songUrl)
        {
            string[] artistAndName = new string[2];
            try
            {
                artistAndName = await GetSong(songUrl);
            }
            catch(Exception e)
            {
                return new JsonResult(new { succ = false, msg = e.Message });
            }

            Song song = dBService.GetSongByNameAndArtist(artistAndName[1], artistAndName[0]);
            if (song == null)
                return new JsonResult(new
                {
                    succ = false,
                    msg = "Cannot find song in database"
                });
            
            List<Song> artistSongs = dBService.GetSongsWithSameArtist(song);
            artistSongs.Remove(artistSongs.FirstOrDefault(s => s.name == song.name));

            List<Song> genreSongs = dBService.GetSongsInSameGenre(song);
            genreSongs.Remove(genreSongs.FirstOrDefault(s => s.name == song.name));

            List<Song> bpmSongs = dBService.GetSongWithinTempo(Int32.Parse(song.bpm) - 30, Int32.Parse(song.bpm) + 30);
            bpmSongs.Remove(bpmSongs.FirstOrDefault(s => s.name == song.name));

            List<Song> durationSongs = dBService.GetSongWithinDuration(Int32.Parse(song.duration) - 60, Int32.Parse(song.duration) + 60);
            durationSongs.Remove(durationSongs.FirstOrDefault(s => s.name == song.name));

            Random r = new Random();
            r.Next(0, Enum.GetValues(typeof(MetalRelationship.MetalRelationshipType)).Length - 1);

            Array values = Enum.GetValues(typeof(MetalRelationship.MetalRelationshipType));
            Random random = new Random();
            MetalRelationship randomRel = new MetalRelationship((MetalRelationship.MetalRelationshipType)values.GetValue(random.Next(values.Length)));

            List<Song> similarSongs = dBService.GetSimilarSongs(song, randomRel);
            similarSongs.Remove(similarSongs.FirstOrDefault(s => s.name == song.name));

            return new JsonResult(new
            {
                succ = true,
                artist = artistSongs,
                genre = genreSongs,
                bpm = bpmSongs,
                duration = durationSongs,
                similar = similarSongs
            });
        } 

        [HttpPost("submit-new")]
        public JsonResult Submit([FromBody] Song song)
        {
            bool res = dBService.AddNewSong(song);

            if (song.genre.Contains("etal"))
            {
                if (Int32.Parse(song.bpm) < 110)
                {
                    Song s1 = dBService.GetSongByNameAndArtist(song.name, song.band);
                    Song s2 = dBService.GetSongsInSameGenre(s1).FirstOrDefault();

                    dBService.CreateRelationship(s1, s2, new MetalRelationship(MetalRelationship.MetalRelationshipType.DRONING));
                }
                else if(Int32.Parse(song.bpm) > 170)
                {
                    Song s1 = dBService.GetSongByNameAndArtist(song.name, song.band);
                    Song s2 = dBService.GetSongsInSameGenre(s1).FirstOrDefault();

                    dBService.CreateRelationship(s1, s2, new MetalRelationship(MetalRelationship.MetalRelationshipType.ENERGETIC));
                }
                
                if(Int32.Parse(song.duration) > 600)
                {
                    Song s1 = dBService.GetSongByNameAndArtist(song.name, song.band);
                    Song s2 = dBService.GetSongsInSameGenre(s1).FirstOrDefault();

                    dBService.CreateRelationship(s1, s2, new MetalRelationship(MetalRelationship.MetalRelationshipType.EPIC));
                }
            }

            return new JsonResult(new { succ = res });
        }

        [HttpPost("populate")]
        public async Task<JsonResult> Populate()
        {
            //await Deezer();
            return new JsonResult(new { succ = true });
        }

        private async Task<string[]> GetSong(string link)
        {
            string[] artistAndName = await webCrawlerService.GetSongName(link);
            return artistAndName;
        }

        async Task Deezer()
        {
            var populator = new DBPopulator(dBService, deezerService);
            await populator.PopulateForAllGenres(3);
        }
    }
}
