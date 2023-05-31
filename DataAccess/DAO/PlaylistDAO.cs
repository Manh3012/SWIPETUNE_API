using System;
using System.Linq;
using System.Text;
using BusinessObject;
using BusinessObject.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

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
            }else
            {
                throw new Exception("Playlist existed");
            }
        }
    }
}
