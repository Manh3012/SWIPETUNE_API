﻿// <auto-generated />
using System;
using BusinessObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BusinessObject.Migrations
{
    [DbContext(typeof(SWIPETUNEDbContext))]
    partial class SWIPETUNEDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.16")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("BusinessObject.Models.Account", b =>
                {
                    b.Property<Guid>("AccountId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created_At")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DOB")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.Property<DateTime>("Verified_At")
                        .HasColumnType("datetime2");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.HasKey("AccountId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("BusinessObject.Models.AccountSubscription", b =>
                {
                    b.Property<Guid>("AccountSubId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("SubscriptionId")
                        .HasColumnType("int");

                    b.HasKey("AccountSubId");

                    b.HasIndex("AccountID");

                    b.HasIndex("SubscriptionId");

                    b.ToTable("AccountSubscriptions");
                });

            modelBuilder.Entity("BusinessObject.Models.Artist", b =>
                {
                    b.Property<Guid>("ArtistId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("artis_genres")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("artist_img_url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ArtistId");

                    b.ToTable("Artists");
                });

            modelBuilder.Entity("BusinessObject.Models.Genre", b =>
                {
                    b.Property<int>("GenreId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GenreId"), 1L, 1);

                    b.Property<string>("Description")
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid?>("SongId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("GenreId");

                    b.HasIndex("SongId");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("BusinessObject.Models.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<Guid>("AccountSubId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EventDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("EventType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AccountSubId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("BusinessObject.Models.Playlist", b =>
                {
                    b.Property<Guid>("PlaylistId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isPublic")
                        .HasColumnType("bit");

                    b.Property<string>("playlist_img_url")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PlaylistId");

                    b.HasIndex("AccountId");

                    b.ToTable("Playlists");
                });

            modelBuilder.Entity("BusinessObject.Models.PlaylistSong", b =>
                {
                    b.Property<Guid>("PlaylistId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SongId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("added_at")
                        .HasColumnType("datetime2");

                    b.Property<int>("position")
                        .HasColumnType("int");

                    b.HasIndex("PlaylistId");

                    b.HasIndex("SongId");

                    b.ToTable("PlaylistSongs");
                });

            modelBuilder.Entity("BusinessObject.Models.Song", b =>
                {
                    b.Property<Guid>("SongId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ArtisId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ArtistId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<TimeSpan?>("Duration")
                        .HasColumnType("time");

                    b.Property<int?>("GenreId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ReleaseDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Song_title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("song_img_url")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SongId");

                    b.HasIndex("ArtistId");

                    b.ToTable("Songs");
                });

            modelBuilder.Entity("BusinessObject.Models.Subscription", b =>
                {
                    b.Property<int>("SubscriptionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SubscriptionId"), 1L, 1);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("time");

                    b.Property<decimal?>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("SubscriptionName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SubscriptionId");

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("BusinessObject.Models.SyncedPlaylist", b =>
                {
                    b.Property<Guid>("SyncedPlaylistId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PlaylistId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("last_synced_at")
                        .HasColumnType("datetime2");

                    b.Property<string>("spotify_playlist_ID")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SyncedPlaylistId");

                    b.HasIndex("AccountId");

                    b.HasIndex("PlaylistId");

                    b.ToTable("SyncedPlaylists");
                });

            modelBuilder.Entity("BusinessObject.Models.AccountSubscription", b =>
                {
                    b.HasOne("BusinessObject.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Models.Subscription", "Subscription")
                        .WithMany("AccountSubscriptions")
                        .HasForeignKey("SubscriptionId");

                    b.Navigation("Account");

                    b.Navigation("Subscription");
                });

            modelBuilder.Entity("BusinessObject.Models.Genre", b =>
                {
                    b.HasOne("BusinessObject.Models.Song", null)
                        .WithMany("Genres")
                        .HasForeignKey("SongId");
                });

            modelBuilder.Entity("BusinessObject.Models.Payment", b =>
                {
                    b.HasOne("BusinessObject.Models.AccountSubscription", "AccountSubscription")
                        .WithMany()
                        .HasForeignKey("AccountSubId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AccountSubscription");
                });

            modelBuilder.Entity("BusinessObject.Models.Playlist", b =>
                {
                    b.HasOne("BusinessObject.Models.Account", "Account")
                        .WithMany("Playlists")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("BusinessObject.Models.PlaylistSong", b =>
                {
                    b.HasOne("BusinessObject.Models.Playlist", "Playlist")
                        .WithMany()
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Models.Song", "Song")
                        .WithMany()
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Playlist");

                    b.Navigation("Song");
                });

            modelBuilder.Entity("BusinessObject.Models.Song", b =>
                {
                    b.HasOne("BusinessObject.Models.Artist", null)
                        .WithMany("Songs")
                        .HasForeignKey("ArtistId");
                });

            modelBuilder.Entity("BusinessObject.Models.SyncedPlaylist", b =>
                {
                    b.HasOne("BusinessObject.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BusinessObject.Models.Playlist", "Playlist")
                        .WithMany()
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Playlist");
                });

            modelBuilder.Entity("BusinessObject.Models.Account", b =>
                {
                    b.Navigation("Playlists");
                });

            modelBuilder.Entity("BusinessObject.Models.Artist", b =>
                {
                    b.Navigation("Songs");
                });

            modelBuilder.Entity("BusinessObject.Models.Song", b =>
                {
                    b.Navigation("Genres");
                });

            modelBuilder.Entity("BusinessObject.Models.Subscription", b =>
                {
                    b.Navigation("AccountSubscriptions");
                });
#pragma warning restore 612, 618
        }
    }
}
