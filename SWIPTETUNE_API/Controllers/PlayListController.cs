using BusinessObject;
using DataAccess.Service;
using DataAccess.Interface;
using Repository.Interface;
using System.Globalization;
using System.Globalization;
using BusinessObject.Models;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using BusinessObject.Sub_Model;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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
        private readonly SWIPETUNEDbContext _context;
        private readonly IArtistRepository artistRepository;
        private readonly IAccountRepository accountRepository;
        public PlayListController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ISpotifyAccountService spotifyAccountService, ISpotifyService spotifyService,IPlayListRepository playListRepository, SWIPETUNEDbContext _context, IArtistRepository artistRepository, IAccountRepository accountRepository)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            this.spotifyAccountService = spotifyAccountService;
            this.spotifyService = spotifyService;
            this.playListRepository = playListRepository;
            this._context = _context;
            this.artistRepository = artistRepository;
            this.accountRepository = accountRepository;
        }

        [HttpPost]
        [Route("CreatePlaylist")]

        public async Task<IActionResult> CreatePlayList(PlaylistModel playlist1)
        {
            var msg = "";
            try
            {
                var playlist = new Playlist {
                    PlaylistId = GenerateRandomString(22),
                    AccountId = playlist1.AccountId,
                    Name= playlist1.Name,
                    Created= DateTime.Now,
                    playlist_img_url = playlist1.playlist_img_url,
                    isPublic = playlist1.isPublic
                };

                playListRepository.CreatePlayList(playlist);
                msg = "Create success";
            }catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Ok(msg);
        }
        [HttpGet]
        [Route("GetProfileId")]
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
            return Ok(new
            {
                UserProfileId =   msg
            });
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

        [HttpPost]
        [Route("AddTrackToPlaylist/{playlistId}")]
        public async Task<IActionResult> AddTrackToPlaylist(List<string> trackIds, string playlistId,int position)
        {
            string accessToken = await spotifyService.GetAccessToken();
            try
            {
                // Validate and authenticate the user's access token
                if (string.IsNullOrEmpty(playlistId))
                {
                    return BadRequest("Playlist ID is missing.");
                }
                foreach (var trackId in trackIds)
                {
                    PlaylistSong playlistSong = new PlaylistSong
                    {
                        PlaylistId = playListRepository.GetPlaylistId(playlistId),
                        added_at = DateTime.Now,
                        position = position + 1,
                        SongId = trackId
                    };
                    var songDetails = await spotifyService.GetSongs(trackId, accessToken);
                    int durationMs = songDetails.duration_ms;
                    TimeSpan duration = TimeSpan.FromMilliseconds(durationMs);
                    Song song = new Song
                    {
                        SongId = trackId,
                        Song_title = songDetails.name,
                        ArtistId = songDetails.artists.FirstOrDefault().id,
                        Duration = duration,
                        ReleaseDate = DateTime.Parse(songDetails.album.release_date),
                        song_img_url = songDetails.album.images.FirstOrDefault()?.url,
                    };
                    if (await artistRepository.GetArtistById(song.ArtistId) == null)
                    {
                        var artistSpotify = await spotifyService.SearchArtists(song.ArtistId, accessToken);
                        List<Artist> artist = new List<Artist>();
                        Artist artist1 = ArtistConverter.ConvertFromArtistSpotify(artistSpotify);
                        artist.Add(artist1);
                         artistRepository.AddArtist(artist);
                      
                            await _context.Songs.AddAsync(song);
                            await _context.SaveChangesAsync();
                            playListRepository.AddTrackToPlaylist(playlistSong);
                        }else if (await _context.Songs.SingleOrDefaultAsync(x => x.SongId == song.SongId) == null)

                    {
                        await _context.Songs.AddAsync(song);
                        await _context.SaveChangesAsync();
                        playListRepository.AddTrackToPlaylist(playlistSong);
                    }
                    else if (await _context.Songs.SingleOrDefaultAsync(x => x.SongId == song.SongId) != null)
                    {
                        playListRepository.AddTrackToPlaylist(playlistSong);
                    }
                    else
                    {
                        return BadRequest("Already exist the song");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding track to playlist: {ex.Message}");
            }
            return Ok();
        }
        [HttpGet]
        [Route("recommendations")]
        public async Task<IActionResult> GetRecommendation (Guid accountId)
        {
            var accessToken = await spotifyService.GetAccessToken();
            var listSongs = new List<Song>();
            try
            {
                
                listSongs= await spotifyService.GetRecommendation(accountId, accessToken);
            }catch(Exception ex)
            {
                throw new Exception("Cant get songs");
            }
            return Ok(listSongs);
        }
        [HttpGet]
        [Route("GetPlaylistSong")]
        public async Task<IActionResult> GetPlaylistSong(string playlistId)
        {
            var playlist = playListRepository.GetPlaylistSong(playlistId);
            if(playlist==null)
            {
                return BadRequest("Playlist doesnt exist");
            }
            return Ok(playlist);
        }
        /// <summary>
        ///  This is api to sync playlist from database to spotify
        /// </summary>
        /// <param name="playlist">playlist</param>
        /// <param name="accessToken">accessToken</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        [Route("SyncPlaylist")]
        public async Task<IActionResult> SyncPlaylist(Playlist playlist, string accessToken)
        {
            bool isAdded=false;
            try
            {
                isAdded =await spotifyService.SyncPlaylist(playlist, accessToken);
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
          if(isAdded)
            {
                return Ok("Sync completed");
            }else
            {
                return BadRequest("Sync failed");
            }
        }

        public static string GenerateRandomString(int length)
        {
            Random random = new Random();
            string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string randomString = new string(Enumerable.Repeat(letters, length)
                                              .Select(s => s[random.Next(s.Length)])
                                              .ToArray());
            return randomString;
        }
    }
}
