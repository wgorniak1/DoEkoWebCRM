using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Survey
{
    //public enum TargetHotWaterType
    //{
    //    [Display(Name = "Kolektory płaskie")]
    //    FlatCollector,
    //    [Display(Name = "Kolektory próżniowe")]
    //    VacuumCollector,
    //    [Display(Name = "Pompa ciepła")]
    //    HeatPump
    //}

    public enum HotWaterConfiguration
    {
        [Display(Name = "2 kolektory płaskie o łącznej powierzchni min. 4,6 m2 oraz zasobnik min. 230 l")]
        Config_1,
        [Display(Name = "3 kolektory płaskie o łącznej powierzchni min. 6,9 m2 oraz zasobnik min. 345 l")]
        Config_2,
        [Display(Name = "4 kolektory płaskie o łącznej powierzchni min. 9,6 m2 oraz zasobnik min. 480 l")]
        Config_3,
        [Display(Name = "2 kolektory próżniowe o łącznej powierzchni min. 3 m2 oraz zasobnik min. 210 l")]
        Config_4,
        [Display(Name = "3 kolektory próżniowe o łącznej powierzchni min. 4,5 m2 oraz zasobnik min. 315 l")]
        Config_5,
        [Display(Name = "4 kolektory próżniowe o łącznej powierzchni min. 6 m2 oraz zasobnik min. 420 l")]
        Config_6,
    }

    [Table(nameof(SurveyHotWater))]
    public class SurveyHotWater : Survey
    {
        public SurveyRSETypeHotWater RSEType { get; set; }


        ////KOLEKTORY/POMPA CIEPŁA CWU
        //public TargetHotWaterType TargetHotWaterType { get; set; }
        

        //hw_rse_configuration
        public HotWaterConfiguration Configuration { get; set; }

    }
}
