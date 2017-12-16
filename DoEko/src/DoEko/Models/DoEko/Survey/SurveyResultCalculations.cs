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
            FillProperties(type, rseType, data);
        }

        public void FillProperties(SurveyType type, int rseType, DataRow data)
        {
            try
            {

                this.CO2DustEquivValue = this.ParseDouble("Kolumna 44", data.Field<string>(44));
                this.CO2DustEquivPercent = this.ParseDouble("Kolumna 45", data.Field<string>(45));
                this.CO2EquivValue = this.ParseDouble("Kolumna 46", data.Field<string>(46));
                this.CO2Value = this.ParseDouble("Kolumna 47", data.Field<string>(47));
                this.CO2Percent = this.ParseDouble("Kolumna 48", data.Field<string>(48));
                this.PM10Value = this.ParseDouble("Kolumna 49", data.Field<string>(49));
                this.PM10Percent = this.ParseDouble("Kolumna 50", data.Field<string>(50));
                this.PM25Value = this.ParseDouble("Kolumna 51", data.Field<string>(51));
                this.PM25Percent = this.ParseDouble("Kolumna 52", data.Field<string>(52));
                this.BenzoPirenValue = this.ParseDouble("Kolumna 53", data.Field<string>(53));
                this.BenzoPirenPercent = this.ParseDouble("Kolumna 54", data.Field<string>(54));

                switch (type)
                {
                    case SurveyType.CentralHeating:

                        this.HeatLossFactor = this.ParseDouble("Kolumna 30", data.Field<string>(30));
                        this.CHRequiredEnFactor = this.ParseDouble("Kolumna 31", data.Field<string>(31));
                        this.CHMaxRequiredEn = this.ParseDouble("Kolumna 32", data.Field<string>(32));
                        this.CHRequiredEn = this.ParseDouble("Kolumna 33", data.Field<string>(33));
                        this.HWRequiredEnYearly = this.ParseDouble("Kolumna 34", data.Field<string>(34));
                        this.FinalRSEPower = this.ParseDouble("Kolumna 35", data.Field<string>(35));
                        this.CHRSEWorkingTime = this.ParseDouble("Kolumna 36", data.Field<string>(36));
                        this.CHRSEYearlyProduction = this.ParseDouble("Kolumna 37", data.Field<string>(37));
                        this.HWRSEWorkingTime = this.ParseDouble("Kolumna 38", data.Field<string>(38));
                        this.HWRSEYearlyProduction = this.ParseDouble("Kolumna 39", data.Field<string>(39));
                        this.RSEEnYearlyConsumption = this.ParseDouble("Kolumna 40", data.Field<string>(40));
                        this.RSENetPrice = this.ParseDouble("Kolumna 41", data.Field<string>(41));
                        this.RSETax = this.ParseDouble("Kolumna 42", data.Field<string>(42));
                        this.RSEGrossPrice = this.ParseDouble("Kolumna 43", data.Field<string>(43));
                        break;
                    case SurveyType.HotWater:

                        if (rseType == (int)SurveyRSETypeHotWater.HeatPump)
                        {
                            this.HWRequiredEnYearly = this.ParseDouble("Kolumna 13", data.Field<string>(13));
                            this.FinalRSEPower = this.ParseDouble("Kolumna 14", data.Field<string>(14));
                            this.RSEYearlyProduction = this.ParseDouble("Kolumna 15", data.Field<string>(15));
                            this.RSEWorkingTime = this.ParseDouble("Kolumna 16", data.Field<string>(16));
                            this.RSEEnYearlyConsumption = this.ParseDouble("Kolumna 17", data.Field<string>(17));
                            this.RSENetPrice = this.ParseDouble("Kolumna 18", data.Field<string>(18));
                            this.RSETax = this.ParseDouble("Kolumna 19", data.Field<string>(19));
                            this.RSEGrossPrice = this.ParseDouble("Kolumna 20", data.Field<string>(20));

                        }
                        else
                        {
                            this.FinalSOLConfig = data.Field<string>(3);
                            this.HWRequiredEnYearly = this.ParseDouble("Kolumna 4", data.Field<string>(4));
                            this.FinalRSEPower = this.ParseDouble("Kolumna 5", data.Field<string>(5));
                            this.RSEEfficiency = this.ParseDouble("Kolumna 6", data.Field<string>(6));
                            this.RSEWorkingTime = this.ParseDouble("Kolumna 7", data.Field<string>(7));
                            this.RSEYearlyProduction = this.ParseDouble("Kolumna 8", data.Field<string>(8));
                            this.RSEEnYearlyConsumption = this.ParseDouble("Kolumna 9", data.Field<string>(9));
                            this.RSENetPrice = this.ParseDouble("Kolumna 10", data.Field<string>(10));
                            this.RSETax = this.ParseDouble("Kolumna 11", data.Field<string>(11));
                            this.RSEGrossPrice = this.ParseDouble("Kolumna 12", data.Field<string>(12));
                        }

                        break;
                    case SurveyType.Energy:

                        this.FinalPVConfig = this.ParseDouble("Kolumna 21", data.Field<string>(21));
                        this.FinalRSEPower = this.ParseDouble("Kolumna 22", data.Field<string>(22));
                        this.RSEEfficiency = this.ParseDouble("Kolumna 23", data.Field<string>(23));
                        this.RSEYearlyProduction = this.ParseDouble("Kolumna 24", data.Field<string>(24));
                        this.RSEEnYearlyConsumption = this.ParseDouble("Kolumna 25", data.Field<string>(25));
                        this.RSEWorkingTime = this.ParseDouble("Kolumna 26", data.Field<string>(26));
                        this.RSENetPrice = this.ParseDouble("Kolumna 27", data.Field<string>(27));
                        this.RSETax = this.ParseDouble("Kolumna 28", data.Field<string>(28));
                        this.RSEGrossPrice = this.ParseDouble("Kolumna 29", data.Field<string>(29));

                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                throw;
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
        public bool Completed { get; set; }
        #region helpers
        private Double ParseDouble(string name, string s)
        {
            try
            {
                return Double.Parse(string.IsNullOrEmpty(s) ? "0" : s);
            }            
            catch (ArgumentNullException)
            {
                //s is null.
                throw new DoubleParseException(message: "wartość '{1}' w polu '{0}' jest pusta", fieldname: name, fieldvalue: s);
            } 
            catch (FormatException) 
            {
                //s does not represent a number in a valid format.
                throw new DoubleParseException(message: "Nie można odczytać wartości '{1}' w polu '{0}' jako liczby.", fieldname: name, fieldvalue: s);

            } 
            catch (OverflowException) 
            {
                //s represents a number that is less than MinValue or greater than MaxValue.
                throw new DoubleParseException(message: "wartość '{1}' w polu '{0} jest poza dozwolonym zakresem.'", fieldname: name, fieldvalue: s);
            }
        }
        #endregion
    }

    public class DoubleParseException : SystemException
    {
        public DoubleParseException() : base() { }

        public DoubleParseException(string message) : base(message)
        {
        }

        public DoubleParseException(string fieldname, string fieldvalue)
            : this(fieldname, fieldvalue, "Nieprawidłowa wartość '{1}' w polu '{0}'") { }

        public DoubleParseException(string fieldname, string fieldvalue, string message) : base(message)
        {
            this.Fieldname = fieldname;
            this.Fieldvalue = fieldvalue;
        }

        public string Fieldname { get; }
        public string Fieldvalue { get; }
        public override string Message
        {
            get
            {
                return string.Format(base.Message, Fieldname, Fieldvalue);
            }
        }
    }
}
