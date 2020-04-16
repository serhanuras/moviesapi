using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesAPi.Entities
{
    public interface IRepository
    {
        Task<List<Genre>> GetAllGenres();
        Genre GetGenreById(int id);
    }
}
