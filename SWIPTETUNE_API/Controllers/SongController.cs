using System.Text;
using System.Diagnostics;
using DataAccess.Interface;
using BusinessObject.Models;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using BusinessObject.Sub_Model;
using Microsoft.AspNetCore.Authorization;

namespace SWIPTETUNE_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private readonly ISpotifyAccountService spotifyAccountService;
        private readonly ISpotifyService spotifyService;

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public SongController(IHttpClientFactory httpClientFactory, IConfiguration configuration,ISpotifyAccountService spotifyAccountService,ISpotifyService spotifyService)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            this.spotifyAccountService = spotifyAccountService;
            this.spotifyService = spotifyService;
        }

        [HttpGet("tracks/{trackId}")]
        public async Task<IActionResult> GetTrack(string trackId)
        {
            var track = new Track();
           try
            {
                var accessToken= await spotifyService.GetAccessToken();
                track = await spotifyService.GetSongs(trackId, accessToken);
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            Song song = new Song
            {
                SongId = trackId,
                Song_title = track.name,
                ArtistId = track.artists.FirstOrDefault().id,
                Duration = TimeSpan.FromSeconds(track.duration_ms),
                ReleaseDate = DateTime.Parse(track.album.release_date),
                song_img_url = track.album.images.FirstOrDefault()?.url,

        };
            
            return Ok(song);
        }


        // Other endpoints...

        private class SpotifyAccessTokenResponse
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
        }
    }
}
