﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MoviesAPi.Validations;

namespace MoviesAPi.Entities
{
    public class Genre
    {


        public int Id { get; set; }

        //[FirstLetterUppercase]
        [Required(ErrorMessage = "The field with name {0} is required.")]
        [StringLength(40)]
        public string Name { get; set; }


        public List<MoviesGenres> MoviesGenres { get; set; }
    }
}
