using System;
using System.Linq;
using System.Text;
using DataAccess.DAO;
using BusinessObject.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Repository.Interface
{
    public interface ISubscriptionRepository
    {
        Task<Subscription> AddSubscription(Subscription sub);
        bool DeleteSubscription(int id);
        Task<Subscription> UpdateSubscription(Subscription sub);
        Task<List<Subscription>> GetSubscriptions();
    }
}
