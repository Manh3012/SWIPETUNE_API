using System;
using System.Linq;
using System.Text;
using BusinessObject;
using DataAccess.DAO;
using Repository.Interface;
using BusinessObject.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Repository.Repo
{
    public class SubscriptionRepository:ISubscriptionRepository
    {
        private readonly SubscriptionDAO subscriptionDAO;

        public SubscriptionRepository(SWIPETUNEDbContext context)
        {
            subscriptionDAO = new SubscriptionDAO(context);
        }
        public async Task<Subscription> AddSubscription(Subscription sub)=> await subscriptionDAO.AddSubscription(sub);
        public  bool DeleteSubscription(int id) => subscriptionDAO.DeleteSubscription(id);
        public async Task<Subscription> UpdateSubscription(Subscription sub)=>await subscriptionDAO.UpdateSubscription(sub);
        public async Task<List<Subscription>> GetSubscriptions()=>await subscriptionDAO.GetSubscriptions();
    }
}
