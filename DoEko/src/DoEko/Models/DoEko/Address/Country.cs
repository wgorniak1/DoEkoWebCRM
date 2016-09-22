using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Addresses
{
    [Table(nameof(Country))]
    public class Country
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public int CountryId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [StringLength(maximumLength: 2,ErrorMessage ="{0} składa się z dokładnie {1} znaków",MinimumLength = 2)]
        [Display(Description = "", Name = "Kod Kraju", ShortName = "Kod")]
        public string Key { get; set; }
        [Required]
        [StringLength(50,ErrorMessage = "Pole {0} nie może być dłuższe niż {1} ani krótsze niż {2}")]
        [Display(Description = "", Name = "Nazwa", ShortName = "Nazwa")]
        public string Name { get; set; }
    }
}
