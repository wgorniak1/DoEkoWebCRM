using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko
{
    public class BusinessPartnerPerson : BusinessPartner
    {
        [Required]
        [StringLength(30)]
        [Display(Description = "", Name = "Imię", ShortName = "Imię")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(30)]
        [Display(Description = "", Name = "Nazwisko", ShortName = "Nazwisko")]
        public string LastName { get; set; }
        /// <summary>
        /// Birth date is calculated from PESEL
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Description = "", Name = "Data Urodz.", ShortName = "Data Ur.")]
        public DateTime BirthDate
        {
            get {
                if (Pesel.Length == 11)
                {
                    ushort year = UInt16.Parse(Pesel.Substring(0, 2));
                    ushort month = UInt16.Parse(Pesel.Substring(3, 2));
                    ushort day = UInt16.Parse(Pesel.Substring(5, 2));

                    if (month >= 21 && month < 41) //person born between 2000 - 2099 
                    {
                        year += 2000;
                        month -= 20;
                    }
                    else { year += 1900; } //person born between 1900 - 1999

                    return new DateTime(year: year, month: month, day: day);
                }
                else
                {
                    return DateTime.MinValue;
                }

            }
            private set { }
        }

        [Required]
        [RegularExpression("")]
        [Display(Description = "", Name = "Nr PESEL", ShortName = "PESEL")]
        public string Pesel { get; set; }
        [RegularExpression("")]
        [Display(Description = "", Name = "Nr dowodu osobistego", ShortName = "Dowód os.")]
        public string IdNumber { get; set; }
    }
}
