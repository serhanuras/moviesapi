
using System.ComponentModel.DataAnnotations;
using MoviesAPi.Validations;

namespace MoviesAPi.Entities
{
    public class GenreCreationDTO
    {


        //[FirstLetterUppercase]
        [Required(ErrorMessage = "The field with name {0} is required.")]
        [StringLength(40)]
        [FirstLetterUppercase]
        public string Name { get; set; }


    }
}