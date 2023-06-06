using System.Text;
using BusinessObject;
using DataAccess.DAO;
using Newtonsoft.Json;
using DataAccess.Interface;
using Newtonsoft.Json.Linq;
using System.Globalization;
using BusinessObject.Models;
using System.Net.Http.Headers;
using BusinessObject.Sub_Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Service
{
    public class SpotifyService : ISpotifyService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ISpotifyAccountService spotifyAccountService;
        private readonly SWIPETUNEDbContext _WIPETUNEDbContext;


        public SpotifyService(HttpClient httpClient, IConfiguration configuration, ISpotifyAccountService spotifyAccountService, SWIPETUNEDbContext dbContext)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            this.spotifyAccountService = spotifyAccountService;
            _WIPETUNEDbContext = dbContext;
        }
        public async Task<Track> GetSongs(string trackId, string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync($"tracks/{trackId}");
            response.EnsureSuccessStatusCode();
            using var responseStream = await response.Content.ReadAsStreamAsync();
            var responseObject = await System.Text.Json.JsonSerializer.DeserializeAsync<Track>(responseStream);
            return responseObject;




        }
        public async Task<ArtistSpotify> SearchArtists(string searchQuery, string accessToken)
        {
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                string url = $"artists/{searchQuery}";

                try
                {
                    HttpResponseMessage response = await _httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<ArtistSpotify>(json);
                        return data;
                    }
                    else
                    {
                        // Handle error response
                        Console.WriteLine($"Request failed with status code {response.StatusCode}");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    // Handle exception
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    return null;
                }
            }
        }

        public async Task<string> GetAccessToken()
        {

            var clientId = _configuration.GetValue<string>("SpotifyApi:ClientId");
            var clientSecret = _configuration.GetValue<string>("SpotifyApi:ClientSecret");
            var content = await spotifyAccountService.GetToken(clientId, clientSecret);

            return content;
        }
        public async Task<List<string>> GetArtistIds(string query, string accessToken)
        {
            string url = $"search?q={Uri.EscapeDataString(query)}&type=artist&limit=20";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var response = await _httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to retrieve artist IDs. Error: {responseContent}");
            }

            var artistIds = new List<string>();
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseContent);

            if (data["artists"] != null && data["artists"]["items"] != null)
            {
                foreach (var artist in data["artists"]["items"])
                {
                    artistIds.Add(artist["id"].ToString());
                }
            }

            return artistIds;
        }
        public async Task<List<Song>> GetTopTracks(string artistId, string accessToken)
        {
            string url = $"artists/{artistId}/top-tracks?country=VN";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var response = await _httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to retrieve top tracks. Error: {responseContent}");
            }

            var tracksResponse = JsonConvert.DeserializeObject<TopTracksResponse>(responseContent);

            var songs = new List<Song>();
            foreach (var track in tracksResponse.Tracks)
            {
                DateTime? releaseDate = null;
                if (!string.IsNullOrEmpty(track.album.release_date))
                {
                    if (DateTime.TryParse(track.album.release_date, out DateTime parsedDate))
                    {
                        releaseDate = parsedDate;
                    }
                }
                var song = new Song
                {
                    SongId = track.id,
                    ArtistId = artistId,
                    Song_title = track.name,
                    Duration = TimeSpan.FromMilliseconds(track.duration_ms),
                    ReleaseDate = releaseDate,
                    song_img_url = track.album.images[0].url,

                };

                songs.Add(song);
            }

            return songs;
        }
        public async Task<string> CreatePlaylist(Playlist Createplaylist, string accessToken)
        {
            var user_id = await GetUserProfile(accessToken);
            var endpoint = $"users/{user_id}/playlists";
            CreatedPlaylist createPlaylistRequest = new CreatedPlaylist
            {
                name = Createplaylist.Name,
                description = "",
                _public = Createplaylist.isPublic
            };

            var json = System.Text.Json.JsonSerializer.Serialize(createPlaylistRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to create playlist. Error: {responseContent}");
            }

            var playlist = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseContent);
            var playlistId = playlist.id.ToString();

            return playlistId;
        }
        public async Task<string> GetUserProfile(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var endpoint = "me";
            var response = await _httpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to retrieve user profile. Error: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var userProfile = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfile>(responseContent);

            return userProfile.id;



        }
        public async Task<bool> AddTrackToPlaylist(List<string> trackIds , string playlistId,int position, string accessToken)
        {
            var apiUrl = $"playlists/{playlistId}/tracks";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var requestBody = new
            {
                uris = trackIds.ConvertAll(id => $"spotify:track:{id}"),
                position = position
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            return response.IsSuccessStatusCode;
        }
        public async Task<Playlist> GetPlaylist(string playlistId, string accessToken)
        {
            var apiUrl = $"playlists/{playlistId}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var playlistJson = JObject.Parse(content);

                var playlistDetail = new Playlist
                {
                    PlaylistId = playlistJson["id"]?.ToString(),
                    Name = playlistJson["name"]?.ToString(),
                    Created = playlistJson["created"]?.ToObject<DateTime?>(),
                    isPublic = playlistJson["public"]?.ToObject<bool>() ?? false,
                    playlist_img_url = playlistJson["images"]?.FirstOrDefault()?["url"]?.ToString(),
                };

                return playlistDetail;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<Playlist>> FetchUserPlaylists(string accessToken)
        {
            var playlists = new List<Playlist>();
            var apiUrl = "https://api.spotify.com/v1/me/playlists";

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var responseJson = JObject.Parse(content);

                    var playlistsJson = responseJson["items"];

                    foreach (var playlistJson in playlistsJson)
                    {
                        var playlist = new Playlist
                        {
                            PlaylistId = playlistJson["id"].ToString(),
                            Name = playlistJson["name"]?.ToString(),
                            Created = playlistJson["created"]?.ToObject<DateTime?>(),
                            playlist_img_url = playlistJson["images"]?.FirstOrDefault()?["url"]?.ToString(),
                            isPublic = playlistJson["public"]?.ToObject<bool>() ?? false
                        };

                        playlists.Add(playlist);
                    }
                }
            }
            return playlists;
        }


        public async Task<List<Song>> GetRecommendation(Guid accountId, string accessToken)
        {
            var existAccount = await _WIPETUNEDbContext.Accounts
                            .Include(x => x.AccountArtists)
                            .ThenInclude(x => x.Artist)
                            .Include(x => x.AccountSubscriptions)
                            .ThenInclude(x => x.Subscription)
                            .Include(X => X.Playlists)
                            .ThenInclude(X => X.PlaylistSongs)
                            .Include(x => x.AccountGenres)
                            .ThenInclude(x => x.Genre)

                            .SingleOrDefaultAsync(x => x.Id == accountId);
            if (existAccount == null)
            {
                throw new Exception("No account match");
            }
            var listSongs = new List<Song>();
            var apiUrl = "recommendations";
            var queryParams = new List<string>
            {
                $"seed_artists={Uri.EscapeDataString(string.Join(",", existAccount.AccountArtists.Select(x => x.ArtistId).ToList()))}",
                $"seed_genres={Uri.EscapeDataString(string.Join(",", existAccount.AccountGenres.Select(x => x.Genre.Name)))}",
                "min_popularity=50", // Optional: Adjust the popularity threshold as needed
            };
            var requestUrl = $"{apiUrl}?{string.Join("&", queryParams)}";
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            var response = await _httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                // Parse the response content
                var responseContent = await response.Content.ReadAsStringAsync();
                var tracks = JObject.Parse(responseContent)["tracks"].ToObject<List<Track>>();
                var songs = tracks.Select(track => new Song
                {
                   SongId = track.id,
                    Song_title = track.name,
                    ArtistId = track.artists.FirstOrDefault()?.id,
                    Duration = TimeSpan.FromSeconds(track.duration_ms),
                    ReleaseDate = DateTime.TryParseExact(track.album.release_date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate) ? parsedDate : DateTime.MinValue,
                    song_img_url = track.album.images.FirstOrDefault()?.url,
                    
                }).ToList();
                foreach (var song in songs)
                {
                    var artistSpotify = await SearchArtists(song.ArtistId, accessToken);
                    Artist artist = ArtistConverter.ConvertFromArtistSpotify(artistSpotify);
                    song.Artist = artist;
                }
                return songs;
            }
            else
            {
                return new List<Song>();
            }
        }
        public async Task<bool> SyncPlaylist(Playlist Createplaylist, string accessToken)
        {
            var user_id = await GetUserProfile(accessToken);
            var endpoint = $"users/{user_id}/playlists";
            CreatedPlaylist createPlaylistRequest = new CreatedPlaylist
            {
                name = Createplaylist.Name,
                description = "",
                _public = Createplaylist.isPublic
            };

            var json = System.Text.Json.JsonSerializer.Serialize(createPlaylistRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to create playlist. Error: {responseContent}");
            }

            var playlist = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseContent);
            var playlistId = playlist.id.ToString();
            List<string> songs= _WIPETUNEDbContext.PlaylistSongs.Where(ps => ps.PlaylistId == Createplaylist.PlaylistId)
            .Select(ps => ps.SongId)
            .ToList();
            var apiUrl = $"playlists/{playlistId}/tracks";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var requestBody = new
            {
                uris = songs.ConvertAll(id => $"spotify:track:{id}"),
            };

            var json1 = JsonConvert.SerializeObject(requestBody);
            var content1 = new StringContent(json1, Encoding.UTF8, "application/json");

            var response1 = await _httpClient.PostAsync(apiUrl, content1);
            try
            {
                SyncedPlaylist syncedPlaylist = new SyncedPlaylist
                {
                    AccountId = (Guid)Createplaylist.AccountId,
                    last_synced_at = DateTime.UtcNow,
                    PlaylistId = Createplaylist.PlaylistId,
                    spotify_playlist_ID = playlistId
                };
                await _WIPETUNEDbContext.SyncedPlaylists.AddAsync(syncedPlaylist);
                await _WIPETUNEDbContext.SaveChangesAsync();
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
            return response1.IsSuccessStatusCode;
        }
    }
}



