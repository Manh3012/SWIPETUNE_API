using DataAccess.Interface;
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
        public async Task<ArtistSpotify> GetArtist(string query,string accessToken)
        {
            var ListArtist = new ArtistSpotify();
            try
            {
                ListArtist= await spotifyService.SearchArtists(query, accessToken);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ListArtist;
        }
    }
}
