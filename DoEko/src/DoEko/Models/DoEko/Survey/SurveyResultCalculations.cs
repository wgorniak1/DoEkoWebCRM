using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Survey
{


    [ComplexType]
    public class SurveyResultCalculations
    {
        public SurveyResultCalculations()
        {

        }
        public SurveyResultCalculations(SurveyType type, int rseType, DataRow data)
        {
            this.CO2DustEquivValue = double.Parse(string.IsNullOrEmpty(data.Field<string>(44))? "0" : data.Field<string>(44));
            this.CO2DustEquivPercent = double.Parse(string.IsNullOrEmpty(data.Field<string>(45)) ? "0" : data.Field<string>(45));
            this.CO2EquivValue = double.Parse(string.IsNullOrEmpty(data.Field<string>(46)) ? "0" : data.Field<string>(46));
            this.CO2Value = double.Parse(string.IsNullOrEmpty(data.Field<string>(47)) ? "0" : data.Field<string>(47));
            this.CO2Percent = double.Parse(string.IsNullOrEmpty(data.Field<string>(48)) ? "0" : data.Field<string>(48));
            this.PM10Value = double.Parse(string.IsNullOrEmpty(data.Field<string>(49)) ? "0" : data.Field<string>(49));
            this.PM10Percent = double.Parse(string.IsNullOrEmpty(data.Field<string>(50)) ? "0" : data.Field<string>(50));
            this.PM25Value = double.Parse(string.IsNullOrEmpty(data.Field<string>(51)) ? "0" : data.Field<string>(51));
            this.PM25Percent = double.Parse(string.IsNullOrEmpty(data.Field<string>(52)) ? "0" : data.Field<string>(52));
            this.BenzoPirenValue = double.Parse(string.IsNullOrEmpty(data.Field<string>(53)) ? "0" : data.Field<string>(53));
            this.BenzoPirenPercent = double.Parse(string.IsNullOrEmpty(data.Field<string>(54)) ? "0" : data.Field<string>(54));

            switch (type)
            {
                case SurveyType.CentralHeating:

                    this.HeatLossFactor = double.Parse(string.IsNullOrEmpty(data.Field<string>(30)) ? "0" : data.Field<string>(30));
                    this.CHRequiredEnFactor = double.Parse(string.IsNullOrEmpty(data.Field<string>(31)) ? "0" : data.Field<string>(31));
                    this.CHMaxRequiredEn = double.Parse(string.IsNullOrEmpty(data.Field<string>(32)) ? "0" : data.Field<string>(32));
                    this.CHRequiredEn = double.Parse(string.IsNullOrEmpty(data.Field<string>(33)) ? "0" : data.Field<string>(33));
                    this.HWRequiredEnYearly = double.Parse(string.IsNullOrEmpty(data.Field<string>(34)) ? "0" : data.Field<string>(34));
                    this.FinalRSEPower = double.Parse(string.IsNullOrEmpty(data.Field<string>(35)) ? "0" : data.Field<string>(35));
                    this.CHRSEWorkingTime = double.Parse(string.IsNullOrEmpty(data.Field<string>(36)) ? "0" : data.Field<string>(36));
                    this.CHRSEYearlyProduction = double.Parse(string.IsNullOrEmpty(data.Field<string>(37)) ? "0" : data.Field<string>(37));
                    this.HWRSEWorkingTime = double.Parse(string.IsNullOrEmpty(data.Field<string>(38)) ? "0" : data.Field<string>(38));
                    this.HWRSEYearlyProduction = double.Parse(string.IsNullOrEmpty(data.Field<string>(39)) ? "0" : data.Field<string>(39));
                    this.RSEEnYearlyConsumption = double.Parse(string.IsNullOrEmpty(data.Field<string>(40)) ? "0" : data.Field<string>(40));
                    this.RSENetPrice = double.Parse(string.IsNullOrEmpty(data.Field<string>(41)) ? "0" : data.Field<string>(41));
                    this.RSETax = double.Parse(string.IsNullOrEmpty(data.Field<string>(42)) ? "0" : data.Field<string>(42));
                    this.RSEGrossPrice = double.Parse(string.IsNullOrEmpty(data.Field<string>(43)) ? "0" : data.Field<string>(43));

                    break;
                case SurveyType.HotWater:

                    if (rseType == (int)SurveyRSETypeHotWater.HeatPump)
                    {
                        this.HWRequiredEnYearly = double.Parse(string.IsNullOrEmpty(data.Field<string>(13)) ? "0" : data.Field<string>(13));
                        this.FinalRSEPower = double.Parse(string.IsNullOrEmpty(data.Field<string>(14)) ? "0" : data.Field<string>(14));
                        this.RSEYearlyProduction = double.Parse(string.IsNullOrEmpty(data.Field<string>(15)) ? "0" : data.Field<string>(15));
                        this.RSEWorkingTime = double.Parse(string.IsNullOrEmpty(data.Field<string>(16)) ? "0" : data.Field<string>(16));
                        this.RSEEnYearlyConsumption = double.Parse(string.IsNullOrEmpty(data.Field<string>(17)) ? "0" : data.Field<string>(17));
                        this.RSENetPrice = double.Parse(string.IsNullOrEmpty(data.Field<string>(18)) ? "0" : data.Field<string>(18));
                        this.RSETax = double.Parse(string.IsNullOrEmpty(data.Field<string>(19)) ? "0" : data.Field<string>(19));
                        this.RSEGrossPrice = double.Parse(string.IsNullOrEmpty(data.Field<string>(20)) ? "0" : data.Field<string>(20));

                    }
                    else
                    {
                        this.FinalSOLConfig = data.Field<string>(3);
                        this.HWRequiredEnYearly = double.Parse(string.IsNullOrEmpty(data.Field<string>(4)) ? "0" : data.Field<string>(4));
                        this.FinalRSEPower = double.Parse(string.IsNullOrEmpty(data.Field<string>(5)) ? "0" : data.Field<string>(5));
                        this.RSEEfficiency = double.Parse(string.IsNullOrEmpty(data.Field<string>(6)) ? "0" : data.Field<string>(6));
                        this.RSEWorkingTime = double.Parse(string.IsNullOrEmpty(data.Field<string>(7)) ? "0" : data.Field<string>(7));
                        this.RSEYearlyProduction = double.Parse(string.IsNullOrEmpty(data.Field<string>(8)) ? "0" : data.Field<string>(8));
                        this.RSEEnYearlyConsumption = double.Parse(string.IsNullOrEmpty(data.Field<string>(9)) ? "0" : data.Field<string>(9));
                        this.RSENetPrice = double.Parse(string.IsNullOrEmpty(data.Field<string>(10)) ? "0" : data.Field<string>(10));
                        this.RSETax = double.Parse(string.IsNullOrEmpty(data.Field<string>(11)) ? "0" : data.Field<string>(11));
                        this.RSEGrossPrice = double.Parse(string.IsNullOrEmpty(data.Field<string>(12)) ? "0" : data.Field<string>(12));
                    }

                    break;
                case SurveyType.Energy:
                    
                    this.FinalPVConfig = double.Parse(string.IsNullOrEmpty(data.Field<string>(21)) ? "0" : data.Field<string>(21));
                    this.FinalRSEPower = double.Parse(string.IsNullOrEmpty(data.Field<string>(22)) ? "0" : data.Field<string>(22));
                    this.RSEEfficiency = double.Parse(string.IsNullOrEmpty(data.Field<string>(23)) ? "0" : data.Field<string>(23));
                    this.RSEYearlyProduction = double.Parse(string.IsNullOrEmpty(data.Field<string>(24)) ? "0" : data.Field<string>(24));
                    this.RSEEnYearlyConsumption = double.Parse(string.IsNullOrEmpty(data.Field<string>(25)) ? "0" : data.Field<string>(25));
                    this.RSEWorkingTime = double.Parse(string.IsNullOrEmpty(data.Field<string>(26)) ? "0" : data.Field<string>(26));
                    this.RSENetPrice = double.Parse(string.IsNullOrEmpty(data.Field<string>(27)) ? "0" : data.Field<string>(27));
                    this.RSETax = double.Parse(string.IsNullOrEmpty(data.Field<string>(28)) ? "0" : data.Field<string>(28));
                    this.RSEGrossPrice = double.Parse(string.IsNullOrEmpty(data.Field<string>(29)) ? "0" : data.Field<string>(29));

                    break;
                default:
                    break;
            }
        }


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
        [NotMapped]
        [Display(Name = "Udział własny")]
        public double RSEOwnerContrib { get; set; }

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
