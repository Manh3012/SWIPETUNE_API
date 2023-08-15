using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BusinessObject.Sub_Model
{
    public class ArtistModel
    {
        public string ArtistId { get; set; }

        public string Name { get; set; }

        public string? artis_genres { get; set; }

        public string? artist_img_url { get; set; }
    }
}
