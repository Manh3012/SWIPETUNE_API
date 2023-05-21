using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Models
{
    public class Song
    {
        [Key]
        public Guid SongId { get; set; }
        public Guid ArtisId { get; set; }
        public int? GenreId { get; set; }
        public string? Song_title { get; set; }
        public TimeSpan? Duration { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? song_img_url { get; set; }

        public virtual ICollection<Genre> Genres { get; set; }

    }
}
