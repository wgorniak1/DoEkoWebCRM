using DoEko.Models.DoEko.Addresses;
using DoEko.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.ClusterImport
{
    public enum InstallationType
    {
        Prosument = 1,
        PVFarm = 2
    }

    [ClusterInvestment]
    public class ClusterInvestment
    {
        [Key]
        public Guid ClustInvestmentId { get; set; }

        
        [Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        [Display(Description = "", Name = "Gmina", ShortName = "")]
        public int ContractId {get;set;}
        
        [ForeignKey("ContractId")]
        public Contract Contract { get; set; }

        [Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        [Display(Description = "", Name = "Rodzaj uczestnika", ShortName = "Rodzaj ucz.")]
        public BusinessPartnerType MemberType { get; set; }

        [Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        [Display(Description = "", Name = "Imię", ShortName = "Imię")]
        [StringLength(30)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        [Display(Description = "", Name = "Nazwisko", ShortName = "Nazwisko")]
        [StringLength(30)]
        public string Name2 { get; set; }

        [Display(Description = "", Name = "Wielkość", ShortName = "Wielkość")]
        public CompanySize CompanySize { get; set; }

        [NIP]
        [Display(Description = "", Name = "NIP", ShortName = "NIP")]
        public string TaxId { get; set; }

        [Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        [Display(Description = "", Name = "Nr telefonu", ShortName = "Nr tel.")]
        [Phone()]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        [Display(Description = "", Name = "Adres e-mail", ShortName = "E-mail")]
        [EmailAddress]
        public string Email { get; set; }
        #region address
        //[Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        //[Display(Description = "", Name = "Województwo", ShortName = "Woj.")]
        //public int StateId { get; set; }

        //[Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        //[Display(Description = "", Name = "Powiat", ShortName = "Powiat")]
        //public int DistrictId { get; set; }

        //[Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        //[Display(Description = "", Name = "Gmina", ShortName = "Gmina")]
        //public int CommuneId { get; set; }

        //[Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        //[Display(Description = "", Name = "Typ Gminy", ShortName = "Typ Gminy")]
        //public CommuneType CommuneType { get; set; }

        //[Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        //[StringLength(6, ErrorMessage = "Proszę wprowadzić {0} w formacie 00-000", MinimumLength = 6)]
        //[DataType(DataType.PostalCode)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:##-###}")]
        //[Display(Description = "", Name = "Kod Poczty", ShortName = "Kod P.", Prompt = "00-000")]
        //[RegularExpression(pattern: @"^([0-9]{2}-[0-9]{3})$", ErrorMessage = "Proszę wprowadzić {0} w formacie 00-000")]
        //public string PostalCode { get; set; }

        //[Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        //[StringLength(50, ErrorMessage = "", MinimumLength = 3)]
        //[Display(Description = "", Name = "Miejscowość", ShortName = "Miejscowość")]
        //public string City { get; set; }

        //[StringLength(50, ErrorMessage = "Proszę podać prawidłową nazwę ulicy", MinimumLength = 1)]
        //[Display(Description = "Ulica", Name = "Ulica", ShortName = "Ulica")]
        //public string Street { get; set; }

        //[Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        //[StringLength(5, ErrorMessage = "Długość pola {0} nie może przekroczyć {1} znaków", MinimumLength = 1)]
        //[Display(Description = "Opis", Name = "Nr Budynku", ShortName = "Nr Bud.")]
        //public string BuildingNo { get; set; }

        //[StringLength(5, ErrorMessage = "Proszę", MinimumLength = 1)]
        //[Display(Description = "", Name = "Nr Mieszkania", ShortName = "Nr Mieszk.")]
        //public string ApartmentNo { get; set; }


        //public State State { get; set; }

        //public District District { get; set; }

        //public Commune Commune { get; set; }
        #endregion

        [Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        [Display(Description = "", Name = "Adres", ShortName = "Adres")]
        [ForeignKey("Address")]
        public int AddressId { get; set; }

        public Address Address { get; set; }

        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        public bool NewInstallation { get; set; }

        [Range((double)0, (double)100.0, ErrorMessage = "Prawidłowa wartość mieści się w przedziale {0} - {1}")]
        public double PvPower { get; set; }

        [Range((double)0,(double)1000.0,ErrorMessage ="Prawidłowa wartość mieści się w przedziale {0} - {1}")]
        public double PvYearlyProduction { get; set; }

        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        public InstallationType Type { get; set; }

        [MaxLength(1000,ErrorMessage = "Maksymalna długość pola wynosi {0}")]
        public string Description { get; set; }

        [Range((double)0, (double)1000.0, ErrorMessage = "Prawidłowa wartość mieści się w przedziale {0} - {1}")]
        public double EnYearlyConsumption {get;set;}

    }
}
