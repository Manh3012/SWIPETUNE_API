using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BusinessObject.Sub_Model
{
    public class PlaylistModel
    {
        public Guid AccountId { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public string playlist_img_url { get; set; }
        public bool isPublic { get; set; }
    }

}
