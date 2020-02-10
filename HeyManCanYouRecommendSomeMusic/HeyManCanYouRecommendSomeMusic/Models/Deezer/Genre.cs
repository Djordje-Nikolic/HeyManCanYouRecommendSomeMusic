using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace HeyManCanYouRecommendSomeMusic.Models.Deezer
{
    public class Genre : DeezerObject
    {
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("picture")] public Uri PictureUrl { get; set; }
        [JsonPropertyName("picture_small")] public Uri SmallPictureUrl { get; set; }
        [JsonPropertyName("picture_medium")] public Uri MediumPictureUrl { get; set; }
        [JsonPropertyName("picture_big")] public Uri BigPictureUrl { get; set; }
        [JsonPropertyName("picture_xl")] public Uri XLPictureUrl { get; set; }
    }
}
