using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Models
{
    public class SyncedPlaylist
    {
        [Key]
        public Guid SyncedPlaylistId { get; set; }
        public Guid AccountId { get; set; }
        public virtual Account Account { get; set; }
        public Guid PlaylistId { get; set; }
        public virtual Playlist Playlist { get; set; }
        public string spotify_playlist_ID { get; set; }
        public DateTime last_synced_at { get; set; }


    }
}
