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

            Song song = new Song { band = artistAndName[0], name = artistAndName[1] };

            List<Song> artistSongs = dBService.GetSongsWithSameArtist(song);
            List<Song> genreSongs = dBService.GetSongsInSameGenre(song);
            
            return new JsonResult(new
            {
                succ = true,
                artist = artistSongs,
                genre = genreSongs
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
