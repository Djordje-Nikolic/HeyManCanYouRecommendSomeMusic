using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HeyManCanYouRecommendSomeMusic.Models.Deezer
{
    public class Album : DeezerObject
    {
        [JsonPropertyName("title")] public string Title { get; set; }
        [JsonPropertyName("upc")] public string UPC { get; set; }
        [JsonPropertyName("link")] public Uri Link { get; set; }
        [JsonPropertyName("share")] public Uri ShareLink { get; set; }
        [JsonPropertyName("cover")] public Uri CoverUrl { get; set; }
        [JsonPropertyName("cover_small")] public Uri SmallCoverUrl { get; set; }
        [JsonPropertyName("cover_medium")] public Uri MediumCoverUrl { get; set; }
        [JsonPropertyName("cover_big")] public Uri BigCoverUrl { get; set; }
        [JsonPropertyName("cover_xl")] public Uri XLCoverUrl { get; set; }
        [JsonPropertyName("genre_id")] public int FirstGenreID { get; set; }
        [JsonPropertyName("genres")] public DataWrapper<Genre> Genres { get; set; }
        [JsonPropertyName("label")] public string LabelName { get; set; }
        [JsonPropertyName("nb_tracks")] public int TrackNumber { get; set; }
        [JsonPropertyName("duration")] public int Duration { get; set; }
        [JsonPropertyName("fans")] public int FanNumber { get; set; }
        [JsonPropertyName("rating")] public int Rating { get; set; }
        [JsonPropertyName("release_date")] public DateTime ReleaseDate { get; set; }
        [JsonPropertyName("record_type")] public string RecordType { get; set; }
        [JsonPropertyName("available")] public bool IsAvailable { get; set; }
        [JsonPropertyName("alternative")] public Album AlternativeAlbum { get; set; }
        [JsonPropertyName("tracklist")] public Uri TracklistAPILink { get; set; }
        [JsonPropertyName("explicit_lyrics")] public bool HasExplicitLyrics { get; set; }
        [JsonPropertyName("explicit_content_lyrics")] public int ExplicitLyricsNum { get; set; }
        [JsonPropertyName("explicit_content_cover")] public int ExplicitCoverNum { get; set; }
        [JsonPropertyName("contributors")] public IList<Artist> Contributors { get; set; }
        [JsonPropertyName("artist")] public Artist Artist { get; set; }
        [JsonPropertyName("tracks")] public DataWrapper<Track> Tracks { get; set; }

        public void Expand(Album album)
        {
            if (album == null)
                throw new ArgumentNullException();

            UPC = album.UPC;
            ShareLink = album.ShareLink;
            FirstGenreID = album.FirstGenreID;
            Genres = album.Genres;
            LabelName = album.LabelName;
            TrackNumber = album.TrackNumber;
            Duration = album.Duration;
            FanNumber = album.FanNumber;
            Rating = album.Rating;
            ReleaseDate = album.ReleaseDate;
            IsAvailable = album.IsAvailable;
            AlternativeAlbum = album.AlternativeAlbum;
            ExplicitLyricsNum = album.ExplicitLyricsNum;
            ExplicitCoverNum = album.ExplicitCoverNum;
            Contributors = album.Contributors;
        }
    }
}
