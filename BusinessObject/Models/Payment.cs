using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        public Guid AccountSubId { get; set; }
        public string EventType { get; set; }
        public DateTime EventDate { get; set; }

        [ForeignKey("AccountSubId")]
        public virtual AccountSubscription? AccountSubscription { get; set; }
    }
}
