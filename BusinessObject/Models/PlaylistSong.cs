using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Models
{
    public class PlaylistSong
    {
        [Key]
        [JsonIgnore]
        public int PlaylistSongId { get; set; }
        [JsonIgnore]

        public string PlaylistId { get; set; }
        [JsonIgnore]

        public virtual Playlist Playlist { get; set; }
        public string SongId { get; set; }  
        public virtual Song Song { get; set; }
        public int position { get; set; }
        public DateTime added_at { get; set; }
    }
}
