﻿using Newtonsoft.Json;
using DataAccess.Interface;
using Repository.Interface;
using Newtonsoft.Json.Linq;
using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace SWIPTETUNE_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ISpotifyService spotifyService;
        private readonly IGenreRepository genreRepository;

        public GenreController(HttpClient httpClient,ISpotifyService spotifyService,IGenreRepository genreRepository)
        {
            _httpClient = httpClient;
            this.spotifyService = spotifyService;
            this.genreRepository = genreRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetGenres()
        {
            var accessToken = await spotifyService.GetAccessToken();
            string apiUrl = "https://api.spotify.com/v1/recommendations/available-genre-seeds";

            // Set the authorization header with the access token
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            // Make the GET request to retrieve the available genre seeds
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                string responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(responseContent);

                // Extract the genres array from the JSON object
                var genresArray = jsonResponse["genres"]?.ToObject<List<string>>();

                // Create a list of Genre objects
                var genreList = new List<Genre>();

                // Convert the genres to Genre objects
                if (genresArray != null)
                {
                    foreach (var genreName in genresArray)
                    {
                        var genre = new Genre
                        {
                            Name = genreName
                        };

                        genreList.Add(genre);
                    }
                }

                // Add the genres to the database
                foreach (var genre in genreList)
                {
                    // Assuming you have an instance of your database context named 'dbContext'
                    genreRepository.AddGenre(genre);
                }


                return Ok(genreList);
            }
            else
            {
                return StatusCode((int)response.StatusCode, response.ReasonPhrase);
            }
        }
    }
    }

