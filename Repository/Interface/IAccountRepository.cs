using System;
using System.Linq;
using System.Text;
using BusinessObject.Models;
using System.Threading.Tasks;
using BusinessObject.Sub_Model;
using System.Collections.Generic;

namespace Repository.Interface
{
    public interface IAccountRepository
    {
        Guid RegisterAccount(RegisterAccountModel account);
        Account Login(string email, string password);
        void UpdateAccount(Guid AccountId, UpdateAccountModel account);
        void DeleteAccount(Guid AccountId);
        void AddToken(Guid AccountId, string token);
        void LogOut(Guid AccountId);
    }
}
