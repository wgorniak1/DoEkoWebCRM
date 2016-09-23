using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko
{
    public class BusinessPartnerEntity : BusinessPartner
    {
        [Required]
        [StringLength(30)]
        [Display(Description = "", Name = "Nazwa", ShortName = "Nazwa")]
        public string Name { get; set; }
        [StringLength(30)]
        [Display(Description = "", Name = "Nazwa cd.", ShortName = "Nazwa cd.")]
        public string Name2 { get; set; }
    }
}
