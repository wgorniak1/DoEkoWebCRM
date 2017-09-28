using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko
{
    public enum CompanySize
    {
        micro = 1,
        small = 2,
        medium,
        large
    }
    public class BusinessPartnerEntity : BusinessPartner
    {
        [Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        [StringLength(30)]
        [Display(Description = "", Name = "Nazwa", ShortName = "Nazwa")]
        public string Name {
            get { return PartnerName1; }
            set {
                PartnerName1 = value;
            }
        }
        [StringLength(30,ErrorMessage = "Długość pola {0} nie może przekroczyć {1} znaków")]
        [Display(Description = "", Name = "Nazwa cd.", ShortName = "Nazwa cd.")]
        public string Name2 {
            get { return PartnerName2; }
            set {
                PartnerName2 = value;
            } }

        [Display(Description = "", Name = "Wielkość", ShortName = "Wielkość")]
        public CompanySize CompanySize { get; set; }
    }
}
