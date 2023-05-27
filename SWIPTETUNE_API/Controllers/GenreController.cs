using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace SWIPTETUNE_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public GenreController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        [HttpGet]
        public async Task<IActionResult> GetGenres(string accessToken)
        {
            string apiUrl = "https://api.spotify.com/v1/recommendations/available-genre-seeds";

            // Set the authorization header with the access token
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            // Make the GET request to retrieve the available genre seeds
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                string responseContent = await response.Content.ReadAsStringAsync();

                return Ok(responseContent);
            }
            else
            {
                return StatusCode((int)response.StatusCode, response.ReasonPhrase);
            }
        }
    }
    }

