using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoviesAPi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using MoviesAPi.Filters;
using MoviesAPi.PostgreSqlProvider;
using MoviesAPi.DTOs;
using AutoMapper;

namespace MoviesAPi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GenresController : ControllerBase
    {

        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;

        private readonly IMapper _mapper;

        public GenresController(ILogger<GenresController> logger, ApplicationDbContext dbContext, IMapper mapper)
        {

            _logger = logger;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet]
        [HttpGet("list")] //api/genres/list
        [HttpGet("/allgenres")] //allgenres
        //[ResponseCache(Duration=60)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ServiceFilter(typeof(MyActionFilter))]
        public async Task<ActionResult<List<GenreDTO>>> Get()
        {
            var genres = await _dbContext.Genres.AsNoTracking().ToListAsync();

            var genreDTOs = _mapper.Map<List<GenreDTO>>(genres);

            return genreDTOs;

            // List<GenreDTO> genreDTOs = new List<GenreDTO>();
            // foreach (var genre in genres)
            // {
            //     var genreDTO = new GenreDTO(){
            //         Id = genre.Id,
            //         Name = genre.Name
            //     };

            //     genreDTOs.Add(genreDTO);
                
            // }

            // return genreDTOs;
        }

        [HttpGet("{Id:int}", Name = "GetGenre")]
        public async Task<ActionResult<GenreDTO>> Get(int id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            var genre = await _dbContext.Genres.FirstOrDefaultAsync(x => x.Id == id);

            var genreDTO = _mapper.Map<GenreDTO>(genre);

            if (genre == null)
            {
                _logger.LogWarning($"There is not record according to this id. Id is {id}");
                return NotFound();
            }

            return Ok(genreDTO);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GenreCreationDTO genreCreationDTO)
        {

            var genre = _mapper.Map<Genre>(genreCreationDTO);
            _dbContext.Add(genre);

            await _dbContext.SaveChangesAsync();

            var genreDTO = _mapper.Map<GenreDTO>(genre);

            return new CreatedAtRouteResult("GetGenre", new { Id = genreDTO.Id }, genreDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] GenreCreationDTO genreCreationDTO)
        {
            var genre = _mapper.Map<Genre>(genreCreationDTO);
            genre.Id = id;

            _dbContext.Entry(genre).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await _dbContext.Genres.AnyAsync(x=>x.Id == id);

            if(!exists)
                return NotFound();

            _dbContext.Remove(new Genre() { Id =id});

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
