using DoEko.Models.DoEko.Addresses;
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
    public class Investment
    {   
        /// <summary>
        /// 
        /// </summary>
        [Key]
        [Display(Description = "Opis", Name = "Id Inwestycji",ShortName = "Id Inwestycji")]
        public int InvestmentId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [Display(Description = "Opis", Name = "Umowa", ShortName = "Umowa")]
        public virtual Contract Contract { get; set; }
        //public virtual Project Project { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [Display(Description = "Opis", Name = "Adres", ShortName = "Adres")]
        public virtual Address Address { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [StringLength(11,MinimumLength = 11)]
        [RegularExpression("")]
        [Display(Description = "Opis", Name = "Nr Działki", ShortName = "Nr Działki")]
        public string PlotNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [StringLength(11, MinimumLength = 11)]
        [RegularExpression("")]
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
        //wlasciciel / inwestor
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "Opis", Name = "Ankiety", ShortName = "Ankiety")]
        public virtual ICollection<Survey> Surveys { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "Opis", Name = "Właściciele", ShortName = "Właściciele")]
        public virtual ICollection<InvestmentOwner> InvestmentOwners { get; set; }

    }
}