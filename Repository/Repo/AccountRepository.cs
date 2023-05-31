﻿using System;
using System.Linq;
using System.Text;
using DataAccess.DAO;
using BusinessObject;
using Repository.Interface;
using BusinessObject.Models;
using System.Threading.Tasks;
using BusinessObject.Sub_Model;
using System.Collections.Generic;

namespace Repository.Repo
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AccountDAO accountDAO;

        public AccountRepository(SWIPETUNEDbContext context)
        {
            accountDAO = new AccountDAO(context);
        }

        public async Task<Account> GetUserById(Guid id)=>await accountDAO.GetUserByid(id);
        public void UpdateProfile(Account account)=> accountDAO.UpdateProfile(account);
    }
}
