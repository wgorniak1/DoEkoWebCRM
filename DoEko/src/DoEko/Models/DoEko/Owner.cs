using DoEko.Models.DoEko.Addresses;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoEko.Models.DoEko
{
    [Table(nameof(Owner))]
    public abstract class Owner
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public int OwnerId { get; set; }
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
