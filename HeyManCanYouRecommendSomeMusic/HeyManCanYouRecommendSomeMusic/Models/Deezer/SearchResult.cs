using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HeyManCanYouRecommendSomeMusic.Models.Deezer
{
    public class SearchResult<T> : DataWrapper<T>
    {
        [JsonPropertyName("total")] public int TotalHits { get; set; }
        [JsonPropertyName("next")] public Uri NextAPILink { get; set; }
        [JsonPropertyName("prev")] public Uri PreviousAPILink { get; set; }
    }
}
