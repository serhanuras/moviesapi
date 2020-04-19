using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoviesAPi.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using MoviesAPi.Filters;

namespace MoviesAPi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GenresInMemoryController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;

        public GenresInMemoryController(IRepository repository, ILogger<GenresController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        [HttpGet("list")] //api/genres/list
        [HttpGet("/allgenresinmemory")] //allgenres
        //[ResponseCache(Duration=60)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ServiceFilter(typeof(MyActionFilter))]
        public async Task<ActionResult<List<Genre>>> Get()
        {

            _logger.LogDebug("Debug Task controller.");
            _logger.LogInformation("Getting all the genres");
            return await _repository.GetAllGenres();
        }


        // [HttpGet("{Id:int}/{params:alpha}")]
        // public IActionResult Get(int id)
        // {

        //     Genre genre = _repository.GetGenreById(id);

        //     return Ok(genre);
        // }

        // [HttpGet("{Id:int}")]
        // public IActionResult Get(int id, [BindRequired]string param2)
        // {

        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }

        //     Genre genre= _repository.GetGenreById(id);

        //     if (genre == null)
        //     {
        //         return NotFound();
        //     }

        //     return Ok(genre);
        // }


        [HttpGet("{Id:int}", Name = "GetGenreInMemory")]
        public IActionResult Get(int id, [FromHeader]string param2)
        {



            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
                
            }

            Genre genre = _repository.GetGenreById(id);

            if (genre == null)
            {
                _logger.LogWarning($"There is not record according to this id. Id is {id}");
                return NotFound();
            }

            return Ok(genre);
        }

        [HttpPost]
        public ActionResult Post([FromBody] Genre genre)
        {

            this._repository.AddGenre(genre);


            return new CreatedAtRouteResult("GetGenreInMemory", new { Id = genre.Id }, genre);
        }

        [HttpPut]
        public ActionResult Put([FromBody] Genre genre)
        {
            return NoContent();
        }

        [HttpDelete]
        public ActionResult Delete()
        {
            return NoContent();
        }
    }
}
