using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Survey
{


    [ComplexType]
    public class SurveyResultCalculations
    {
        [Key,ForeignKey("Survey")]
        public Guid SurveyId { get; set; }
        public virtual Survey Survey { get; set; }

        [Display(Name = "Ostateczny dobór paneli")]
        public double FinalPVConfig { get; set; }
        //
        [Display(Name = "Ostateczny dobór kolektorów")]
        public string FinalSOLConfig { get; set; }

        [Display(Name = "Ostateczny dobór mocy OZE")]
        public double FinalRSEPower { get; set; }
        [Display(Name = "Sprawność OZE")]
        public double RSEEfficiency { get; set; }
        [Display(Name = "Czas pracy instalacji OZE")]
        public double RSEWorkingTime { get; set; }
        [Display(Name = "Roczna produkcja OZE")]
        public double RSEYearlyProduction { get; set; }
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
        [Display(Name = "Roczne zużycie en. elektr.")]
        public double RSEEnYearlyConsumption { get; set; }
        [Display(Name = "Kwota Netto")]
        public double RSENetPrice { get; set; }
        [Display(Name = "VAT")]
        public double RSETax { get; set; }
        [Display(Name = "Kwota brutto")]
        public double RSEGrossPrice { get; set; }
        [Display(Name = "ΔRÓWNOWAŻNE(PYŁY, NOX, SOX) CO2")]
        public double CO2DustEquivValue { get; set; }
        [Display(Name = "ΔRÓWNOWAŻNE(PYŁY, NOX, SOX) CO2")]
        public double CO2DustEquivPercent { get; set; }
        [Display(Name = "ΔRÓWNOWAŻNE CO2")]
        public double CO2EquivValue { get; set; }
        [Display(Name = "ΔCO2")]
        public double CO2Value { get; set; }
        [Display(Name = "ΔCO2")]
        public double CO2Percent { get; set; }
        [Display(Name = "ΔPM10")]
        public double PM10Value { get; set; }
        [Display(Name = "ΔPM10")]
        public double PM10Percent { get; set; }
        [Display(Name = "ΔPM2,5")]
        public double PM25Value { get; set; }
        [Display(Name = "ΔPM2,5")]
        public double PM25Percent { get; set; }
        [Display(Name = "ΔBENZO(A)PIREN")]
        public double BenzoPirenValue { get; set; }
        [Display(Name = "ΔBENZO(A)PIREN")]
        public double BenzoPirenPercent { get; set; }
        //

    }
}
