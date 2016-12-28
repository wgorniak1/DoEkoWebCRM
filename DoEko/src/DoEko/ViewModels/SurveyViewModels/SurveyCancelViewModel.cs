using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoEko.Models.DoEko.Survey;
using System.ComponentModel.DataAnnotations;

namespace DoEko.ViewModels.SurveyViewModels
{
    public class SurveyCancelViewModel
    {
        [Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        public Guid SurveyId { get; set; }
        [Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        [Display(Name = "Powód anulowania")]
        public SurveyCancelType CancelType { get; set; }
        [Display(Name ="Dodatkowe uwagi")]
        [MaxLength(4096,ErrorMessage ="Maksymalna liczba znaków wynosi {0}")]
        [Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        public string CancelComments { get; set; }
        [Display(Name ="Rodzaj energii")]
        public SurveyType? ReplaceWithType { get; set; }
        [Display(Name = "Typ Źródła")]
        public SurveyRSETypeEnergy? ReplaceWithRSEEN { get; set; }
        public SurveyRSETypeHotWater? ReplaceWithRSEHW { get; set; }
        public SurveyRSETypeCentralHeating? ReplaceWithRSECH { get; set; }
    }
}
