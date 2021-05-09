using MusicArt.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace MusicArt.ViewModels.Genius
{
    public class GeniusSearch
    {
        [JsonPropertyName("response")]
        public GeniusSearchResponse Response { get; set; }
    }

    public class GeniusSearchResponse
    {
        [JsonPropertyName("sections")]
        public List<GeniusSearchResponseSection> Sections { get; set; }

        [JsonPropertyName("next_page")]
        public long NextPage { get; set; }
    }

    public class GeniusSearchResponseSection
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("hits")]
        public List<GeniusSearchHit> Hits { get; set; }
    }

    public class GeniusSearchHit
    {
        [JsonPropertyName("highlights")]
        public List<object> Highlights { get; set; }

        [JsonPropertyName("index")]
        public string Index { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("result")]
        public GeniusSearchResult Result { get; set; }
    }

    public partial class GeniusSearchResult : BindableModelBase
    {
        private string type;
        private string apiPath;
        private string fullTitle;
        private int id;
        private bool instrumental;
        private string lyricsState;
        private Uri songArtImageThumbnailUri;
        private Uri songArtImageUri;
        private string title;
        private string titleWithFeatured;
        private long updatedByHumanAt;
        private Uri uri;

        [JsonPropertyName("_type")]
        public string Type { get => type; set => SetProperty(ref type, value); }

        [JsonPropertyName("api_path")]
        public string ApiPath { get => apiPath; set => SetProperty(ref apiPath, value); }

        [JsonPropertyName("full_title")]
        public string FullTitle { get => fullTitle; set => SetProperty(ref fullTitle, value); }

        [JsonPropertyName("id")]
        public int Id { get => id; set => SetProperty(ref id, value); }

        [JsonPropertyName("instrumental")]
        public bool Instrumental { get => instrumental; set => SetProperty(ref instrumental, value); }

        [JsonPropertyName("lyrics_state")]
        public string LyricsState { get => lyricsState; set => SetProperty(ref lyricsState, value); }

        [JsonPropertyName("song_art_image_thumbnail_url")]
        public Uri SongArtImageThumbnailUri { get => songArtImageThumbnailUri; set => SetProperty(ref songArtImageThumbnailUri, value); }

        [JsonPropertyName("song_art_image_url")]
        public Uri SongArtImageUri { get => songArtImageUri; set => SetProperty(ref songArtImageUri, value); }

        [JsonPropertyName("title")]
        public string Title { get => title; set => SetProperty(ref title, value); }

        [JsonPropertyName("title_with_featured")]
        public string TitleWithFeatured { get => titleWithFeatured; set => SetProperty(ref titleWithFeatured, value); }

        [JsonPropertyName("updated_by_human_at")]
        public long UpdatedByHumanAt { get => updatedByHumanAt; set => SetProperty(ref updatedByHumanAt, value); }

        [JsonPropertyName("url")]
        public Uri Uri { get => uri; set => SetProperty(ref uri, value); }

        [JsonPropertyName("primary_artist")]
        public PrimaryArtist PrimaryArtist { get; set; }

        private RelayCommand<GeniusSearchResult> navigateCommand;
        public RelayCommand<GeniusSearchResult> NavigateCommand => navigateCommand ??=
            new((GeniusSearchResult result) =>
            {
                ProcessStartInfo psi = new ProcessStartInfo { FileName = Uri.ToString(), UseShellExecute = true };
                Process.Start(psi);
            });
    }

    public partial class PrimaryArtist
    {
        [JsonPropertyName("_type")]
        public string Type { get; set; }

        [JsonPropertyName("api_path")]
        public string ApiPath { get; set; }

        [JsonPropertyName("header_image_url")]
        public Uri HeaderImageUrl { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("image_url")]
        public Uri ImageUrl { get; set; }

        [JsonPropertyName("index_character")]
        public string IndexCharacter { get; set; }

        [JsonPropertyName("is_meme_verified")]
        public bool IsMemeVerified { get; set; }

        [JsonPropertyName("is_verified")]
        public bool IsVerified { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("url")]
        public Uri Url { get; set; }

        [JsonPropertyName("iq")]
        public long? Iq { get; set; }
    }
}
