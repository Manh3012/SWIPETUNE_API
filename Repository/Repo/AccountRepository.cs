using System;
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

        public AccountRepository()
        {
            accountDAO = new AccountDAO();
        }

        public Guid RegisterAccount(RegisterAccountModel account)=> accountDAO.RegisterAccount(account);
        public Account Login(string email, string password)=> accountDAO.Login(email, password);

        public void UpdateAccount(Guid AccountId,UpdateAccountModel account)=>accountDAO.UpdateAccount(AccountId,account);

        public void DeleteAccount(Guid AccountId)=>accountDAO.DeleteAccount(AccountId);
        public void AddToken(Guid AccountId, string token) => accountDAO.AddToken(AccountId, token);
        public void LogOut(Guid AccountId)=>accountDAO.LogOut(AccountId);
    }
}
