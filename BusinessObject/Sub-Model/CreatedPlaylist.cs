using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BusinessObject.Sub_Model
{
    public class CreatedPlaylist
    {
        public string name { get; set; }
        public string description { get; set; }
        [JsonPropertyName("public")]
        public bool _public { get; set; }
    }

}
