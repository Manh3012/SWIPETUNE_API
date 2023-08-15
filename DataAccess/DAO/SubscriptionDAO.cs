using System;
using System.Linq;
using System.Text;
using BusinessObject;
using BusinessObject.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
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
        public void AddAccountSubscription(Guid id)
        {
            string type = "FREE";
            DateTime startDate  = DateTime.Now;
        
             AccountSubscription accountSubscription = new AccountSubscription
            {

                AccountID= id,
                SubscriptionId = context.Subscriptions.SingleOrDefault(x=>x.SubscriptionName == "FREE")?.SubscriptionId,
                StartDate= startDate,
                EndDate = startDate.AddDays(30),
                
            };
            try
            {
                context.AccountSubscriptions.Add(accountSubscription);
                context.SaveChanges();
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async void UpdateToPremium(Guid id)
        {
            DateTime StartDate1 = DateTime.UtcNow;

            string type = "FREE";
            
            var account = context.AccountSubscriptions.SingleOrDefault(x => x.AccountID == id);

            try
            {
                Subscription? subscription = context.AccountSubscriptions.Where(x => x.AccountID == id).Select(x => x.Subscription).SingleOrDefault();
                if (subscription.SubscriptionName == type)
                {

                    account.AccountID = id;
                    account.SubscriptionId = context.Subscriptions.SingleOrDefault(x => x.SubscriptionName == "PREMIUM")?.SubscriptionId;
                    account.StartDate = StartDate1;
                    account.EndDate = StartDate1.AddDays(30);

                    context.Entry<AccountSubscription>(account).State = EntityState.Modified;
                    context.SaveChanges();
                }else
                {
                    throw new Exception("Cant set to premium cause it's already premium");

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string GetSubscriptionName(Guid id)
        { 
        
                var user = context.AccountSubscriptions
                .Include(x=>x.Subscription)
                .SingleOrDefault(x=>x.AccountID == id);
            string name = "";
            if(user!=null)
            {
                name= user.Subscription.SubscriptionName;
            }
            if(name==null)
            {
                return name="";
            }
            return name;
        }
    }
}
