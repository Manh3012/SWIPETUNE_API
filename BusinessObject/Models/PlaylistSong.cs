using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public class PlaylistSong
    {
        public string PlaylistId { get; set; }
        public virtual Playlist Playlist { get; set; }

        public string SongId { get; set; }
        public virtual Song Song { get; set; }
        public int position { get; set; }
        public DateTime added_at { get; set; }
    }
}
