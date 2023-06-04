using System;
using System.Linq;
using System.Text;
using BusinessObject;
using BusinessObject.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class PlaylistDAO
    {

        private readonly SWIPETUNEDbContext context;

        public PlaylistDAO(SWIPETUNEDbContext context)
        {
            this.context = context;
        }


        public void CreatePlayList(Playlist playlist)
        {
            var existed = context.Playlists.FirstOrDefault(x => x.PlaylistId == playlist.PlaylistId);
            if(existed == null)
            {

                context.Playlists.Add(playlist);
                context.SaveChanges();
            }else
            {
                throw new Exception("Playlist existed");
            }
        }
        public void AddTrackToPlaylist(PlaylistSong playlistSong)
        {
            try
            {
                if (context.PlaylistSongs.AsNoTracking().Any(ps => ps.SongId == playlistSong.SongId && ps.PlaylistId == playlistSong.PlaylistId))
                {
                    throw new Exception("Already has this song");
                }

                context.PlaylistSongs.Add(playlistSong);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}", ex);
            }
        }
        public string GetPlaylistId(string playlistname)
        {
            var playlist = context.Playlists.FirstOrDefault(x => x.PlaylistId == playlistname);
            if(playlist == null)
            {
                throw new Exception("No playlist has that Id");
            }
            return playlist.PlaylistId;
        }
        public Playlist GetPlaylistSong(string playlistId)
        {
            var playlist = context.Playlists
                .Include(x=>x.PlaylistSongs)
                .ThenInclude(x=>x.Song)
                .FirstOrDefault(x=>x.PlaylistId == playlistId);
            return playlist;
        }
    }
}
