using BusinessObject;
using Newtonsoft.Json;
using DataAccess.Interface;
using Repository.Interface;
using Newtonsoft.Json.Linq;
using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using BusinessObject.Sub_Model;
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
        private readonly SWIPETUNEDbContext context;
        private readonly IAccountGenreRepository accountGenre;

        public GenreController(HttpClient httpClient, ISpotifyService spotifyService, IGenreRepository genreRepository, SWIPETUNEDbContext context, IAccountGenreRepository accountGenre)
        {
            _httpClient = httpClient;
            this.spotifyService = spotifyService;
            this.genreRepository = genreRepository;
            this.context = context;
            this.accountGenre = accountGenre;
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

        [HttpGet]
        [Route("GetAllGenre")]
        public async Task<List<Genre>> GettAll()
        {
            var list = new List<Genre>();
            try
            {
                list = await context.Genres.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return list;
        }

        [HttpGet]
        [Route("GetListAccountGenre/{accountId}")]
        public async Task<List<GenreModel>> GetGenreAccount(Guid accountId)
        {
            var list = new List<GenreModel>();
            var listgenre = new List<Genre>();
            try
            {
                listgenre= await accountGenre.GetGenreFromAccount(accountId);
                foreach(var genre in listgenre)
                {
                    var genreModel = new GenreModel
                    { 
                        GenreId= genre.GenreId,
                        Name = genre.Name
                    };  
                    list.Add(genreModel);
                }
            }catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return list;
        }



    }
}

