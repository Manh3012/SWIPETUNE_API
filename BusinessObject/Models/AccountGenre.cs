using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Models
{
    public class AccountGenre
    {
        [Key]
        public int AccountGenreId { get; set; }

        public Guid AccountId { get; set; }
        public virtual Account? Account { get; set; }

        public int GenreId { get; set; }
        public virtual Genre? Genre { get; set; }
    }
}
