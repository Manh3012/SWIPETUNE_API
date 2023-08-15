using System;
using System.Linq;
using System.Text;
using BusinessObject.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Repository.Interface
{
    public interface IArtistRepository
    {
        void AddArtist(List<Artist> artist);
        Task<Artist> GetArtistById(string artistId);
    }
}
