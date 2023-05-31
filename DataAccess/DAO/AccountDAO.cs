using System;
using System.Linq;
using System.Text;
using BusinessObject;
using BusinessObject.Models;
using System.Threading.Tasks;
using BusinessObject.Sub_Model;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.DAO
{
    public class AccountDAO
    {
        private readonly SWIPETUNEDbContext context;

        public AccountDAO(SWIPETUNEDbContext context)
        {
            this.context = context;
        }


        public async Task<Account> GetUserByid(Guid id)
        {
            var account = await context.Accounts.SingleOrDefaultAsync(x=>x.Id== id);
            if(account == null)
            {
                throw new Exception("No account match");
            }
            return account;
        }
        public void UpdateProfile(Account account)
        {
            try
            {
                   context.Entry<Account>(account).State = EntityState.Modified;
                context.SaveChanges();
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
