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

    public enum BuildingType
    {
        [Display(Name = "Gospodarczy")]
        Business,
        [Display(Name = "Mieszkalny")]
        Housing
    }

    public enum InstallationLocalization
    {
        [Display(Name = "Dach")]
        Roof,
        [Display(Name = "Grunt")]
        Ground,
        [Display(Name = "Elewacja")]
        Wall
    }

    public enum BusinessActivity
    {
        [Display(Name = "Gospodarcza")]
        Office,
        [Display(Name = "Rolnicza")]
        Agricultural,
        [Display(Name = "Gospodarcza i rolnicza")]
        Both
    }

    public enum BuildingType2
    {
        [Display(Name = "Wolnostojący")]
        Type_1,
        [Display(Name = "Bliźniak")]
        Type_2,
        [Display(Name = "Szeregowy środkowy")]
        Type_3
    }

    public enum BuildingState
    {
        [Display(Name = "Isniejący")]
        Completed,
        [Display(Name = "W budowie")]
        InProgress
    }

    public enum CentralHeatingType
    {
        [Display(Name = "Piec na paliwo ciekłe")]
        LiquidFuelHeater,
        [Display(Name = "Piec na paliwo gazowe")]
        GasFuelHeater,
        [Display(Name = "Kominek z płaszczem wodnym")]
        Fireplace,
        [Display(Name = "Grzejniki elektryczne")]
        ElectricHeaters,
        [Display(Name = "Pompa ciepła")]
        HeatPump
    }

    public enum HotWaterType
    {
        [Display(Name = "Piec na paliwo stałe")]
        SolidFuelHeater,
        [Display(Name = "Piec na paliwo ciekłe")]
        LiquidFuelHeater,
        [Display(Name = "Piec na paliwo gazowe")]
        GasFuelHeater,
        [Display(Name = "Podgrzewacze przepływowe")]
        ElectricHeater,
        [Display(Name = "Pompa ciepła")]
        HeatPump,
    }


    public enum FuelType
    {
        [Display(Name = "Węgiel kamienny")]
        Coal,
        [Display(Name = "Drewno")]
        Wood,
        [Display(Name = "Gaz ziemny")]
        Gas,
        [Display(Name = "Pellet")]
        Pellet,
        [Display(Name = "Owies")]
        Oats,
        [Display(Name = "Gaz płynny")]
        LiquidGas,
        [Display(Name = "Energia elektryczna")]
        Eletricity,
        [Display(Name = "Olej opałowy")]
        Oil
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
        //lokalizacja instalacji_1 budynek gospodarczy / mieszkalny
        public BuildingType BuildingType { get; set; }
        //lokalizacja instalacji_2 dach grunt elewacja
        public InstallationLocalization InstallationLocalization { get; set;}
        //czy w miejscu inwestycji prow.Jest dz.Gosp    gospod. / rolicza / gosp.I roln. / nie jest
        public BusinessActivity BusinessActivity { get; set; }
        //internet available
        public Boolean InternetConnectionAvailable { get; set; }
        //typ budynku wolnostojący / bliżniak / szeregowy środkowy
        public BuildingType2 BuildingType2 { get; set; }
        //stan budynku    istniejący / w budowie(z komentarzem)
        public BuildingState BuildingState { get; set; }
        //rok budowy(rok zakonczenia czy rozpoczecia)
        public short BuildingCompletionYear { get; set; }
        //powierzchnia użytkowa budynku
        public double BuildingUsableArea { get; set; }
        //powierzchnia całkowita
        public double BuildingOverallArea { get; set; }
        //liczba mieszkańców
        public short BuildingNumberOfHosts { get; set; }
        //Moc przyłącza budynku[kW]:		
        public double BuildingCurrentEnergyTotal { get; set; }

        //Rodzaj głównego źródła ciepła dla c.o.
        public CentralHeatingType CentralHeatingType { get; set; }
        //Paliwo głównego źródła ciepła dla c.o.
        public FuelType CentralHeatingFuel { get; set; }
        //Rodzaj głównego źródła ciepła dla c.w.u.
        public HotWaterType HotWaterType { get; set; }
        //Paliwo głównego źródła ciepła dla c.w.u.
        public FuelType HotWaterFuel { get; set; }


        //***********************************************
        //nr obrębu (jeśli nie ma w nr ks lub działki)
        //tytuł prawny do nieruchomości własność / współwłasność / dzierżawa / użyczenie
    }
}
