using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoEko.Models.DoEko.Survey;
using System.ComponentModel.DataAnnotations;

namespace DoEko.ViewModels.SurveyViewModels
{
    public class SurveyCreateViewModel
    {
        [Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        public Guid InvestmentId { get; set; }
        [Display(Name ="Rodzaj energii")]
        [Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        public SurveyType SurveyType { get; set; }
        [Display(Name = "Typ Źródła")]
        public SurveyRSETypeEnergy? RSETypeEN { get; set; }
        public SurveyRSETypeHotWater? RSETypeHW { get; set; }
        public SurveyRSETypeCentralHeating? RSETypeCH { get; set; }
        public int RSEType {
            get
            {
                return (int?)RSETypeEN ?? 
                       (int?)RSETypeCH ?? 
                       (int?)RSETypeHW ?? 0;
            }
            private set { }
        }
    }
}
