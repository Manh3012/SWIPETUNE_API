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
        public static List<Account> GetAccounts() { 
            var list = new List<Account>();
            try
            {
                using (var context= new SWIPETUNEDbContext())
                {
                    list = context.Accounts.ToList();

                }
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return list;
        }
        public static Guid RegisterAccount(RegisterAccountModel account)
        {
            var createAccount =new Account();
            try
            {
                using (var context = new SWIPETUNEDbContext())
                {
                    createAccount.AccountId = Guid.NewGuid();
                    createAccount.Email= account.Email;
                    createAccount.Password= account.Password;
                    // Set default values for some properties
                    createAccount.Verified_At = DateTime.UtcNow;
                    createAccount.isDeleted = false;
                    createAccount.Created_At = DateTime.UtcNow;
                    createAccount.Status = true;
                    byte[] salt = GenerateSalt();

                    // Hash the password with the salt
                    byte[] hashedPassword = HashPassword(createAccount.Password, salt);

                    // Convert byte arrays to base64 strings for storage
                    createAccount.Password = Convert.ToBase64String(hashedPassword);
                    createAccount.SaltPassword = Convert.ToBase64String(salt);

                    if (context.Accounts.SingleOrDefault(x=>x.Email == account.Email) != null)
                    {
                        throw new Exception("Email exists");
                    }
                    context.Accounts.Add(createAccount);
                    context.SaveChanges();
                }
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return createAccount.AccountId;
        }
        public static void AddToken(Guid AccountId,string token)
        {
            var account = GetAccountById(AccountId);
            if (account != null) { 
            using (var context=new SWIPETUNEDbContext())
                {
                    account.accessToken = token;
                    context.Entry<Account>(account).State=EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }
        public static Account Login(string email, string password)
        {
            // Find the account with the given email
            using (var context = new SWIPETUNEDbContext())
            {
                Account? account = context.Accounts.FirstOrDefault(a => a.Email == email);

                // Check if account exists and the password matches
                if (account != null)
                {
                    // Verify the password
                    bool isPasswordValid = VerifyPassword(password, account.Password, account.SaltPassword);

                    if (isPasswordValid)
                    {
                        return account; // Return the account if login is successful
                    }
                }
            }

            return null; // Return null if login fails
        }
        public static Account GetAccountById(Guid AccountId)
        {
            Account? existingAccount = new Account();
            using(var context= new SWIPETUNEDbContext())
            {
                existingAccount = context.Accounts.SingleOrDefault(x => x.AccountId == AccountId);
                if(existingAccount != null)
                {
                    return existingAccount;
                }
                return null;
            }
        }
        public static void LogOut(Guid AccountId)
        {
            var account = GetAccountById(AccountId);
            if(account != null)
            {
                using(var context = new SWIPETUNEDbContext())
                {
                    account.accessToken = null;
                    context.Entry<Account>(account).State=EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }

        public static void UpdateAccount(Guid AccountId,UpdateAccountModel account)
        {
            var exist = GetAccountById(AccountId);
            if (exist == null) {
                throw new Exception("No account to be updated");
            }
            using (var context= new SWIPETUNEDbContext())
            {
                exist.FullName = account.FullName;
                exist.Gender = account.Gender;
                exist.DOB = account.DOB;
                exist.PhoneNumber = account.PhoneNumber;
                exist.Address = account.Address;

                context.Entry<Account>(exist).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        private static byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }
        public static void DeleteAccount(Guid AccountId)
        {
            var account = GetAccountById(AccountId);
            if (account != null)
            {
                try
                {
                    using(var context=new SWIPETUNEDbContext())
                    {
                        context.Accounts.Remove(account);
                        context.SaveChanges();
                    }
                }catch(Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        private static byte[] HashPassword(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                return pbkdf2.GetBytes(20); // 20 is the length of the hashed password
            }
        }
        private static bool VerifyPassword(string password, string hashedPassword, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] hashedPasswordBytes = Convert.FromBase64String(hashedPassword);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000))
            {
                byte[] computedHash = pbkdf2.GetBytes(20);

                // Compare the computed hash with the stored hashed password
                if (computedHash.Length != hashedPasswordBytes.Length)
                {
                    return false;
                }

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != hashedPasswordBytes[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
