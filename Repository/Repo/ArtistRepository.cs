using System;
using System.Linq;
using System.Text;
using BusinessObject;
using DataAccess.DAO;
using Repository.Interface;
using BusinessObject.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Repository.Repo
{
    public class ArtistRepository : IArtistRepository
    {
        private readonly ArtistDAO artistDAO;

        public ArtistRepository(SWIPETUNEDbContext context)
        {
            artistDAO= new ArtistDAO(context);
        }
        public void AddArtist(List<Artist> artist)=>artistDAO.AddArtist(artist);
        public async Task<Artist> GetArtistById(string artistId)=>await artistDAO.GetArtistById(artistId);
    }
}
