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
        Task<Account> GetUserById(Guid id);
        void UpdateProfile(Account account);
        void AddAccountGenre(AccountGenreModel model);
        Task<AccountGenre> UpdateAccountGenre(AccountGenreModel model);
    }
}
