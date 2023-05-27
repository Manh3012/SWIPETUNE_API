using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Models
{
    public class Artist
    {
        [Key]
        public string ArtistId { get; set; }

        public string Name { get; set; }

        public string? artis_genres { get; set; }

        public string artist_img_url { get; set; }

        public virtual ICollection<Song> Songs { get; set; }

    }
}
