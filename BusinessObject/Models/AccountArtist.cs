using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Models
{
    public class AccountArtist
    {
        [Key]
        public int AccountArtistId { get; set; }

        public Guid AccountId { get; set; }
        public virtual Account? Account { get; set; }

        public string ArtistId { get; set; }
        public virtual Artist? Artist { get; set; }
    }
}
