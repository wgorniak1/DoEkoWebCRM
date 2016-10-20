using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko
{
    public enum SurveyType
    {
        [Display(Name = "Inst. solarna")]
        Solar,
        [Display(Name = "Inst. fotowoltaiczna")]
        Fotovoltaic,
        [Display(Name = "Pompa ciepła")]
        HeatPump
    }
    public enum SurveyStatus
    {
        [Display(Name = "Nowa")]
        New,
        [Display(Name = "W trakcie realizacji")]
        Draft,
        [Display(Name = "W trakcie weryfikacji")]
        Approval,
        [Display(Name = "Do poprawy")]
        Rejected,
        [Display(Name = "Zatwierdzona")]
        Approved
    }
    [Table(nameof(Survey))]
    public class Survey
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public Guid SurveyId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym.")]
        [EnumDataType(typeof(SurveyType), ErrorMessage = "Błąd model")]
        [Display(Description = "Rodzaj OZE", Name = "Rodzaj OZE", ShortName = "Rodzaj OZE")]
        public SurveyType Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym.")]
        [EnumDataType(typeof(SurveyStatus), ErrorMessage = "Błąd model")]
        [Display(Description = "Status inspekcji", Name = "Status inspekcji", ShortName = "Status ins.")]
        public SurveyStatus Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "Zaksięgowano wpłatę", Name = "Zapłacona", ShortName = "Zapłacona")]
        public bool IsPaid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym.")]
        [Display(Description = "Inwestycja", Name = "Inwestycja", ShortName = "Inwestycja")]
        public Guid InvestmentId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "Inwestycja", Name = "Inwestycja", ShortName = "Inwestycja")]
        public Investment Investment { get; set; }
    }
}
