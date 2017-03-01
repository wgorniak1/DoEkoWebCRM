﻿using DoEko.Controllers.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Survey
{
    public enum SurveyType
    {
        /// <summary>
        /// Pompa ciepła gruntowa / kocioł na pellet
        /// Przeznaczenie: Centralne Ogrzewanie
        /// </summary>
        [Display(Name = "Centralne Ogrzewanie")]
        CentralHeating,
        /// <summary>
        /// Instalacja Solarna = Kolektory płaskie / próżniowe lub Pompa ciepła
        /// Przeznaczenie: Ciepła woda użytkowa
        /// </summary>
        [Display(Name = "Ciepła Woda Użytk.")]
        HotWater,
        /// <summary>
        /// Instalacja fotowoltaiczna
        /// Przeznaczenie: Energia Elektryczna
        /// </summary>
        [Display(Name = "Energia Elektr.")]
        Energy
    }
    public enum SurveyRSETypeCentralHeating
    {
        [Display(Name = "Pompa Ciepła Grunt.")]
        HeatPump = 1,
        [Display(Name = "Kocioł na Pellet")]
        PelletBoiler = 2,
        [Display(Name = "Pompa Ciepła Powietrzna")]
        HeatPumpAir = 3
    }
    public enum SurveyRSETypeHotWater
    {
        [Display(Name = "Solary")]
        Solar = 3,
        [Display(Name = "Pompa Ciepła")]
        HeatPump = 4
    }
    public enum SurveyRSETypeEnergy
    {
        [Display(Name = "Panele Fotowolt.")]
        PhotoVoltaic = 5
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
        Approved,
        [Display(Name = "Anulowana")]
        Cancelled

    }

    public enum SurveyCancelType
    {
        [Display(Name = "Przed inspekcją")]
        Before = 1,
        [Display(Name = "Po inspekcji")]
        After,
        [Display(Name = "Instalacja niemożliwa")]
        NotPossible,
        [Display(Name = "Brak kontaktu z właścicielem")]
        NoContact,
        [Display(Name = "Błąd - niezamawiane źródło")]
        TechnicalIssue
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
        [Display(Description = "Rodzaj Energii", Name = "Rodzaj Energii", ShortName = "Rodzaj Energii")]
        public SurveyType Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym.")]
        [EnumDataType(typeof(SurveyStatus), ErrorMessage = "Błąd model")]
        [Display(Description = "Status inspekcji", Name = "Status inspekcji", ShortName = "Status ins.")]
        public SurveyStatus Status { get; set; }
        [Display(Description = "Pole może służyć do przekazania dodatkowych informacji przy odrzucaniu ankiety", Name = "Uwagi", ShortName = "Uwagi")]
        public string RejectComments { get; set; }

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

        //**************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "", Name = "Powód anulowania", ShortName = "Powód anulowania")]
        public SurveyCancelType? CancelType { get; set; }
        //cancellation_comments
        [Display(Description = "", Name = "Uwagi", ShortName = "Uwagi")]
        [Required(ErrorMessage = "{0} jest polem obowiązkowym.")]
        public string CancelComments { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }

        public DateTime ChangedAt { get; set; }
        public Guid ChangedBy { get; set; }

        public DateTime FirstEditAt { get; set; }
        public Guid FirstEditBy { get; set; }

        //***********************************************
        //nr obrębu (jeśli nie ma w nr ks lub działki)

        public SurveyDetBuilding Building { get; set; }
        public SurveyDetPlannedInstall PlannedInstall { get; set; }
        public SurveyDetBoilerRoom BoilerRoom { get; set; }
        public SurveyDetBathroom BathRoom { get; set; }
        public SurveyDetAirCond AirCondition { get; set; }
        public List<SurveyDetRoof> RoofPlanes { get; set; }
        public SurveyDetWall Wall { get; set; }
        public SurveyDetGround Ground { get; set; }
        public SurveyDetEnergyAudit Audit { get; set; }

        [Display(Description = "", Name = "Uwagi", ShortName = "Uwagi")]
        public string FreeCommments { get; set; }

        //
        [Display(Name = "Ostateczny dobór mocy OZE")]
        public double FinalRSEPower { get; set; }
        [Display(Name = "Sprawność OZE")]
        public double RSEEfficiency { get; set; }
        [Display(Name = "Czas pracy instalacji OZE")]
        public double RSEWorkingTime { get; set; }
        [Display(Name = "Roczna produkcja OZE")]
        public double RSEYearlyProduction { get; set; }
        [Display(Name = "Roczne zużycie en. elektr.")]
        public double RSEEnYearlyConsumption { get; set; }
        [Display(Name = "Kwota Netto")]
        public double RSENetPrice { get; set; }
        [Display(Name = "VAT")]
        public double RSETax { get; set; }
        [Display(Name = "Kwota brutto")]
        public double RSEGrossPrice { get; set; }

        //
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


        public string TypeFullDescription()
        {
            string source = "";
            switch (Type)
            {
                case DoEko.Survey.SurveyType.CentralHeating:
                    source = Type.DisplayName() + '|' +
                            (((SurveyCentralHeating)this).RSEType.DisplayName());
                    break;
                case DoEko.Survey.SurveyType.HotWater:
                    source = Type.DisplayName() + '|' +
                            (((SurveyHotWater)this).RSEType.DisplayName());
                    break;
                case DoEko.Survey.SurveyType.Energy:
                    source = Type.DisplayName() + '|' +
                            (((SurveyEnergy)this).RSEType.DisplayName());
                    break;
                default:
                    source = Type.DisplayName() + '|';
                    break;
            }
            return source;
        }

        public int GetRSEType () 
        {
            switch (Type)
            {
                case DoEko.Survey.SurveyType.CentralHeating:
                    return (int)((SurveyCentralHeating)this).RSEType;
                case DoEko.Survey.SurveyType.HotWater:
                    return (int)((SurveyHotWater)this).RSEType;
                case DoEko.Survey.SurveyType.Energy:
                    return (int)((SurveyEnergy)this).RSEType;
                default:
                    return 0;
            }
        }

    }
}
