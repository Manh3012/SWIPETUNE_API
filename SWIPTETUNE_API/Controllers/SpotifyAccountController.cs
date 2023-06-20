using System;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http;
using System.Diagnostics;
using DataAccess.Interface;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
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
        private const string RedirectUri = "https://localhost:7049/api/SpotifyAccount/callback";
        // private const string RedirectUri = "http://18.141.188.211:7049/api/SpotifyAccount/callback";

        private const string Scopes = "playlist-modify-public user-read-private user-read-email playlist-read-collaborative playlist-read-private playlist-modify-private";
        string clientId = "64a2a8a63cb7433a8f8e5bc11f62e189";
        string clientSecret = "b0a49f2eab454292bff2c5d5bb7613c3";
        public SpotifyAccountController(HttpClient _httpClient, IConfiguration configuration, ISpotifyAccountService spotifyAccountService, ISpotifyService spotifyService)
        {
            this._httpClient = _httpClient;
            _configuration = configuration;
            this.spotifyAccountService = spotifyAccountService;
            this.spotifyService = spotifyService;
        }
        //[HttpGet("login")]
        //public async Task<IActionResult> LoginWithSpotify(string? code)
        //{
        //    if (string.IsNullOrEmpty(code))
        //    {
        //        // No authorization code provided, redirect the user to Spotify login page
        //        var clientId = _configuration.GetValue<string>("SpotifyApi:ClientId");
        //        // Construct the Spotify authorization URL
        //        var authUrl = $"https://accounts.spotify.com/authorize?client_id={clientId}&response_type=code&redirect_uri={RedirectUri}&scope={Scopes}";

        //        return Ok(authUrl);
        //    }
        //    else
        //    {
        //        // Authorization code is provided, exchange it for an access token
        //        var httpClient = new HttpClient();
        //        var requestBody = new FormUrlEncodedContent(new[]
        //        {
        //    new KeyValuePair<string, string>("client_id", _configuration.GetValue<string>("SpotifyApi:ClientId")),
        //    new KeyValuePair<string, string>("client_secret", _configuration.GetValue<string>("SpotifyApi:ClientSecret")),
        //    new KeyValuePair<string, string>("grant_type", "authorization_code"),
        //    new KeyValuePair<string, string>("code", code),
        //    new KeyValuePair<string, string>("redirect_uri", RedirectUri)
        //});

        //        var response = await httpClient.PostAsync("https://accounts.spotify.com/api/token", requestBody);
        //        var responseContent = await response.Content.ReadAsStringAsync();

        //        var accessToken = JObject.Parse(responseContent)["access_token"].ToString();
        //        return Ok(new { access_token = accessToken });
        //    }
        //}

        //[HttpGet("login")]
        //public async Task<string> GetSpotifyAccessToken()
        //{
        //    string responseType = "code";

        //    string authUrl = "https://accounts.spotify.com/authorize" +
        //        $"?response_type={responseType}" +
        //        $"&client_id={clientId}" +
        //        $"&scope={Scopes}" +
        //        $"&redirect_uri={RedirectUri}";

        //    // Launch a web browser to authenticate
        //    string result = await OpenWebBrowser(authUrl);

        //    // Extract the authorization code from the callback URL
        //    Uri callbackUri = new Uri(result);
        //    string code = callbackUri.Query.Substring(1).Split('&')[0].Split('=')[1];

        //    string tokenUrl = "https://accounts.spotify.com/api/token";

        //    using (HttpClient client = new HttpClient())
        //    {
        //        // Set the request headers
        //        client.DefaultRequestHeaders.Authorization =
        //            new System.Net.Http.Headers.AuthenticationHeaderValue(
        //                "Basic",
        //                Convert.ToBase64String(
        //                    System.Text.Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}")
        //                )
        //            );
        //        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

        //        // Set the request body
        //        var body = new System.Collections.Generic.List<KeyValuePair<string, string>>
        //{
        //    new KeyValuePair<string, string>("grant_type", "authorization_code"),
        //    new KeyValuePair<string, string>("code", code),
        //    new KeyValuePair<string, string>("redirect_uri", RedirectUri)
        //};
        //        var requestBody = new System.Net.Http.FormUrlEncodedContent(body);

        //        // Send the POST request to exchange the authorization code for an access token
        //        HttpResponseMessage response = await client.PostAsync(tokenUrl, requestBody);
        //        string responseJson = await response.Content.ReadAsStringAsync();

        //        // Extract the access token from the response JSON
        //        var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseJson);
        //        string accessToken = responseObject.access_token;

        //        return accessToken;
        //    }
        //}
        [HttpGet("login")]
        public async Task<IActionResult> LoginWithSpotify()
        {
            var clientId = _configuration.GetValue<string>("SpotifyApi:ClientId");
            var authUrl = $"https://accounts.spotify.com/authorize?client_id={clientId}&response_type=code&redirect_uri={RedirectUri}&scope={Scopes}";
            OpenBrowser(authUrl);

            return Ok();
        }

        [HttpGet("callback")]
        public async Task<IActionResult> SpotifyCallback([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Authorization code not found.");
            }

            var tokenUrl = "https://accounts.spotify.com/api/token";

            using (var client = new HttpClient())
            {
                // Prepare the request body
                var requestBody = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", RedirectUri },
            { "client_id", clientId },
            { "client_secret", clientSecret }
        };

                // Send the POST request to exchange the authorization code for an access token
                var response = await client.PostAsync(tokenUrl, new FormUrlEncodedContent(requestBody));
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Parse the response JSON to extract the access token
                    var responseObject = JObject.Parse(responseContent);
                    var accessToken = responseObject["access_token"].ToString();

                    // Perform any additional actions with the access token here...

                    return Ok(accessToken);
                }
                else
                {
                    // Handle the case when the token exchange fails
                    // You can customize the error handling based on your requirements
                    return BadRequest("Failed to exchange authorization code for access token.");
                }
            }
        }
        private void OpenBrowser(string url)
        {
            try
            {
                // Launch the default browser with the specified URL
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur while launching the browser
                // You can customize the error handling based on your requirements
                Console.WriteLine($"Failed to open browser: {ex.Message}");
            }



        }
    }
}
