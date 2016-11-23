using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko
{
    public class OwnerPerson : Owner
    {
        [Required]
        [StringLength(30)]
        [Display(Description = "", Name = "Imię", ShortName = "Imię")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(30)]
        [Display(Description = "", Name = "Nazwisko", ShortName = "Nazwisko")]
        public string LastName { get; set; }
        //[Required]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        //[Display(Description = "", Name = "Data Urodz.", ShortName = "Data Ur.")]
        //public DateTime BirthDate { get; set; }
        //[Required]
        [RegularExpression("^[0-9]{11}$")]
        [Display(Description = "", Name = "Nr PESEL", ShortName = "PESEL")]
        public string Pesel { get; set; }
        [RegularExpression("")]
        [Display(Description = "", Name = "Nr dowodu osobistego", ShortName = "Dowód os.")]
        public string IdNumber { get; set; }
    }
}
