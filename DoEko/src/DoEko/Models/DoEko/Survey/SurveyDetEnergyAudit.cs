using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Survey
{
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
        [Display(Name ="W obrębie budynku")]
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
        Type_5
    }

    [ComplexType]
    public class SurveyDetEnergyAudit
    {
        [Key, ForeignKey("Survey")]
        public Guid SurveyId { get; set; }
        //ch_type
        //Rodzaj głównego źródła ciepła dla c.o.
        [Display(Name = "Rodzaj gł. źródła C.O.")]
        public CentralHeatingType CentralHeatingType { get; set; }
        //Paliwo głównego źródła ciepła dla c.o.
        //ch_fuel
        [Display(Name = "Paliwo gł. źródła C.O.")]
        public FuelType CentralHeatingFuel { get; set; }
        //Rodzaj głównego źródła ciepła dla c.w.u.
        //hw_type   
        [Display(Name = "Rodzaj gł. źródła C.W.U.")]
        public HotWaterType HotWaterType { get; set; }
        //Paliwo głównego źródła ciepła dla c.w.u.
        //hw_fuel
        [Display(Name = "Paliwo gł. źródła C.W.U.")]
        public FuelType HotWaterFuel { get; set; }
        //Moc przyłącza budynku[kW]:		

        //power_company
        [Display(Name = "Nazwa zakładu energet.")]
        public string PowerCompanyName { get; set; }
        //power_supply_type
        [Display(Name = "Rodzaj przyłącza do sieci el.")]
        public PowerSupplyType PowerSupplyType { get; set; }
        //power_energy_meter_location
        [Display(Name = "Umiejscowienie licznika en. el.")]
        public PowerConsMeterLocation PowerConsMeterLocation { get; set; }
        //power_of_electricity
        [Display(Name = "Moc przyłącza [kW]")]
        public double ElectricityPower { get; set; }
        //power_avg_monthly_cost
        [Display(Name = "Mies. rachunek za prąd [PLN]")]
        [DisplayFormat(ApplyFormatInEditMode = true,ConvertEmptyStringToNull = true,DataFormatString = "{0:C}",HtmlEncode = true)]
        public decimal ElectricityAvgMonthlyCost { get; set; }
        //power_avg_yearly_consumption
        [Display(Name = "Roczne zużycie prądu [kW]")]
        public double PowerAvgYearlyConsumption { get; set; }
        //planned_power_level_of_pv
        [Display(Name = "Planowana moc instalacji PV")]
        public double PVPowerLevel { get; set; }
        //hw_source_power
        [Display(Name = "Moc gł. źródła C.W.U.")]
        public double HWSourcePower { get; set; }
        //hw_is_installed
        [Display(Name = "Czy istnieje instalacja C.W.U. ")]
        public bool HWInstalled { get; set; }
        //hw_circulation_is_installed
        [Display(Name = "Czy istn. cyrkulacja C.W.U.")]
        public bool HWCirculationInstalled { get; set; }
        //hw_pressure_reductor_exists
        [Display(Name = "Czy istn. reduktor ciśń. C.W.U.")]
        public bool HWPressureReductorExists { get; set; }
        //radiator_heat
        [Display(Name = "Ogrzewanie grzejnikowe")]
        public bool CHRadiatorsInstalled { get; set; }

        //radiator_heat_type
        [Display(Name = "Typ grzejników")]
        public CentralHeatingRadiatorType CHRadiatorType { get; set;  }
        //Radiant floor heat
        [Display(Name = "Ogrzewanie podłogowe")]
        public bool CHFRadiantFloorInstalled { get; set; }
        //ch_hw_avg_fuel_yearly_consumption
        [Display(Name = "Roczne zużycie paliwa (C.O. i C.W.U.)")]
        public double AverageYearlyFuelConsumption { get; set; }
        //ch_hw_avg_yearly_costs
        [Display(Name = "Koszty roczne (C.O. i C.W.U.)")]
        public decimal AverageYearlyHeatingCosts { get; set; }
        //ch_boiler_power
        [Display(Name = "Moc kotła C.O. [kW]")]
        public double BoilerNominalPower { get; set; }
        //ch_boiler_max_temp
        [Display(Name = "Temp. rzeczywista inst.")]
        public double BoilerMaxTemp { get; set; }
        //ch_boiler_production_year
        [Display(Name = "Rok produkcji kotła")]
        public short BoilerProductionYear { get; set; }
        //ch_boiler_upcoming_replacement
        [Display(Name = "Czy planowana jest wymiana kotła?")]
        public bool BoilerPlannedReplacement { get; set; }
        //building_additional_heat_source
        [Display(Name = "Czy istn. dodatkowe źr. ciepła")]
        public bool AdditionalHeatSource { get; set; }
        //building_additional_heat_params
        [Display(Name = "Parametry dod. źr. ciepła")]
        public string AdditionalHeatParams { get; set; }
        //building_fireplace_watered
        [Display(Name = "Czy planuje się kominek z płaszczem wodnym?")]
        public bool FirePlaceWithWater { get; set; }
        //hw_tank_exists
        [Display(Name = "Czy jest zasobnik C.W.U.")]
        public bool TankExists { get; set; }
        //hw_tank_volume
        [Display(Name = "Pojemność zasobnika C.W.U.")]
        public double TankVolume { get; set; }
        //hw_coil_size
        [Display(Name = "Pow. wężownicy C.W.U.")]
        public double TankCoilSize { get; set; }
        //hw_is_installed

        public virtual Survey Survey { get; set; }
    }
}
