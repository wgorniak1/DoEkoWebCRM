using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Survey
{

    public enum PowerSupplyType
    {
        [Display(Name ="Napowietrzne")]
        Type_1,
        [Display(Name ="Kablowe")]
        Type_2
    }
    public enum PowerConsMeterLocation
    {
        [Display(Name ="Dom")]
        Loc_1,
        [Display(Name ="W obrębie budynku, na którym nastąpi montaż instalacji")]
        Loc_2,
        [Display(Name ="W granicy działki")]
        Loc_3,
        [Display(Name ="Na słupie")]
        Loc_4
    }

    public enum CentralHeatingRadiatorType
    {
        [Display(Name = "Żeliwne Członowe")]
        Type_1,
        [Display(Name = "Aluminiowe Członowe")]
        Type_2,
        [Display(Name = "Stalowe Ożebrowane")]
        Type_3,
        [Display(Name = "Stalowe Płytowe")]
        Type_4,
        [Display(Name = "Stalowe Członowe")]
        Type_5,
        [Display(Name = "Inne")]
        Type_6
    }
    public enum PowerCompanyName
    {
        [Display(Name = "RWE")]
        Type_1,
        [Display(Name = "PGE")]
        Type_2,
        [Display(Name = "Enea")]
        Type_3,
        [Display(Name = "Tauron")]
        Type_4,
        [Display(Name = "Energa")]
        Type_5,
        [Display(Name = "Inne")]
        Type_6
    }

    public enum PhaseCount
    {
        [Display(Name = "1")]
        One,
        [Display(Name = "2")]
        Two
    }


    [ComplexType]
    public class SurveyDetEnergyAudit
    {
        [Key, ForeignKey("Survey")]
        public Guid SurveyId { get; set; }
        [Display(Name = "Moc przyłącza [kW]")]
        public double ElectricityPower { get; set; }        
        [Display(Name = "Rodzaj gł. źródła C.O.")]
        public PowerCompanyName PowerCompanyName { get; set; }
        [Display(Name = "Rodzaj przyłącza do sieci el.")]
        public PowerSupplyType PowerSupplyType { get; set; }
        [Display(Name = "Umiejscowienie licznika en. el.")]
        public PowerConsMeterLocation PowerConsMeterLocation { get; set; }
        [Display(Name = "Istnieje dodatkowy licznik energii")]
        public bool ENAdditionalConsMeter { get; set; }
        [Display(Name = "Mies. rachunek za prąd [PLN]")]
        [DisplayFormat(ApplyFormatInEditMode = true,ConvertEmptyStringToNull = true,DataFormatString = "{0:C}",HtmlEncode = true)]
        public decimal ElectricityAvgMonthlyCost { get; set; }
        [Display(Name = "Roczne zużycie prądu [kW]")]
        public double PowerAvgYearlyConsumption { get; set; }
        [Display(Name = "Planowana moc instalacji PV")]
        public double ENPowerLevel { get; set; }

        [Display(Name = "Istnieje instalacja uziemiająca")]
        public bool ENIsGround { get; set; }
        [Display(Name = "Podpisano umowę kompleksową")]
        public bool ComplexAgreement { get; set; }
        [Display(Name = "Liczba faz")]
        public PhaseCount PhaseCount { get; set; }
        [Display(Name = "Moc gł. źródła C.W.U.")]
        public double HWSourcePower { get; set; }
        [Display(Name = "Ogrzewanie grzejnikowe")]
        public bool CHRadiatorsInstalled { get; set; }
        [Display(Name = "Typ grzejników")]
        public CentralHeatingRadiatorType CHRadiatorType { get; set;  }
        [Display(Name = "Ogrzewanie podłogowe")]
        public bool CHFRadiantFloorInstalled { get; set; }
        [Display(Name ="Powierzchnia ogrzewana [% pow. budynku]")]
        [Range(0,100)]
        public int CHRadiantFloorAreaPerc { get; set; }
        [Display(Name = "Roczne zużycie paliwa (C.O. i C.W.U.)")]
        public double AverageYearlyFuelConsumption { get; set; }
        [Display(Name = "Koszty roczne (C.O. i C.W.U.)")]
        public decimal AverageYearlyHeatingCosts { get; set; }
        [Display(Name = "Moc kotła C.O. [kW]")]
        public double BoilerNominalPower { get; set; }
        [Display(Name = "Temp. rzeczywista inst.")]
        public double BoilerMaxTemp { get; set; }
        [Display(Name = "Rok produkcji kotła")]
        public short BoilerProductionYear { get; set; }
        [Display(Name = "Czy planowana jest wymiana kotła?")]
        public bool BoilerPlannedReplacement { get; set; }
        [Display(Name = "Czy istn. dodatkowe źr. ciepła")]
        public bool AdditionalHeatSource { get; set; }
        [Display(Name = "Parametry dod. źr. ciepła")]
        public string AdditionalHeatParams { get; set; }       
        [Display(Name = "Czy jest pionowy zasobnik C.W.U.")]
        public bool TankExists { get; set; }
        [Display(Name = "Pojemność zasobnika C.W.U.")]
        public double TankVolume { get; set; }
        [Display(Name = "Pow. wężownicy C.W.U.")]
        public double TankCoilSize { get; set; }
        [Display(Name = "Czy planowana pompa ciepła będzie jedynym źródłem C.O.")]
        public bool CHIsHPOnlySource { get; set; }
        public virtual Survey Survey { get; set; }
    }
}
