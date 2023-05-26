using System;
using System.Linq;
using System.Text;
using BusinessObject;
using BusinessObject.Models;
using System.Threading.Tasks;
using BusinessObject.Sub_Model;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace DataAccess.DAO
{
    public class GenreDAO
    {
        private readonly SWIPETUNEDbContext context;

        public GenreDAO(SWIPETUNEDbContext context)
        {
            this.context = context;
        }


        public  void AddGenre(Genre genre)
        {
           
            
                context.Genres.Add(genre);
                context.SaveChanges();
            



        }
        public  List<Genre> GetGenres()
        {
            var list = new List<Genre>();
            try
            {
                
                    list=context.Genres.ToList();
                
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return list;
        }
        public  Genre GetGenreById(int id)
        {
            Genre genre = new Genre();
            
                genre = context.Genres.FirstOrDefault(x => x.GenreId == id);
                if (genre != null)
                {
                    return genre;
                }
                return null;
            
        }

    }
}
