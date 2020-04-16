using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MoviesAPi.Entities;

namespace MoviesAPi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenresController: ControllerBase
    {
        private readonly IRepository _repository;

        public GenresController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [HttpGet("list")] //api/genres/list
        [HttpGet("/allgenres")] //allgenres
        public async Task<ActionResult<List<Genre>>> Get()
        {
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


        [HttpGet("{Id:int}")]
        public IActionResult Get2(int id, [FromHeader]string param2)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Genre genre = _repository.GetGenreById(id);

            if (genre == null)
            {
                return NotFound();
            }

            return Ok(genre);
        }

        [HttpPost]
        public ActionResult Post([FromBody] Genre genre)
        {

            return NoContent();
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
