using System;
using System.Linq;
using System.Text;
using BusinessObject;
using BusinessObject.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccess.DAO
{
    public class SubscriptionDAO
    {
        private readonly SWIPETUNEDbContext context;

        public SubscriptionDAO(SWIPETUNEDbContext context)
        {
            this.context = context;
        }


        public async Task<Subscription> AddSubscription(Subscription sub)
        {
            try
            {
                await context.Subscriptions.AddAsync(sub);
                await context.SaveChangesAsync();
            }catch(Exception ex)
            {
                throw new Exception("Failed to create subscription");
            }
            return sub;
        }

        public  bool DeleteSubscription(int id)
        {
            try
            {
                var exist =  context.Subscriptions.SingleOrDefault(x => x.SubscriptionId == id);
                if (exist != null)
                {
                    context.Subscriptions.Remove(exist);
                 context.SaveChanges();

                }
                else
                {
                    throw new Exception("No subscription to delete");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create subscription");
            }
            return true;
        }
        public async Task<Subscription> UpdateSubscription(Subscription sub)
        {
            try
            {
                context.Entry<Subscription>(sub).State=EntityState.Modified;
                await context.SaveChangesAsync();
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return sub;
        }
        public async Task<List<Subscription>> GetSubscriptions()
        {
            return await context.Subscriptions.ToListAsync();
        }
    }
}
