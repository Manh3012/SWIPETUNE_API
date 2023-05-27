using System;
using System.Linq;
using System.Text;
using DataAccess.DAO;
using BusinessObject;
using Repository.Interface;
using BusinessObject.Models;
using System.Threading.Tasks;
using BusinessObject.Sub_Model;
using System.Collections.Generic;

namespace Repository.Repo
{
    public class GenreRepository : IGenreRepository
    {
        private readonly GenreDAO genreDAO;

        public GenreRepository(SWIPETUNEDbContext context)
        {
            genreDAO = new GenreDAO(context);
        }
        public void AddGenre(Genre genre) => genreDAO.AddGenre(genre);
        public List<Genre> GetGenres() => genreDAO.GetGenres();
        public Genre GetGenreById(int id) => genreDAO.GetGenreById(id);
    }
}
