using DoEko.Models.DoEko.Addresses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko
{
    public class BusinessPartner
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
        public string TaxId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Description = "", Name = "Nr Telefonu", ShortName = "Tel.")]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [DataType(DataType.EmailAddress)]
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
