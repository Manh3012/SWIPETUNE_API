using Newtonsoft.Json;
using System.Text.Json;
using DataAccess.Interface;
using BusinessObject.Sub_Model;

namespace DataAccess.Service
{
    public class SpotifyService : ISpotifyService
    {
        private readonly HttpClient _httpClient;

        public SpotifyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
            string encodedQuery = Uri.EscapeDataString(searchQuery);
            string url = $"search?q={encodedQuery}&type=artist";

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
    }
}
