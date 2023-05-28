using DataAccess.Interface;
using BusinessObject.Models;
using BusinessObject.Sub_Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

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
        public ArtistController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ISpotifyAccountService spotifyAccountService, ISpotifyService spotifyService)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            this.spotifyAccountService = spotifyAccountService;
            this.spotifyService = spotifyService;
        }

        [HttpGet]
        public async Task<List<Artist>> GetArtist([FromQuery] List<string> artistIds)
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
        [Route("ListArtistId")]
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
    }
}
