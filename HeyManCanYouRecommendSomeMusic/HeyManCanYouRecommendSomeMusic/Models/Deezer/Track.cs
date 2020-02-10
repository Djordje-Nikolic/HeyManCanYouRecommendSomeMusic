using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HeyManCanYouRecommendSomeMusic.Models.Deezer
{
    public class Track : DeezerObject
    {
        [JsonPropertyName("readable")] public bool IsReadable { get; set; }
        [JsonPropertyName("title")] public string Title { get; set; }
        [JsonPropertyName("title_short")] public string ShortTitle { get; set; }
        [JsonPropertyName("title_version")] public string TitleVersion { get; set; }
        [JsonPropertyName("unseen")] public bool IsUnseen { get; set; }
        [JsonPropertyName("isrc")] public string ISRC { get; set; }
        [JsonPropertyName("link")] public Uri Link { get; set; }
        [JsonPropertyName("share")] public Uri ShareLink { get; set; }
        [JsonPropertyName("duration")] public int Duration { get; set; }
        [JsonPropertyName("track_position")] public int PositionInAlbum { get; set; }
        [JsonPropertyName("disk_number")] public int AlbumDiskNumber { get; set; }
        [JsonPropertyName("rank")] public int DeezerRank { get; set; }
        [JsonPropertyName("release_date")] public DateTime ReleaseDate { get; set; }
        [JsonPropertyName("explicit_lyrics")] public bool HasExplicitLyrics { get; set; }
        [JsonPropertyName("explicit_content_lyrics")] public int ExplicitLyricsNum { get; set; }
        [JsonPropertyName("explicit_content_cover")] public int ExplicitCoverNum { get; set; }
        [JsonPropertyName("preview")] public Uri PreviewFileLink { get; set; }
        [JsonPropertyName("bpm")] public float Bpm { get; set; }
        [JsonPropertyName("gain")] public float Gain { get; set; }
        [JsonPropertyName("available_countries")] public IList<string> AvailableCountries { get; set; }
        [JsonPropertyName("alternative")] public Track AlternativeTrack { get; set; }
        [JsonPropertyName("contributors")] public IList<Artist> Contributors { get; set; }
        [JsonPropertyName("artist")] public Artist Artist { get; set; }
        [JsonPropertyName("album")] public Album Album { get; set; }

        public void Expand(Track expandedTrack)
        {
            if (expandedTrack == null)
                throw new ArgumentNullException();

            IsReadable = expandedTrack.IsReadable;
            ShareLink = expandedTrack.ShareLink;
            IsUnseen = expandedTrack.IsUnseen;
            ISRC = expandedTrack.ISRC;
            PositionInAlbum = expandedTrack.PositionInAlbum;
            AlbumDiskNumber = expandedTrack.AlbumDiskNumber;
            ReleaseDate = expandedTrack.ReleaseDate;
            Bpm = expandedTrack.Bpm;
            Gain = expandedTrack.Gain;
            AvailableCountries = expandedTrack.AvailableCountries;
            AlternativeTrack = expandedTrack.AlternativeTrack;
            Contributors = expandedTrack.Contributors;
        }
    }
}
