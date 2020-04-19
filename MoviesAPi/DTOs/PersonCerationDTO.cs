
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using MoviesAPi.Validations;

namespace MoviesAPi.DTOs
{

    public class PersonCreationDTO
    {

        //[FirstLetterUppercase]
        [Required(ErrorMessage = "The field with name {0} is required.")]
        [StringLength(120)]
        public string Name { get; set; }

        public string Biografy { get; set; }

        public DateTime DateOfBirth { get; set; }


        [FileSizeValidatorAttribute(4)]
        [ContentTypeValidator(ContentTypeGroup.Image)]
        public IFormFile Picture { get; set; }
    }
}