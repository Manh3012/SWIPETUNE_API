using System;
using System.Linq;
using System.Text;
using DataAccess.DAO;
using BusinessObject;
using Repository.Interface;
using BusinessObject.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Repository.Repo
{
    public class AccountGenreRepository :IAccountGenreRepository
    {

        private readonly AccountGenreDAO accountgenreDAO;

        public AccountGenreRepository(SWIPETUNEDbContext context)
        {
            accountgenreDAO = new AccountGenreDAO(context);
        }
        public async Task<List<Genre>> GetGenreFromAccount(Guid accountId)=>await accountgenreDAO.GetGenreFromAccount(accountId);


    }
}
