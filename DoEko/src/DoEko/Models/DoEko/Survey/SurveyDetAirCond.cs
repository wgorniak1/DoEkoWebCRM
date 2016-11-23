using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Survey
{
    public enum ACType
    {
        [Display(Name="Aktywne")]
        Active,
        [Display(Name= "Pasywne")]
        Passive
    }

    [ComplexType]
    public class SurveyDetAirCond
    {
        [Key,ForeignKey("Survey")]
        public Guid SurveyId { get; set; }
        //building_ac_exists
        [Display(Name = "Czy istnieje chłodzienie budynku?")]
        public bool Exists { get; set; }
        //building_ac_planned
        [Display(Name = "Czy przewiduje się zastosowanie chłodzenia?")]
        public bool isPlanned { get; set; }
        //building_ac_type
        [Display(Name = "Typ instalacji")]
        public ACType Type { get; set; }
        //building_mech_ventilation_exists
        [Display(Name = "Czy w budynku jest wentylacja mechaniczna?")]
        public bool MechVentilationExists { get; set; }
        public virtual Survey Survey { get; set; }
    }
}
