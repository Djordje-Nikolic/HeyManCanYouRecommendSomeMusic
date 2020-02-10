using System;
using System.Text.Json.Serialization;

namespace HeyManCanYouRecommendSomeMusic.Models.Deezer
{
    public class Artist : DeezerObject
    {
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("link")] public Uri Link { get; set; }
        [JsonPropertyName("share")] public Uri ShareLink { get; set; }
        [JsonPropertyName("picture")] public Uri PictureUrl { get; set; }
        [JsonPropertyName("picture_small")] public Uri SmallPictureUrl { get; set; }
        [JsonPropertyName("picture_medium")] public Uri MediumPictureUrl { get; set; }
        [JsonPropertyName("picture_big")] public Uri BigPictureUrl { get; set; }
        [JsonPropertyName("picture_xl")] public Uri XLPictureUrl { get; set; }
        [JsonPropertyName("nb_album")] public int AlbumCount { get; set; }
        [JsonPropertyName("nb_fan")] public int FanCount { get; set; }
        [JsonPropertyName("radio")] public bool HasRadio { get; set; }
        [JsonPropertyName("tracklist")] public Uri TracklistAPILink { get; set; }

        public void Expand(Artist artist)
        {
            if (artist == null)
                throw new ArgumentNullException();

            ShareLink = artist.ShareLink;
            AlbumCount = artist.AlbumCount;
            FanCount = artist.FanCount;
        }
    }
}
