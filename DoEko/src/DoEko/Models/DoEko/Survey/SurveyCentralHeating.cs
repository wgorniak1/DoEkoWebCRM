using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Survey
{
    [Table(nameof(SurveyCentralHeating))]
    public class SurveyCentralHeating : Survey
    {
        public SurveyRSETypeCentralHeating RSEType { get; set; }
        
        [Display(Name = "Wsp. strat ciepła")]
        public double HeatLossFactor { get; set; }

            

        [Display(Name = "Wsp. zapotrzebowania na energię")]
        public double CHRequiredEnFactor { get; set; }
        [Display(Name = "Szczytowe zapotrzebowanie na energię")]
        public double CHMaxRequiredEn { get; set; }
        [Display(Name = "Zapotrzebowanie na energię dla CO.")]
        public double CHRequiredEn { get; set; }
        [Display(Name = "Roczne zapotrzebowanie na energię - stan pierwotny")]
        public double HWRequiredEnYearly { get; set; }

        
        
        
        

        [Display(Name = "Czas pracy instalacji OZE dla CO")]
        public double CHRSEWorkingTime { get; set; }
        [Display(Name = "Czas pracy instalacji OZE dla CWU")]
        public double HWRSEWorkingTime
        {
            get { return this.RSEWorkingTime; }
            set { this.RSEWorkingTime = value; }
        }
        [Display(Name = "Roczna produkcja OZE dla CO")]
        public double CHRSEYearlyProduction { get; set; }
        [Display(Name = "Roczna produkcja OZE dla CWU")]
        public double HWRSEYearlyProduction
        {
            get { return this.RSEYearlyProduction; }
            set { this.RSEYearlyProduction = value; }
        }

        private new double RSEEfficiency { get; set; }
    }
}
