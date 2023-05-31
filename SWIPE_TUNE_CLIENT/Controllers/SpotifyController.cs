using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using BusinessObject.Sub_Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace SWIPE_TUNE_CLIENT.Controllers
{
    public class SpotifyController : Controller
    {
        private const string ClientId = "64a2a8a63cb7433a8f8e5bc11f62e189";
        private const string ClientSecret = "b0a49f2eab454292bff2c5d5bb7613c3";
        private const string RedirectUri = "https://localhost:7134/Spotify/Callback";
        private const string Scopes = "playlist-modify-public playlist-read-private";


        private readonly HttpClient httpClient = new HttpClient();

        public async Task<ActionResult> Login()
        {
            var apiUrl = "https://localhost:7049/api/SpotifyAccount/login";
            var response = await httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                // Handle API error here
                // For example, return an error view
                return View("Error");
            }

            // Read the response content (login URL)
            var authUrl = await response.Content.ReadAsStringAsync();

            // Pass the login URL to the view
            ViewBag.AuthUrl = authUrl;

            return View();
        }

        //public async Task<ActionResult> Callback(string code)
        //{
        //    // Exchange the authorization code for access and refresh tokens
        //    var tokenUrl = "https://accounts.spotify.com/api/token";
        //    var clientCredentials = $"{ClientId}:{ClientSecret}";
        //    var authorizationHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientCredentials));
        //    var content = new FormUrlEncodedContent(new Dictionary<string, string>
        //{
        //    { "grant_type", "authorization_code" },
        //    { "code", code },
        //    { "redirect_uri", RedirectUri },
        //    { "client_id", ClientId },
        //        { "client_secret", ClientSecret }
        //    });

        //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationHeader);

        //    var response = await httpClient.PostAsync(tokenUrl, content);
        //    var responseContent = await response.Content.ReadAsStringAsync();

        //    // Parse the token response
        //    var tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<AccessToken>(responseContent);
        //    var accessToken = tokenResponse.access_token;
        //    var refreshToken = tokenResponse.refreshToken;
        //    // Store the tokens securely for future use

        //    return RedirectToAction("Index", "Home",tokenResponse);
        //}
            [HttpGet]
            public async Task<IActionResult> Callback(string code)
            {
            var scopes = new List<string>
{
    "playlist-read-private", // Read access to user's private playlists
    "playlist-read-collaborative", // Read access to user's collaborative playlists
    "playlist-modify-public", // Write access to user's public playlists
    // Add more scopes as needed
};
            var scopeString = string.Join(" ", scopes);
            // Exchange the authorization code for an access token and refresh token
            var tokenUrl = "https://accounts.spotify.com/api/token";

                var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", RedirectUri },
            { "client_id", ClientId },
    { "client_secret", ClientSecret },
            { "scope", scopeString },

        });


                var response = await httpClient.PostAsync(tokenUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<AccessToken>(responseContent);
            Response.Cookies.Append("access_token", tokenResponse.access_token);
                // Handle the token response and store the access token and refresh token securely

                return RedirectToAction("Index", "Home", tokenResponse.access_token);
            }

        public IActionResult Logout()
        {
            // Remove the access token cookie
            var accessTokenCookie = Request.Cookies["access_token"];

            if (!string.IsNullOrEmpty(accessTokenCookie))
            {
                // Remove the access token cookie
                Response.Cookies.Delete("accessToken");
            }

            // Redirect to the login page
            return RedirectToAction("Login", "Spotify");
        }


    }
}
