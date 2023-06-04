using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    public class Genre
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GenreId { get; set; }

        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(60)]
        public string? Description { get; set; }
        public virtual ICollection<AccountGenre>? AccountGenres { get; set; }
    }
}
