using BusinessObject;
using DataAccess.Interface;
using Repository.Interface;
using BusinessObject.Models;
using BusinessObject.Sub_Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace SWIPTETUNE_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : ControllerBase
    {
        private readonly ISpotifyAccountService spotifyAccountService;
        private readonly ISpotifyService spotifyService;

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IArtistRepository artistRepository;
        private readonly SWIPETUNEDbContext context;
        public ArtistController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ISpotifyAccountService spotifyAccountService, ISpotifyService spotifyService,IArtistRepository artistRepository,SWIPETUNEDbContext context)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            this.spotifyAccountService = spotifyAccountService;
            this.spotifyService = spotifyService;
            this.artistRepository = artistRepository;
            this.context = context;
        }

        [HttpGet]
        [Route("GetArtisDetail")]
        public async Task<List<Artist>> GetArtistDetail([FromQuery] List<string> artistIds)
        {
            var ListArtist = new List<Artist>();
            string accessToken = "";
            try
            {
                accessToken = await spotifyService.GetAccessToken();

                foreach (var artistId in artistIds)
                {
                    var artistSpotify = await spotifyService.SearchArtists(artistId, accessToken);
                    Artist artist = ArtistConverter.ConvertFromArtistSpotify(artistSpotify);
                    artist.Songs = await spotifyService.GetTopTracks(artistId, accessToken);
                    ListArtist.Add(artist);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ListArtist;
        }
        [HttpGet]
        [Route("GetListArtists")]
        public async Task<ActionResult<IEnumerable<string>>> GetArtistIds(string query)
        {
            var accessToken = await spotifyService.GetAccessToken();
            try
            {
                var artistIds = await spotifyService.GetArtistIds(query, accessToken);
                return Ok(artistIds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("add-artists")]
        public async Task<IActionResult> AddArtistsToDatabase([FromBody] List<string> artistIds)
        {
            try
            {
                // Create a list of Artist objects
                string accessToken = await spotifyService.GetAccessToken();

                var artists = new List<Artist>();

                // Retrieve details for each artist ID and create an Artist object
                foreach (var artistId in artistIds)
                {
                    // Make the API request to retrieve artist details
                    var artistSpotify = await spotifyService.SearchArtists(artistId, accessToken);
                    Artist artist = ArtistConverter.ConvertFromArtistSpotify(artistSpotify);
                    if (artist != null)
                    {
                        artists.Add(artist);
                    }
                }

                // Add the artists to the database
                artistRepository.AddArtist(artists);
                return Ok("Artists added to the database.");
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return StatusCode(500, $"Error adding artists to the database: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("GetListArtistInformation")]
        public async Task<List<Artist>> GetListArtists()
        {
            var list = await context.Artists.ToListAsync();
            if (list.Count == 0)
            {
                return new List<Artist>();
            }
            return list;
        }
    }
}
