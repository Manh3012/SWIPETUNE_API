using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Models
{
    public class Playlist
    {
        [Key]
        public string PlaylistId { get; set; }
        public Guid? AccountId { get; set; }

        [Required] public string? Name { get; set; }
        public DateTime? Created { get; set; }
        public string? playlist_img_url { get; set; }

        public bool isPublic { get; set; }

        public virtual Account? Account { get; set; }
        public ICollection<PlaylistSong>? PlaylistSongs { get; set; }
    }
}
