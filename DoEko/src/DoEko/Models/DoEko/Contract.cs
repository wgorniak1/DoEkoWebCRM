using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko
{
    public enum ContractType
    {
        [Display(Name = "Umowa z gminą")]
        WithCommune,
        [Display(Name = "Umowa z osobą prywatną")]
        WithPerson,
        [Display(Name = "Inne")]
        Other
    }
    public enum ContractStatus
    {
        [Display(Name = "Nowa (Wersja robocza)")]
        Draft,
        [Display(Name = "W trakcie realizacji")]
        InProgress,
        [Display(Name = "Opłacona w całości")]
        Paid,
        [Display(Name = "Zweryfikowana")]
        Verified,
        [Display(Name = "Zrealizowana")]
        Completed
    }
    [Table(nameof(Contract))]
    public class Contract
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        [Required(ErrorMessage = "{0} jest polem obowiązkowym.")]
        [Display(Description = "Opis", Name = "Id Umowy", ShortName = "Id Umowy")]
        public int ContractId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym.")]
        [EnumDataType(typeof(ContractType),ErrorMessage ="Error enum")]
        [Display(Description = "Opis", Name = "Rodzaj umowy", ShortName = "Rodzaj umowy")]
        public ContractType Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym.")]
        [StringLength(20,MinimumLength = 5,ErrorMessage = "Proszę wprowadzić {0} w formacie 12345/A/1/2016")]
        [RegularExpression(@"^[0-9]{1,5}(/)[A-Z]{1}(/)(1|2|3|4|5|6|7|8|9|10|11|12)(/)(2015|2016|2017|2018|2019|2020)$", 
            ErrorMessage = "Proszę wprowadzić {0} w formacie 12345/X/M/RRRR. Gdzie X - typ umowy, M/RRRR data podpisania umowy")]
        [Display(Description = "12345 - kolejny numer umowy. X - rodzaj umowy, M/RRRR - miesiąć/rok podpisania umowy", 
            Name = "Numer", ShortName = "Numer", Prompt ="12345/G/1/2016")]
        public string Number { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true, HtmlEncode = true)]
        [Display(Description = "Opis", Name = "Data Podpisania", ShortName = "Data Podpisania")]
        public DateTime ContractDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Description = "Opis", Name = "Termin Realizacji", ShortName = "Termin Realizacji")]
        public DateTime? FullfilmentDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym.")]
        [EnumDataType(typeof(ContractStatus), ErrorMessage = "Error enum")]
        [Display(Description = "Opis", Name = "Status", ShortName = "Status")]
        public ContractStatus Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym.")]
        [StringLength(maximumLength: 50, ErrorMessage = "Pole {0} powinno mieć przynajmniej {2} i nie więcej niż {1} znaków", MinimumLength = 3)]
        [Display(Description = "Opis", Name = "Krótki Opis", ShortName = "Krótki Opis")]
        public string ShortDescription { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym.")]
        [Display(Description = "Opis", Name = "Projekt", ShortName = "Projekt")]
        public int ProjectId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        
        [Display(Description = "Opis", Name = "Projekt", ShortName = "Projekt")]
        public virtual Project Project { get; set; }

        [Display(Description = "", Name = "Dostawca", ShortName = "Dostawca")]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual ICollection<Investment> Investments { get; set; }
        //inwestycje
        //wplaty
        //znacznik wpłaty
        //id wpłaty
        //status_wykonania
        //data zakonczenia prac(do inspekcji)
    }
}
