using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Sub_Model;
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

        public string? artist_img_url { get; set; }

        public virtual ICollection<Song> Songs { get; set; }
        public virtual ICollection<AccountArtist>? AccountArtists { get; set; }



    }

    public static class ArtistConverter
    {
        public static Artist ConvertFromArtistSpotify(ArtistSpotify artistSpotify)
        {
            if (artistSpotify.id != null && artistSpotify.name != null && artistSpotify.genres.Any() && artistSpotify.images.Any())
            {
                var artist = new Artist
                {
                    ArtistId = artistSpotify.id,
                    Name = artistSpotify.name,
                    artis_genres = string.Join(", ", artistSpotify.genres),
                    artist_img_url = artistSpotify.images.FirstOrDefault()?.url
                };

                return artist;
            }

            return null;
        }
    }

}
