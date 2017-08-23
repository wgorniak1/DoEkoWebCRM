using DoEko.Models.DoEko.Addresses;
using DoEko.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko
{
    public class BusinessPartner : IAddressRelated
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public Guid BusinessPartnerId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "", Name = "NIP", ShortName = "NIP")]
        [NIP(ErrorMessage = "Nr NIP jest nieprawidłowy")]
        [RegularExpression("^[0-9]{3}(-)[0-9]{3}(-)[0-9]{2}(-)[0-9]{2}$", ErrorMessage = "Proszę wprowadzić NIP w formacie 000-000-00-00")]
        public string TaxId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [Display(Description = "", Name = "Nr Telefonu", ShortName = "Tel.")]
        [RegularExpression(pattern: @"^(\+48(\s|)(12|13|14|15|16|17|18|22|23|24|25|29|32|33|34|41|42|43|44|46|48|52|54|55|56|58|59|61|62|63|65|67|68|71|74|75|76|77|81|82|83|84|85|86|87|89|91|94|95)(\s|)[0-9]{3}(\s|)[0-9]{2}(\s|)[0-9]{2})$|^(\+48(\s|)[0-9]{3}(\s|)[0-9]{3}(\s|)[0-9]{3})$",
            ErrorMessage = "Proszę podać nr telefonu w formacie '+48 00 111 22 33' lub '+48 000 111 222'")]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //[Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Proszę podać prawidłowy adres e-mail")]
        [Display(Description = "", Name = "Adres e-mail", ShortName = "E-mail")]
        public string Email { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Description = "", Name = "Adres", ShortName = "Adres")]
        [ForeignKey("Address")]
        public int AddressId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual Address Address { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "", Name = "Właściciel", ShortName = "Właściciel")]
        public virtual ICollection<InvestmentOwner> InvestmentOwners { get; set; }

        public virtual string PartnerName1 { get; set; }
        public virtual string PartnerName2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "", Name = "Zgoda na przetw. danych", ShortName = "Zgoda na przetw. d.")]
        public bool DataProcessingConfirmation { get; set; }

        [NotMapped]
        public string FullName { get { return PartnerName1 + ' ' + PartnerName2; } private set { } }
    }
}
