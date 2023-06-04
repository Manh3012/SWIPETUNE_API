using System;
using System.Linq;
using System.Text;
using BusinessObject;
using DataAccess.DAO;
using Repository.Interface;
using BusinessObject.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Repository.Repo
{
    public class PlayListRepository : IPlayListRepository
    {
        private readonly PlaylistDAO playlistDAO;

        public PlayListRepository(SWIPETUNEDbContext context)
        {
            playlistDAO = new PlaylistDAO(context);
        }

        public void CreatePlayList(Playlist playlist)=> playlistDAO.CreatePlayList(playlist);
        public void AddTrackToPlaylist(PlaylistSong playlistSong)=>playlistDAO.AddTrackToPlaylist(playlistSong);
        public string GetPlaylistId(string playlistname)=> playlistDAO.GetPlaylistId(playlistname);
        public Playlist GetPlaylistSong(string playlistId)=>playlistDAO.GetPlaylistSong(playlistId);
    }
}
