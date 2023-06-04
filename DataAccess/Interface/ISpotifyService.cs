using System;
using System.Linq;
using System.Text;
using BusinessObject.Models;
using System.Threading.Tasks;
using BusinessObject.Sub_Model;
using System.Collections.Generic;

namespace DataAccess.Interface
{
    public interface ISpotifyService
    {
        Task<Track> GetSongs(string trackId, string accessToken);
        Task<ArtistSpotify> SearchArtists(string searchQuery, string accessToken);
        Task<string> GetAccessToken();
        Task<List<string>> GetArtistIds(string query, string accessToken);
        Task<List<Song>> GetTopTracks(string artistId, string accessToken);
        Task<string> CreatePlaylist(Playlist Createplaylist, string accessToken);
        Task<string> GetUserProfile(string accessToken);
        Task<bool> AddTrackToPlaylist(List<string> trackIds, string playlistId, int position, string accessToken);
        Task<Playlist> GetPlaylist(string playlistId, string accessToken);
        Task<List<Playlist>> FetchUserPlaylists(string accessToken);
        Task<List<Song>> GetRecommendation(string artisId, List<string> genres, string trackId, string accessToken);
        Task<bool> SyncPlaylist(Playlist Createplaylist, string accessToken);
    }
}
