using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BusinessObject.Sub_Model
{
    public class TopTracksResponse
    {
        [JsonPropertyName("tracks")]
        public List<Track> Tracks { get; set; }
    }
}
