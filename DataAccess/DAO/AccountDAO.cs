using System;
using System.Linq;
using System.Text;
using BusinessObject;
using BusinessObject.Models;
using System.Threading.Tasks;
using BusinessObject.Sub_Model;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.AspNetCore.SignalR;
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
            var account = await context.Accounts
                .Include(x => x.AccountArtists)
                .ThenInclude(x=>x.Artist)
                .Include(x=>x.AccountSubscriptions)
                .ThenInclude(x=>x.Subscription)
                .Include(X=>X.Playlists)
                .ThenInclude(X=>X.PlaylistSongs)
                .Include(x=>x.AccountGenres)
                .ThenInclude(x=>x.Genre)
                .Include(x=>x.SyncedPlaylists)

                .SingleOrDefaultAsync(x=>x.Id== id);
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
        public void AddAccountGenre(AccountGenreModel model)
        {
            var accountGenre= new AccountGenre();
            accountGenre.AccountId = model.AccountId;
            accountGenre.GenreId= model.GenreId;

            try
            {
                context.AccountGenre.Add(accountGenre);
                context.SaveChanges();

            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void AddAccountArtist(AccountArtistModel model)
        {
            var accountArtist = new AccountArtist();
            accountArtist.AccountId = model.AccountId;
            accountArtist.ArtistId = model.ArtistId;

            try
            {
                context.AccountArtist.Add(accountArtist);
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<AccountGenre> UpdateAccountGenre(AccountGenreModel model)
        {
            var exist = context.AccountGenre.SingleOrDefault(x => x.AccountGenreId == model.AccountGenreId);
            exist.AccountId = model.AccountId;
            exist.GenreId = model.GenreId;
            try
            {
                context.Entry<AccountGenre>(exist).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return exist;
        }
        public async Task<AccountArtist> UpdateAccountArtist(AccountArtistModel model)
        {
            var exist = context.AccountArtist.SingleOrDefault(x => x.AccountArtistId == model.AccountArtistId);
            exist.AccountId = model.AccountId;
            exist.ArtistId = model.ArtistId;
            try
            {
                context.Entry<AccountArtist>(exist).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return exist;
        }
    }
}
