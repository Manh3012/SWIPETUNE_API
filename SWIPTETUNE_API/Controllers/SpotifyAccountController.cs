using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using DataAccess.Interface;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using BusinessObject.Sub_Model;
using Microsoft.AspNetCore.Http;

namespace SWIPTETUNE_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyAccountController : ControllerBase
    {
        private readonly ISpotifyAccountService spotifyAccountService;
        private readonly ISpotifyService spotifyService;

        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;
        private const string RedirectUri = "https://localhost:7049/api/SpotifyAccount/login";
        private const string Scopes = "playlist-modify-public user-read-private user-read-email playlist-read-collaborative playlist-read-private playlist-modify-private";
        public SpotifyAccountController(HttpClient _httpClient, IConfiguration configuration, ISpotifyAccountService spotifyAccountService, ISpotifyService spotifyService)
        {
            this._httpClient = _httpClient;
            _configuration = configuration;
            this.spotifyAccountService = spotifyAccountService;
            this.spotifyService = spotifyService;
        }
        [HttpGet("login")]
        public async Task<IActionResult> LoginWithSpotify(string? code)
        {
            if (string.IsNullOrEmpty(code))
            {
                // No authorization code provided, redirect the user to Spotify login page
                var clientId = _configuration.GetValue<string>("SpotifyApi:ClientId");
                // Construct the Spotify authorization URL
                var authUrl = $"https://accounts.spotify.com/authorize?client_id={clientId}&response_type=code&redirect_uri={RedirectUri}&scope={Scopes}";

                return Ok(authUrl);
            }
            else
            {
                // Authorization code is provided, exchange it for an access token
                var httpClient = new HttpClient();
                var requestBody = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("client_id", _configuration.GetValue<string>("SpotifyApi:ClientId")),
            new KeyValuePair<string, string>("client_secret", _configuration.GetValue<string>("SpotifyApi:ClientSecret")),
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", RedirectUri)
        });

                var response = await httpClient.PostAsync("https://accounts.spotify.com/api/token", requestBody);
                var responseContent = await response.Content.ReadAsStringAsync();

                var accessToken = JObject.Parse(responseContent)["access_token"].ToString();
                return Ok(new { access_token = accessToken });
            }
        }

    }

}
