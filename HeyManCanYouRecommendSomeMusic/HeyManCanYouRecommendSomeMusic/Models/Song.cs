using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeyManCanYouRecommendSomeMusic.Models
{
    public class Song
    {
        public string id { get; set; }
        public string name { get; set; }
        public string band { get; set; }
        public string genre { get; set; }
        public string duration { get; set; }
        public string bpm { get; set; }
    }
}
