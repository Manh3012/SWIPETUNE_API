using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace BusinessObject.Models
{
    public class Song
    {
        [Key]
        public string SongId { get; set; }

        public string? ArtistId { get; set; }
        [JsonIgnore]
        public virtual Artist? Artist { get; set; }
        [JsonIgnore]
        public int? GenreId { get; set; }
        public string? Song_title { get; set; }
        public TimeSpan? Duration { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? song_img_url { get; set; }

        [JsonIgnore]
        public virtual Genre? Genres { get; set; }
        public ICollection<PlaylistSong>? PlaylistSongs { get; set; }

    }
}
