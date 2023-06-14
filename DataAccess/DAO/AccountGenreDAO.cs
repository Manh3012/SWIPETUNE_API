using System;
using System.Linq;
using System.Text;
using BusinessObject;
using BusinessObject.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class AccountGenreDAO
    {
        private readonly SWIPETUNEDbContext context;

        public AccountGenreDAO(SWIPETUNEDbContext context)
        {
            this.context = context;
        }


        public async Task<List<Genre>> GetGenreFromAccount(Guid accountId)
        {
            var list = new List<Genre>();
            try
            {
                list= await context.AccountGenre.Where(x=>x.AccountId==accountId).Select(x=>x.Genre).ToListAsync();
            }catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return list;
        }
    }
}
