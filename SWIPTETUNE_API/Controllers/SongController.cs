using System.Text;
using System.Diagnostics;
using BusinessObject.Models;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SWIPTETUNE_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public SongController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet("token")]
        public async Task<IActionResult> GetAccessToken()
        {
            var client = _httpClientFactory.CreateClient();

            var clientId = _configuration.GetValue<string>("SpotifyApi:ClientId");
            var clientSecret = _configuration.GetValue<string>("SpotifyApi:ClientSecret");

            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");

            var requestBody = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" }
        };

            var requestContent = new FormUrlEncodedContent(requestBody);
            tokenRequest.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"))
            );
            tokenRequest.Content = requestContent;

            var response = await client.SendAsync(tokenRequest);



            var content = await response.Content.ReadAsStringAsync();

            return Ok(new
            {
                content = content
            });
        }
        [HttpGet("tracks/{trackId}")]
        public async Task<IActionResult> GetTrack(string trackId, string accessToken)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var trackRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.spotify.com/v1/tracks/{trackId}");
            var trackResponse = await client.SendAsync(trackRequest);

            if (!trackResponse.IsSuccessStatusCode)
            {
                return BadRequest("Failed to retrieve track information.");
            }

            var trackContent = await trackResponse.Content.ReadAsStringAsync();
            var filePath = "C:\\Users\\Admin\\OneDrive\\Desktop\\PRN221PE_SP23_223469_VUVANMANH\\SWIPETUNE_API\\SWIPTETUNE_API\\track.json";
            await System.IO.File.WriteAllTextAsync(filePath, trackContent);
            var configuration = new ConfigurationBuilder()
                                        .AddJsonFile(filePath)
                                         .Build();
            Song song = new Song
            {
                SongId = configuration.GetSection("id").Value,
                Song_title = configuration.GetSection("name").Value,
                song_img_url = configuration.GetSection("album:images:0:url").Value,


            };
            return Ok(song);
        }


        // Other endpoints...

        private class SpotifyAccessTokenResponse
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
        }
    }
}
