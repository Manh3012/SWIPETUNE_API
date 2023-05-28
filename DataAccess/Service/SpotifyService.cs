using Newtonsoft.Json;
using System.Net.Http;
using System.Text.Json;
using DataAccess.Interface;
using BusinessObject.Models;
using BusinessObject.Sub_Model;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Service
{
    public class SpotifyService : ISpotifyService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ISpotifyAccountService spotifyAccountService;


        public SpotifyService(HttpClient httpClient, IConfiguration configuration, ISpotifyAccountService spotifyAccountService)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            this.spotifyAccountService = spotifyAccountService;
        }
        public async Task<Track> GetSongs(string trackId, string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync($"tracks/{trackId}");
            response.EnsureSuccessStatusCode();
            using var responseStream = await response.Content.ReadAsStreamAsync();
            var responseObject = await System.Text.Json.JsonSerializer.DeserializeAsync<Track>(responseStream);
            return responseObject;




        }
        public async Task<ArtistSpotify> SearchArtists(string searchQuery,string accessToken)
        {
            {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            string url = $"artists/{searchQuery}";

                try
                {
                    HttpResponseMessage response = await _httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<ArtistSpotify>(json);
                        return data;
                    }
                    else
                    {
                        // Handle error response
                        Console.WriteLine($"Request failed with status code {response.StatusCode}");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    // Handle exception
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    return null;
                }
            }
        }

        public async Task<string> GetAccessToken()
        {

            var clientId = _configuration.GetValue<string>("SpotifyApi:ClientId");
            var clientSecret = _configuration.GetValue<string>("SpotifyApi:ClientSecret");
            var content = await spotifyAccountService.GetToken(clientId, clientSecret);

            return content;
        }
        public async Task<List<string>> GetArtistIds(string query, string accessToken)
        {
            string url = $"search?q={Uri.EscapeDataString(query)}&type=artist&limit=20";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var response = await _httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to retrieve artist IDs. Error: {responseContent}");
            }

            var artistIds = new List<string>();
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseContent);

            if (data["artists"] != null && data["artists"]["items"] != null)
            {
                foreach (var artist in data["artists"]["items"])
                {
                    artistIds.Add(artist["id"].ToString());
                }
            }

            return artistIds;
        }
        public async Task<List<Song>> GetTopTracks(string artistId, string accessToken)
        {
            string url = $"artists/{artistId}/top-tracks?country=VN";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var response = await _httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to retrieve top tracks. Error: {responseContent}");
            }

            var tracksResponse = JsonConvert.DeserializeObject<TopTracksResponse>(responseContent);

            var songs = new List<Song>();
            foreach (var track in tracksResponse.Tracks)
            {
                DateTime? releaseDate = null;
                if (!string.IsNullOrEmpty(track.album.release_date))
                {
                    if (DateTime.TryParse(track.album.release_date, out DateTime parsedDate))
                    {
                        releaseDate = parsedDate;
                    }
                }
                var song = new Song
                {
                    SongId = track.id,
                    ArtistId = artistId,
                    Song_title = track.name,
                    Duration = TimeSpan.FromMilliseconds(track.duration_ms),
                    ReleaseDate = releaseDate,
                    song_img_url = track.album.images[0].url
                };

                songs.Add(song);
            }

            return songs;
        }

    }
}
