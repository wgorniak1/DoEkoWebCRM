using DoEko.Models.Validation;
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
        [Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        [StringLength(30)]
        [Display(Description = "", Name = "Imię", ShortName = "Imię")]
        public string FirstName 
        {
            get { return PartnerName1; }
            set
            {
                PartnerName1 = value;
            }
        }
        [Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        [StringLength(30)]
        [Display(Description = "", Name = "Nazwisko", ShortName = "Nazwisko")]
        public string LastName 
        {
            get { return PartnerName2; }
            set
            {
                PartnerName2 = value;
            }
        }
        /// <summary>
        /// Birth date is calculated from PESEL
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Description = "", Name = "Data Urodz.", ShortName = "Data Ur.")]
        public DateTime BirthDate
        {
            get {
                if (Pesel == null)
                {
                    return DateTime.MinValue;
                }
                if (Pesel.Length == 11)
                {
                    ushort year = UInt16.Parse(Pesel.Substring(0, 2));
                    ushort month = UInt16.Parse(Pesel.Substring(2, 2));
                    ushort day = UInt16.Parse(Pesel.Substring(4, 2));

                    if (month >= 21 && month < 41) //person born between 2000 - 2099 
                    {
                        year += 2000;
                        month -= 20;
                    }
                    else { year += 1900; } //person born between 1900 - 1999
                    try
                    {
                        return new DateTime(year: year, month: month, day: day);
                    }
                    catch (Exception)
                    {
                        return DateTime.MinValue;
                    }
                }
                else
                {
                    return DateTime.MinValue;
                }

            }
            private set { }
        }

        [Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        [RegularExpression("^[0-9]{11}$",ErrorMessage ="Nr PESEL jest nieprawidłowy")]
        [Display(Description = "", Name = "Nr PESEL", ShortName = "PESEL")]
        [PESEL(ErrorMessage ="Nr PESEL jest nieprawidłowy")]
        public string Pesel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [RegularExpression("^[A-Z]{3}( )[0-9]{6}$",ErrorMessage ="Proszę podać nr w formacie 'ABC 123456'")]
        [Display(Description = "", Name = "Nr dowodu osobistego", ShortName = "Dowód os.")]
        public string IdNumber { get; set; }
    }
}
