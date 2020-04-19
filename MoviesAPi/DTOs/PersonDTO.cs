
using System;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPi.DTOs{

    public class PersonDTO {

        public int Id { get; set; }

        //[FirstLetterUppercase]
        [Required(ErrorMessage = "The field with name {0} is required.")]
        [StringLength(120)]
        public string Name { get; set; }

        public string Biografy { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Picture { get; set;}
    }
}