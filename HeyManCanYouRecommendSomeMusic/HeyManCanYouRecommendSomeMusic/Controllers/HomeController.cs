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

            Song s1 = dBService.GetSongById(1);
            Song s2 = dBService.GetSongById(3);

            bool res = dBService.CreateRelationship(s1, s2, Relationship.DISTORTED);
            string x = "dsa";
        } 
    }
}
