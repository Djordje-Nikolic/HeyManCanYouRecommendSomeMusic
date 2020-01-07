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
            return View();
        }

        [HttpPost("submit")]
        public async Task Submit([FromBody] string songUrl)
        {
            //string[] artistAndName = await webCrawlerService.GetSongName(songUrl);

            //Song s = dBService.GetSongById(4);
            //List<Song> songss = dBService.GetSongsInRelationship(Relationship.DISTORTED, 5);

            //List<Song> songs = dBService.GetSimilarSongs(s, Relationship.DISTORTED, 2);

            //string x = "dsa";

            var result = await deezerService.FetchForEachGenre(10);

            using (FileStream fileStream = new FileStream("C:\\deezeroutput.txt", FileMode.Create))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    foreach (var r in result.Values)
                    {
                        streamWriter.WriteLine($"Name: {r.Title} Album: {r.Album.Title} Genre: {r.Album.Genres.Data.First().Name} Artist: {r.Artist.Name} Bpm: {r.Bpm}");
                    }
                }
            }
        } 
    }
}
