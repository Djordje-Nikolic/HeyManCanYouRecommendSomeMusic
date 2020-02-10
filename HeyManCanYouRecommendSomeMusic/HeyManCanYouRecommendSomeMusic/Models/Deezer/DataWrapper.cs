using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HeyManCanYouRecommendSomeMusic.Models.Deezer
{
    public class DataWrapper<T>
    {
        [JsonPropertyName("data")] public IList<T> Data { get; set; }
    }
}
