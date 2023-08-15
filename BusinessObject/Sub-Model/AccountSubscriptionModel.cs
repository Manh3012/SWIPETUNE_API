using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BusinessObject.Sub_Model
{
    public class AccountSubscriptionModel
    {
        public Guid AccountID { get; set; }
        public int? SubscriptionId { get; set; }
    }
}
