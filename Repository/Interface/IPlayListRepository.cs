using System;
using System.Linq;
using System.Text;
using BusinessObject.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Repository.Interface
{
    public interface IPlayListRepository
    {
        void CreatePlayList(Playlist playlist);
        void AddTrackToPlaylist(PlaylistSong playlistSong);
        string GetPlaylistId(string playlistname);
        Playlist GetPlaylistSong(string playlistId);
    }
}