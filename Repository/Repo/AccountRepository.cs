using System;
using System.Linq;
using System.Text;
using DataAccess.DAO;
using Repository.Interface;
using BusinessObject.Models;
using System.Threading.Tasks;
using BusinessObject.Sub_Model;
using System.Collections.Generic;

namespace Repository.Repo
{
    public class AccountRepository : IAccountRepository
    {

        public Guid RegisterAccount(RegisterAccountModel account)=>AccountDAO.RegisterAccount(account);
        public Account Login(string email, string password)=> AccountDAO.Login(email, password);

        public void UpdateAccount(Guid AccountId,UpdateAccountModel account)=>AccountDAO.UpdateAccount(AccountId,account);

        public void DeleteAccount(Guid AccountId)=>AccountDAO.DeleteAccount(AccountId);
        public void AddToken(Guid AccountId, string token) => AccountDAO.AddToken(AccountId, token);
        public void LogOut(Guid AccountId)=>AccountDAO.LogOut(AccountId);
    }
}
