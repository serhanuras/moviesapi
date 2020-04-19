

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MoviesAPi.Entities{

    public class Person {

        public int Id { get; set; }

        //[FirstLetterUppercase]
        [Required(ErrorMessage = "The field with name {0} is required.")]
        [StringLength(40)]
        public string Name { get; set; }

        public string Biografy { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Picture { get; set;}

         public List<MoviesActors> MoviesActors { get; set; }
    }
}