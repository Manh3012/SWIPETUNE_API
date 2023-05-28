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

    }
}
