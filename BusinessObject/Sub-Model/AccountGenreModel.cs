using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BusinessObject.Sub_Model
{
    public class AccountGenreModel
    {
        public int? AccountGenreId { get; set; }

        public Guid AccountId { get; set; }
        public int GenreId { get; set; }

    }
}
