using DoEko.Models.DoEko.Addresses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using DoEko.Models.Validation;

namespace DoEko.Models.DoEko
{
    [Table(nameof(Company))]
    public class Company : IAddressRelated
    {
        [Key]
        public int CompanyId { get; set; }
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [StringLength(30,ErrorMessage ="Długość pola {0} nie może przekroczyć {1} znaków")]
        [Display(Description = "", Name = "Nazwa", ShortName = "Nazwa")]
        public string Name { get; set; }
        [StringLength(30, ErrorMessage = "Długość pola {0} nie może przekroczyć {1} znaków")]
        [Display(Description = "", Name = "Nazwa cd.", ShortName = "Nazwa cd.")]
        public string Name2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "", Name = "REGON", ShortName = "REGON")]
        [Regon(ErrorMessage = "Regon jest nieprawidłowy")]
        public string RegonId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "", Name = "KRS", ShortName = "KRS")]
        public string KRSId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "", Name = "NIP", ShortName = "NIP")]
        public string TaxId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataType(DataType.PhoneNumber)]
        [Display(Description = "", Name = "Nr Telefonu", ShortName = "Tel.")]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataType(DataType.EmailAddress)]
        [Display(Description = "", Name = "Adres e-mail", ShortName = "E-mail")]
        public string Email { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "", Name = "Adres", ShortName = "Adres")]
        [ForeignKey("Address")]
        public int AddressId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Address Address { get; set; }

    }
}
