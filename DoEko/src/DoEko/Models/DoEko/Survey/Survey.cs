using System;
using System.Collections.Generic;
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
        Before,
        [Display(Name = "Po inspekcji")]
        After,
        [Display(Name = "Instalacja niemożliwa")]
        NotPossible
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

        //*************************************************
        //*************************************************
        //cancellation_type before_inspection / after_inspection / not possible
        public SurveyCancelType? CancelType { get; set; }
        //cancellation_comments
        public string CancelComments { get; set; }

        //public double BuildingCurrentEnergyTotal { get; set; }


        //***********************************************
        //nr obrębu (jeśli nie ma w nr ks lub działki)
        //tytuł prawny do nieruchomości własność / współwłasność / dzierżawa / użyczenie

        public SurveyDetAirCond AirCondition { get; set; }
        public SurveyDetBathroom BathRoom { get; set; }
        public SurveyDetBoilerRoom BoilerRoom { get; set; }
        public SurveyDetBuilding Building { get; set; }
        public SurveyDetEnergyAudit Audit { get; set; }
        public SurveyDetGround Ground { get; set; }
        public SurveyDetRoof Roof { get; set; }
    }
}
