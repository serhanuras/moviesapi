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
using MoviesAPi.Validations;
using System.IO;
using MoviesAPi.Services;
using Microsoft.AspNetCore.JsonPatch;
using MoviesApi.DTOs;
using System.Linq;
using MoviesAPi.Helpers;

namespace MoviesAPi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PeopleController : ControllerBase
    {

        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;

        private readonly IMapper _mapper;

        private readonly IFileStorageService _fileStorageService;
        
        private readonly string containerName = "people";

        public PeopleController(ILogger<GenresController> logger, ApplicationDbContext dbContext, IMapper mapper, IFileStorageService fileStorageService)
        {

            _logger = logger;
            _dbContext = dbContext;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }

        [HttpGet]
        [HttpGet("list")] //api/genres/list
        [HttpGet("/allpeople")] //allpeople
        [ServiceFilter(typeof(MyActionFilter))]
        public async Task<ActionResult<List<PersonDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
             var queryable = _dbContext.People.AsQueryable();
            await HttpContext.InsertPaginationParametersInResponse(queryable, paginationDTO.RecordsPerPage);
            var people = await queryable.Paginate(paginationDTO).ToListAsync();
            return _mapper.Map<List<PersonDTO>>(people);
           
        }

        [HttpGet("{Id:int}", Name = "GetPerson")]
        public async Task<ActionResult<PersonDTO>> Get(int id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            var person = await _dbContext.People.FirstOrDefaultAsync(x => x.Id == id);

            var personDTO = _mapper.Map<PersonDTO>(person);

            if (person == null)
            {
                _logger.LogWarning($"There is not record according to this id. Id is {id}");
                return NotFound();
            }

            return Ok(personDTO);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PersonCreationDTO personCreationDTO)
        {

            var person = _mapper.Map<Person>(personCreationDTO);

            if (personCreationDTO.Picture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await personCreationDTO.Picture.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(personCreationDTO.Picture.FileName);
                    person.Picture =
                        await _fileStorageService.SaveFile(content, extension, containerName,
                                                            personCreationDTO.Picture.ContentType);
                }
            }

            _dbContext.Add(person);
            await _dbContext.SaveChangesAsync();
            var personDTO = _mapper.Map<PersonDTO>(person);
            return new CreatedAtRouteResult("getPerson", new { id = person.Id }, personDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] PersonCreationDTO personCreationDTO)
        {
             var person = await _dbContext.People.FirstOrDefaultAsync(x => x.Id == id);

            if (person == null) { return NotFound(); }

            person = _mapper.Map(personCreationDTO, person);

            if (personCreationDTO.Picture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await personCreationDTO.Picture.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(personCreationDTO.Picture.FileName);
                    person.Picture =
                        await _fileStorageService.EditFile(content, extension, containerName,
                                                            person.Picture,
                                                            personCreationDTO.Picture.ContentType);
                }
            }

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PersonPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var entityFromDB = await _dbContext.People.FirstOrDefaultAsync(x => x.Id == id);

            if (entityFromDB == null)
            {
                return NotFound();
            }

            var entityDTO = _mapper.Map<PersonPatchDTO>(entityFromDB);

            patchDocument.ApplyTo(entityDTO, ModelState);

            var isValid = TryValidateModel(entityDTO);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(entityDTO, entityFromDB);

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
        
        
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await _dbContext.People.AnyAsync(x=>x.Id == id);

            if(!exists)
                return NotFound();

            _dbContext.Remove(new Person() { Id =id});

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
