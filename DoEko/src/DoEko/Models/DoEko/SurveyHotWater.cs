using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko
{
    public enum TargetHotWaterType
    {
        [Display(Name = "Kolektory płaskie")]
        FlatCollector,
        [Display(Name = "Kolektory próżniowe")]
        VacuumCollector,
        [Display(Name = "Pompa ciepła")]
        HeatPump
    }
    public enum RoofMaterial
    {
        [Display(Name = "Kolektory płaskie")]
        FlatCollector,
        [Display(Name = "Kolektory próżniowe")]
        VacuumCollector,
        [Display(Name = "Pompa ciepła")]
        HeatPump
    }
    public enum RoofType
    {
        [Display(Name = "Płaski")]
        Flat,
        [Display(Name = "Jedno spadowy")]
        Pitched_1,
        [Display(Name = "Dwu spadowy")]
        Pitched_2,
        [Display(Name = "Cztero spadowy")]
        Pitched_4
    }

    [Table(nameof(SurveyHotWater))]
    public class SurveyHotWater : Survey
    {
        //Moc głównego źródła ciepła dla c.w.u. ("moc pieca") [kW]
        public Double Current { get; set; }
        //Czy istnieje pomieszczenie techniczne(kotłownia)
        public Boolean BoilerStationExists { get; set; }
        //Czy zbiornik c.w.u.zmieści się przez drzwi do kotłowni(80 cm)
        public Boolean isDoorSizeEnough { get; set; }
        //Wymiary kotłowni(wpisz w osobnych komórkach w wierszu szerokość x długość x wysokość)
        public double BoilerStationSizeX { get; set; }
        public double BoilerStationSizeY { get; set; }
        public double BoilerStationSizeZ { get; set; }
        //Czy istnieje instalacja c.w.u w budynku
        public Boolean InstalationExists { get; set; }
        //Czy istnieje cyrkulacja c.w.u w budynku
        public Boolean CirculationExists { get; set; }
        //Czy są 3 uziemione gniazda w pomieszczeniu kotłowni (jeśli nie - poinformować o konieczności ich wykonania do czasu montażu solarów)
        public Boolean GroundedSocketsExists { get; set; }
        //Czy istnieje w kotłowni wolny przewód wentylacyjny(nieeksploatowany)
        public Boolean AirVentilationExists { get; set; }
        //Czy istnieje reduktor ciśnienia c.w.u.
        public Boolean PresureRegulator { get; set; }
        //KOLEKTORY/POMPA CIEPŁA CWU
        public TargetHotWaterType TargetHotWaterType { get; set; }
        
        //1. Wysokość budynku[m]:		
        public double BuildingSizeZ { get; set; }
        //2. Wysokość okapu[m]:		
        public double RoofHeight { get; set; }
        //3. Długość dachu[m]:		
        public double RoofWidth { get; set; }
        //4. Długość krawędzi dachu		
        public double RoofEdgeWeight { get; set; }
        //5. Długość grzbietu[m]:	
        public double RoofRidgeWeight { get; set; }
        //6. Kąt pochylenia dachu		
        public double RoofInclinationAngle { get; set; }
        //7a.Długość[m]:		
        public double BuildingSizeX { get; set; }
        //7b.Szerokość[m]:		
        public double BuildingSizeY { get; set; }
        //Powierzchnia przeznaczona pod instalację w m2
        public double InstallationSpace { get; set; }
        //Azymut
        public double Azimuth { get; set; }
        //Pokrycie dachowe:		
        public RoofMaterial RoofMaterial { get; set; }
        //Czy istnieje instalacja odgromowa?		
        public Boolean LightingRodExists { get; set; }
        //Czy są kominy na połaci na której montowana będzie instalacja?		
        public Boolean ChimneysExists { get; set; }
        //Czy są okna na połaci lub na elewacji na której montowana będzie instalacja?
        public Boolean RoofWindowsExists { get; set; }
        //Czy są świetliki na dachu na którym będzie montowana instalacja?
        public Boolean RoofLightsExists { get; set; }
        //Czy są instalacje rozprowadzone pod pokryciem dachu?		
        public Boolean UnderRoofInstallationExists { get; set; }

    }
}
