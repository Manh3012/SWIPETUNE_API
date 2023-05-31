using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Diagnostics;
using DataAccess.Interface;
using BusinessObject.Models;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using BusinessObject.Sub_Model;
using System.Collections.Generic;

namespace DataAccess.Service
{
    public class SpotifyAccountService : ISpotifyAccountService
    {
        private readonly HttpClient _httpClient;

        public SpotifyAccountService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<string> GetToken(string clientId, string clientSecret)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "token");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}")));

            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" }
        });
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseStream = await response.Content.ReadAsStreamAsync();
            var authResult = await JsonSerializer.DeserializeAsync<AccessToken>(responseStream);

            return authResult.access_token;
        }




    }
}
