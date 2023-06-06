using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BusinessObject.Sub_Model
{
    public class AccountArtistModel
    {
        public int? AccountArtistId { get; set; }
        public Guid AccountId { get; set; }
        public string ArtistId { get; set; }
    }
}
