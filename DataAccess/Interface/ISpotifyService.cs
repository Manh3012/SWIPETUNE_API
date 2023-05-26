using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Sub_Model;
using System.Collections.Generic;

namespace DataAccess.Interface
{
    public interface ISpotifyService
    {
        Task<Track> GetSongs(string trackId, string accessToken);
        Task<ArtistSpotify> SearchArtists(string searchQuery, string accessToken);

    }
}
