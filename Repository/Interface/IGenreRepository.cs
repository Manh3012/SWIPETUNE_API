using System;
using System.Linq;
using System.Text;
using DataAccess.DAO;
using BusinessObject.Models;
using System.Threading.Tasks;
using BusinessObject.Sub_Model;
using System.Collections.Generic;

namespace Repository.Interface
{
    public interface IGenreRepository { 


        void AddGenre(Genre genre);
        List<Genre> GetGenres();
         Genre GetGenreById(int id);
    }
}
