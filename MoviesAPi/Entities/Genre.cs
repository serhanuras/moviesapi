using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MoviesAPi.Validations;

namespace MoviesAPi.Entities
{
    public class Genre : IValidatableObject
    {

        public int Id { get; set; }

        //[FirstLetterUppercase]
        [Required(ErrorMessage = "The field with name {0} is required.")]
        [StringLength(10)]
        public string Name { get; set; }

        [Range(18, 120)]
        public int Age { get; set; }

        [CreditCard]
        public string CreditCard { get; set; }

        [Url]
        public string Url { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                var fistLetter = Name[0].ToString();

                if (fistLetter != fistLetter.ToUpper())
                {
                    yield return new ValidationResult("First letter should be uppercase.", new string[] { nameof(Name) });
                }
            }
        }
    }
}
