using DataAccess.Service;
using DataAccess.Interface;
using Repository.Interface;
using System.Globalization;
using BusinessObject.Models;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using BusinessObject.Sub_Model;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SWIPTETUNE_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayListController : ControllerBase
    {
        private readonly ISpotifyAccountService spotifyAccountService;
        private readonly ISpotifyService spotifyService;

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IPlayListRepository playListRepository;
        public PlayListController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ISpotifyAccountService spotifyAccountService, ISpotifyService spotifyService,IPlayListRepository playListRepository)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            this.spotifyAccountService = spotifyAccountService;
            this.spotifyService = spotifyService;
            this.playListRepository = playListRepository;
        }

        [HttpPost]
        [Route("createPlaylist")]

        public async Task<IActionResult> CreatePlayList(Playlist playlist,string accessToken)
        {
            var msg = "";
            try
            {
                await spotifyService.CreatePlaylist(playlist, accessToken);
                msg = "Create success";
            }catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Ok(msg);
        }
        [HttpGet]
        public async Task<IActionResult> GetUserPlayList(string accessToken)
        {
            var msg = "";
            try
            {
                msg =  await spotifyService.GetUserProfile(accessToken);
            }catch(Exception ex)
            { 
                msg=ex.Message;
            }
            return Ok(msg);
        }

        [HttpGet]
        [Route("GetPlaylist")]
        public async Task<IActionResult> GetPlayLists(string accessToken)
        {

            try
            {
                // Validate and authenticate the user's access token
                if (string.IsNullOrEmpty(accessToken))
                {
                    return BadRequest("Access token is missing.");
                }

                // Fetch the user's playlists using the Spotify Web API
                var playlists = await spotifyService.FetchUserPlaylists(accessToken);

                // Handle the API response and return the appropriate result
                return Ok(playlists);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to retrieve user playlists. Error: {ex.Message}");
            }
        }

        [HttpPost("{playlistId}/{trackId}")]
        public async Task<IActionResult> AddTrackToPlaylist(string trackId, string playlistId, string accessToken)
        {
            try
            {
                // Validate and authenticate the user's access token
                if (string.IsNullOrEmpty(playlistId))
                {
                    return BadRequest("Playlist ID is missing.");
                }
                else if (string.IsNullOrEmpty(trackId))
                {
                    return BadRequest("Track ID is missing.");
                }
                bool isAdded = await spotifyService.AddTrackToPlaylist(trackId, playlistId, accessToken);

                if (isAdded)
                {
                    return Ok("Track added to playlist successfully.");
                }
                else
                {
                    return BadRequest("Failed to add track to playlist.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding track to playlist: {ex.Message}");
            }
        }
        [HttpGet]
        [Route("recommendations")]
        public async Task<IActionResult> GetRecommendation (string artisId,[FromQuery]List<string> genres,string trackId,string accessToken)
        {
            var listSongs = new List<Song>();
            try
            {
                if (string.IsNullOrEmpty(artisId)|| string.IsNullOrEmpty(trackId))
                {
                    return BadRequest("Missing artisId or track");
                }
                listSongs= await spotifyService.GetRecommendation(artisId, genres, trackId, accessToken);
            }catch(Exception ex)
            {
                throw new Exception("Cant get songs");
            }
            return Ok(listSongs);
        }
    }
}
