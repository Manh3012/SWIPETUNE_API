using System;
using System.Linq;
using System.Text;
using BusinessObject.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BusinessObject
{
    public class SWIPETUNEDbContext :DbContext
    {
        public SWIPETUNEDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange : true);
            IConfiguration configuration   = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("SWIPE_TUNEDB"));
                
        }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<AccountSubscription> AccountSubscriptions { get; set; }

        public virtual DbSet<Artist> Artists { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Playlist> Playlists { get; set; }
        public virtual DbSet<PlaylistSong> PlaylistSongs { get; set; }
        public virtual DbSet<Song> Songs { get; set; }
        public virtual DbSet<Subscription> Subscriptions { get; set; }
        public virtual DbSet<SyncedPlaylist> SyncedPlaylists { get; set; }

        protected override void OnModelCreating (ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlaylistSong>().HasNoKey();
            modelBuilder.Entity<SyncedPlaylist>()
           .HasOne(sp => sp.Account)
           .WithMany()
           .HasForeignKey(sp => sp.AccountId)
           .OnDelete(DeleteBehavior.NoAction); // Specify ON DELETE NO ACTION

            modelBuilder.Entity<SyncedPlaylist>()
                .HasOne(sp => sp.Playlist)
                .WithMany()
                .HasForeignKey(sp => sp.PlaylistId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

}
