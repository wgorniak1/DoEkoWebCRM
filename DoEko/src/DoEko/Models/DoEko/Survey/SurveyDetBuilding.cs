using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Survey
{
    public enum BuildingPurpose
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
    
    
    public enum WallMaterial
    {
        [Display(Name = "Materiał 1")]
        Material_1,
        [Display(Name = "Materiał 2")]
        Material_2
    }
    public enum InsulationType
    {
        [Display(Name = "Materiał 1")]
        Ins_1,
        [Display(Name = "Materiał 2")]
        Ins_2
    }
    public enum BuildTechnologyType
    {
        [Display(Name = "Nowe budownictwo - pasywny")]
        Type_1,
        [Display(Name = "Nowe budownictwo - energooszczędny")]
        Type_2,
        [Display(Name = "Nowe budownictwo - standardowa izolacja")]
        Type_3,
        [Display(Name = "Starsze budownictwo - nieocieplony")]
        Type_4,
        [Display(Name = "Starsze budownictwo - ocieplony")]
        Type_5
    }

    [ComplexType]
    public class SurveyDetBuilding
    {
        [Key, ForeignKey("Survey")]
        public Guid SurveyId { get; set; }
        
       
        
        /// <summary>
        /// building_volume
        /// </summary>
        [Display(Name = "Kubatura budynku [m3]")]
        public double Volume { get; set; }
        //building_wall_material
        [Display(Name = "Materiał ścian budynku")]
        public WallMaterial WallMaterial { get; set; }
        //building_wall_thickness
        [Display(Name = "Grubość ścian [cm]")]
        public double WallThickness { get; set; }
        //building_insulation_type
        [Display(Name = "Rodzaj docieplenia")]
        public InsulationType InsulationType { get; set; }
        //building_insulation_thickness
        [Display(Name = "Grubość docieplenia [cm]")]
        public double InsulationThickness { get; set; }
        //build_technology
        public BuildTechnologyType TechnologyType { get; set;        }


        public virtual Survey Survey { get; set; }
    }
}
