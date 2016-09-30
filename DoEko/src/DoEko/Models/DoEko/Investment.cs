using DoEko.Models.DoEko.Addresses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoEko.Models.DoEko
{
    public enum InvestmentStatus
    {
        [Display(Name = "Inicjalny")]
        Initial,
        [Display(Name = "Zakończona")]
        Completed,
        [Display(Name = "Zamknięta")]
        Closed,
        [Display(Name = "Anulowana")]
        Cancelled
    }
    public enum InspectionStatus
    {
        [Display(Name = "Nie utworzono")]
        NotExists,
        [Display(Name = "Wersja robocza")]
        Draft,
        [Display(Name = "Do korekty")]
        InCorrection,
        [Display(Name = "Do zatwierdzenia")]
        Submitted,
        [Display(Name = "Zweryfikowana")]
        Approved,
        [Display(Name = "Ukończona")]
        Completed
    }
    [Table(nameof(Investment))]
    public class Investment : IAddressRelated
    {   
        /// <summary>
        /// 
        /// </summary>
        [Key]
        [Display(Description = "Opis", Name = "Id Inwestycji",ShortName = "Id Inwestycji")]
        public Guid InvestmentId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [Display(Description = "Opis", Name = "Umowa", ShortName = "Umowa")]
        public int ContractId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "Opis", Name = "Umowa", ShortName = "Umowa")]
        public virtual Contract Contract { get; set; }
        //public virtual Project Project { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [Display(Description = "Opis", Name = "Adres", ShortName = "Adres")]
        public int AddressId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [Display(Description = "Opis", Name = "Adres", ShortName = "Adres")]
        public virtual Address Address { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [StringLength(19,MinimumLength = 19,ErrorMessage = "Proszę podać nr formacie 112233_4.5678.123/1")]
        [RegularExpression("^[0-9]{6}(_)[0-9]{1}(.)[0-9]{4}(.)[0-9]{3}(/)[0-9]{1}$", ErrorMessage = "Proszę podać nr formacie 112233_4.5678.123/1")]
        [Display(Description = "Opis", Name = "Nr Działki", ShortName = "Nr Działki")]
        public string PlotNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [StringLength(15, MinimumLength = 15, ErrorMessage ="Proszę podać numer w formacie AA1A/12345678/1")]
        [RegularExpression("^[A-Z]{2}[0-9]{1}[A-Z]{1}(/)[0-9]{8}(/)[0-9]{1}$", ErrorMessage = "Proszę podać numer w formacie AA1A/12345678/1")]
        [Display(Description = "Opis", Name = "Nr Ks.Wieczystej", ShortName = "Nr Ks.Wieczystej")]
        public string LandRegisterNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [EnumDataType(typeof(InvestmentStatus),ErrorMessage ="Błądmodel")]
        [Display(Description = "Opis", Name = "Status", ShortName = "Status")]
        public InvestmentStatus Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [EnumDataType(typeof(InspectionStatus),ErrorMessage ="Błądmodel")]
        [Display(Description = "Opis", Name = "Status inspekcji", ShortName = "Status inspekcji")]
        public InspectionStatus InspectionStatus { get; set; }

        //inspector
        [Display(Description = "Opis", Name = "Inspektor", ShortName = "Inspektor")]
        public Guid? InspectorId { get; set; }

        //wlasciciel / inwestor
        ///// <summary>
        ///// 
        ///// </summary>
        //[Display(Description = "Opis", Name = "Ankiety", ShortName = "Ankiety")]
        //public virtual ICollection<Survey> Surveys { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "Opis", Name = "Właściciele", ShortName = "Właściciele")]
        public virtual ICollection<InvestmentOwner> InvestmentOwners { get; set; }

    }
}