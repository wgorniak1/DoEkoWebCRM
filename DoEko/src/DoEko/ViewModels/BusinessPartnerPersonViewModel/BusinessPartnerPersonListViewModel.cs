using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.ViewModels.BusinessPartnerPersonViewModel
{
    public class BusinessPartnerPersonListViewModel
    {
        //[Key]
        //public Guid BusinessPartnerId { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //[Display(Description = "", Name = "NIP", ShortName = "NIP")]
        //[NIP(ErrorMessage = "Nr NIP jest nieprawidłowy")]
        //[RegularExpression("^[0-9]{3}(-)[0-9]{3}(-)[0-9]{2}(-)[0-9]{2}$", ErrorMessage = "Proszę wprowadzić NIP w formacie 000-000-00-00")]
        //public string TaxId { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //[Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        //[Display(Description = "", Name = "Nr Telefonu", ShortName = "Tel.")]
        //[RegularExpression(pattern: @"^(\+48(\s|)(12|13|14|15|16|17|18|22|23|24|25|29|32|33|34|41|42|43|44|46|48|52|54|55|56|58|59|61|62|63|65|67|68|71|74|75|76|77|81|82|83|84|85|86|87|89|91|94|95)(\s|)[0-9]{3}(\s|)[0-9]{2}(\s|)[0-9]{2})$|^(\+48(\s|)[0-9]{3}(\s|)[0-9]{3}(\s|)[0-9]{3})$",
        //    ErrorMessage = "Proszę podać nr telefonu w formacie '+48 00 111 22 33' lub '+48 000 111 222'")]
        //public string PhoneNumber { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        ////[Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        //[DataType(DataType.EmailAddress)]
        //[EmailAddress(ErrorMessage = "Proszę podać prawidłowy adres e-mail")]
        //[Display(Description = "", Name = "Adres e-mail", ShortName = "E-mail")]
        //public string Email { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //[Required]
        //[Display(Description = "", Name = "Adres", ShortName = "Adres")]
        //[ForeignKey("Address")]
        //public int AddressId { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public virtual Address Address { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //[Display(Description = "", Name = "Właściciel", ShortName = "Właściciel")]
        //public virtual ICollection<InvestmentOwner> InvestmentOwners { get; set; }

        //public string PartnerName1 { get; set; }
        //public string PartnerName2 { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //[Display(Description = "", Name = "Zgoda na przetw. danych", ShortName = "Zgoda na przetw. d.")]
        //public bool DataProcessingConfirmation { get; set; }

        //[Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        //[StringLength(30)]
        //[Display(Description = "", Name = "Imię", ShortName = "Imię")]
        //public string FirstName
        //{
        //    get { return PartnerName1; }
        //    set
        //    {
        //        PartnerName1 = value;
        //    }
        //}
        //[Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        //[StringLength(30)]
        //[Display(Description = "", Name = "Nazwisko", ShortName = "Nazwisko")]
        //public string LastName
        //{
        //    get { return PartnerName2; }
        //    set
        //    {
        //        PartnerName2 = value;
        //    }
        //}
        ///// <summary>
        ///// Birth date is calculated from PESEL
        ///// </summary>
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        //[Display(Description = "", Name = "Data Urodz.", ShortName = "Data Ur.")]
        //public DateTime BirthDate
        //{
        //    get
        //    {
        //        if (Pesel == null)
        //        {
        //            return DateTime.MinValue;
        //        }
        //        if (Pesel.Length == 11)
        //        {
        //            ushort year = UInt16.Parse(Pesel.Substring(0, 2));
        //            ushort month = UInt16.Parse(Pesel.Substring(2, 2));
        //            ushort day = UInt16.Parse(Pesel.Substring(4, 2));

        //            if (month >= 21 && month < 41) //person born between 2000 - 2099 
        //            {
        //                year += 2000;
        //                month -= 20;
        //            }
        //            else { year += 1900; } //person born between 1900 - 1999
        //            try
        //            {
        //                return new DateTime(year: year, month: month, day: day);
        //            }
        //            catch (Exception)
        //            {
        //                return DateTime.MinValue;
        //            }
        //        }
        //        else
        //        {
        //            return DateTime.MinValue;
        //        }

        //    }
        //    private set { }
        //}

        ////[Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        //[RegularExpression("^[0-9]{11}$", ErrorMessage = "Nr PESEL jest nieprawidłowy")]
        //[Display(Description = "", Name = "Nr PESEL", ShortName = "PESEL")]
        //[PESEL(ErrorMessage = "Nr PESEL jest nieprawidłowy")]
        //public string Pesel { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //[RegularExpression("^[A-Z]{3}( )[0-9]{6}$", ErrorMessage = "Proszę podać nr w formacie 'ABC 123456'")]
        //[Display(Description = "", Name = "Nr dowodu osobistego", ShortName = "Dowód os.")]
        //public string IdNumber { get; set; }

    }
}
