using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using DataAccess.Interface;
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
        private const string RedirectUri = "https://localhost:7134/Spotify/Callback";
        private const string Scopes = "playlist-modify-public user-read-private user-read-email playlist-read-collaborative playlist-read-private playlist-modify-private";
        public SpotifyAccountController(HttpClient _httpClient, IConfiguration configuration, ISpotifyAccountService spotifyAccountService, ISpotifyService spotifyService)
        {
            this._httpClient = _httpClient;
            _configuration = configuration;
            this.spotifyAccountService = spotifyAccountService;
            this.spotifyService = spotifyService;
        }
        [HttpGet("login")]
        public string LoginWithSpotify()
        {
            var clientId = _configuration.GetValue<string>("SpotifyApi:ClientId");
            // Construct the Spotify authorization URL
            var authUrl = $"https://accounts.spotify.com/authorize?client_id={clientId}&response_type=code&redirect_uri={RedirectUri}&scope={Scopes}";

            return authUrl;
        } 
        }

    }

