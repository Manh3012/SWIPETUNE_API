using System;
using System.Linq;
using System.Text;
using BusinessObject.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BusinessObject.Sub_Model
{
    public class SongModel
    {
        public string SongId { get; set; }
        public string? ArtistId { get; set; }
        public string? Song_title { get; set; }
        public TimeSpan? Duration { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? song_img_url { get; set; }
        public virtual ArtistModel? Artist { get; set; }

    }
}
