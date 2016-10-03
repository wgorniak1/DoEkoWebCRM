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
        [NIP(ErrorMessage ="Nr Nip jest nieprawidłowy")]
        public string TaxId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage ="Pole {0} jest obowiązkowe")]
        [DataType(DataType.PhoneNumber)]
        [Display(Description = "", Name = "Nr Telefonu", ShortName = "Tel.")]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage ="Pole {0} jest obowiązkowe")]
        [DataType(DataType.EmailAddress,ErrorMessage ="Proszę podać prawidłowy adres e-mail")]
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
        [Display(Description = "", Name = "Inwestycje", ShortName = "Inwestycje")]
        public virtual ICollection<InvestmentOwner> InvestmentOwners { get; set; }
    }
}
