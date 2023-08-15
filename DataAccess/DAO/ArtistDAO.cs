using System;
using System.Linq;
using System.Text;
using BusinessObject;
using BusinessObject.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class ArtistDAO
    {
        private readonly SWIPETUNEDbContext context;

        public ArtistDAO(SWIPETUNEDbContext context)
        {
            this.context = context;

        }
        public void AddArtist(List<Artist> artists)
        {
            foreach (var artist in artists)
            {
                var existingArtist = context.Artists.FirstOrDefault(a => a.ArtistId == artist.ArtistId);
                if (existingArtist == null)
                {
                    context.Artists.Add(artist);
                }
            }

            context.SaveChanges();
        }
        public async Task<Artist> GetArtistById(string artistId)
        {
            var artist = await context.Artists.FirstOrDefaultAsync(x=>x.ArtistId == artistId);
            if(artist != null)
            {
                return artist;
            }
            return null;
        }
    }
}
