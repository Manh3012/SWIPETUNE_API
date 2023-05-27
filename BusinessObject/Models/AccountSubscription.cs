using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Models
{
    public class AccountSubscription
    {
        [Key]
        public Guid AccountSubId { get; set; }

        public Guid AccountID { get; set; }
        public int? SubscriptionId { get; set; }
        public virtual Subscription Subscription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public virtual Account Account { get; set; }
    }

}
