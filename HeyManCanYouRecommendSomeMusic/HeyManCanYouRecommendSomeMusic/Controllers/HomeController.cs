using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HeyManCanYouRecommendSomeMusic.Models;
using HeyManCanYouRecommendSomeMusic.Services;

namespace HeyManCanYouRecommendSomeMusic.Controllers
{
    [Route("home")]
    public class HomeController : Controller
    {
        WebCrawlerService webCrawlerService = new WebCrawlerService();
        IDBService dBService = new DBService();

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
            string[] artistAndName = await webCrawlerService.GetSongName(songUrl);

            Song s = dBService.GetSongById(4);

            List<Song> songs = dBService.GetSimilarSongs(s, Relationship.DISTORTED, 2);

            string x = "dsa";
        } 
    }
}
