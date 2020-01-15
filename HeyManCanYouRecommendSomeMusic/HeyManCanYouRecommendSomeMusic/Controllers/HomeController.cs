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

            return new JsonResult(new
            {
                succ = true,
                artist = artistSongs,
                genre = genreSongs,
                bpm = bpmSongs,
                duration = durationSongs
            });
        } 

        [HttpPost("submit-new")]
        public JsonResult Submit([FromBody] Song song)
        {
            bool res = dBService.AddNewSong(song);

            return new JsonResult(new { succ = res });
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
