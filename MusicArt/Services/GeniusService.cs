using MusicArt.ViewModels.Genius;
using Paszt.WPF.Extensions;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace MusicArt.Services
{
    public sealed class GeniusService : IDisposable
    {
        private const string songSearchUrl = "https://genius.com/api/search/song?page=1&q=";
        private HttpClient _client;
        private const string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.72 Safari/537.36";
        internal HttpClient Client
        {
            get
            {
                if (null == _client)
                {
                    _client = new(new Resources.RetryHandler(new SocketsHttpHandler()));
                    _client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
                }
                return _client;
            }
        }

        public async Task<ObservableCollection<GeniusSearchResult>> SongSearchAsync(string query)
        {
            //var temp = await Client.GetStringAsync(songSearchUrl + HttpUtility.UrlEncode(query));
            ObservableCollection<GeniusSearchResult> collection = new();
            HttpResponseMessage response = await Client.GetAsync(songSearchUrl + HttpUtility.UrlEncode(query))
                .ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException("Response status code does not indicate success: " + response.ReasonPhrase);
            using Stream contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            using JsonDocument jsonDocument = await JsonDocument.ParseAsync(contentStream).ConfigureAwait(false);
            if (jsonDocument.RootElement.TryGetProperty("response", out JsonElement searchResponse) &&
                searchResponse.TryGetProperty("sections", out JsonElement sections) &&
                sections.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement section in sections.EnumerateArray())
                {
                    if (section.TryGetProperty("hits", out JsonElement hits) &&
                       hits.ValueKind == JsonValueKind.Array)
                    {
                        foreach (JsonElement hit in hits.EnumerateArray())
                        {
                            if (hit.TryGetProperty("result", out JsonElement result))
                            {
                                collection.Add(result.ToObject<GeniusSearchResult>());
                            }
                        }
                    }
                }
            }
            //GeniusSearch geniusSearch = await Client.GetFromJsonAsync<GeniusSearch>(songSearchUrl + HttpUtility.UrlEncode(query))
            //    .ConfigureAwait(false);
            //foreach (GeniusSearchResponseSection section in geniusSearch.Response.Sections)
            //    foreach (GeniusSearchHit hit in section.Hits)
            //        collection.Add(hit.Result);
            return collection;
        }

        public void Dispose() => Client?.Dispose();
    }
}
