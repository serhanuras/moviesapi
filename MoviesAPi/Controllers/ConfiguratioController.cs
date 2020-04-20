

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MoviesAPi
{

    [ApiController]
    [Route("api/configuratio")]
    public class ConfigurationController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public ConfigurationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_configuration["LastName"]);
        }

    }
}