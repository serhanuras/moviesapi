using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using MoviesAPi.Entities;

namespace MoviesAPi.Services
{
    public class InMemoryRepository: IRepository
    {

        private List<Genre> _genres;
        public InMemoryRepository()
        {
            _genres = new List<Genre>
            {
                new Genre
                {
                    Id=1,
                    Name="Action"
                },
                  new Genre
                {
                    Id=2,
                    Name="Comedy"
                }
            };
        }


        public async Task<List<Genre>> GetAllGenres()
        {
            await Task.Delay(3000);
            return _genres;
        }

        public Genre GetGenreById(int id)
        {
            return _genres.FindLast(x => x.Id == id);
        }

        public void AddGenre(Genre genre){

            genre.Id = _genres.Max(x=>x.Id) + 1;            
            _genres.Add(genre);
        }
    }
}
