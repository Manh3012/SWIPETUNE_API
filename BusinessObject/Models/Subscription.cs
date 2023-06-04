using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Models
{
    public class Subscription
    {
        [Key]
        public int SubscriptionId { get; set; }
        public string SubscriptionName { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? Duration { get; set; }

        public virtual ICollection<AccountSubscription>? AccountSubscriptions { get; set; }
            
    }
}

 