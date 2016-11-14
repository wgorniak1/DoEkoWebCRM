using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Survey
{
    public enum BusinessActivity
    {
        [Display(Name = "Żadna")]
        None,
        [Display(Name = "Gospodarcza")]
        Office,
        [Display(Name = "Rolnicza")]
        Agricultural,
        [Display(Name = "Gospodarcza i rolnicza")]
        Both
    }

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
    public enum BuildingType
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

    [ComplexType]
    public class SurveyDetBuilding
    {
        [Key, ForeignKey("Survey")]
        public Guid SurveyId { get; set; }
        //czy w miejscu inwestycji prow.Jest dz.Gosp    gospod. / rolicza / gosp.I roln. / nie jest
        [Display(Name = "Rodzaj działalnośći")]
        public BusinessActivity BusinessActivity { get; set; }

        //lokalizacja instalacji_1 budynek gospodarczy / mieszkalny
        [Display(Name = "Przeznaczenie budynku")]
        public BuildingPurpose Purpose { get; set; }
        //lokalizacja instalacji_2 dach grunt elewacja
        [Display(Name = "Lokalizacja instalacji")]
        public InstallationLocalization InstallationLocalization { get; set; }
        ///typ budynku wolnostojący / bliżniak / szeregowy środkowy
        [Display(Name = "Rodzaj budynku")]
        public BuildingType Type { get; set; }
        //stan budynku    istniejący / w budowie(z komentarzem)
        [Display(Name = "Stan budynku")]
        public BuildingState Status { get; set; }
        //rok budowy(rok zakonczenia czy rozpoczecia)
        [Display(Name = "Rok zakończenia budowy")]
        public short CompletionYear { get; set; }
        //building area
        [Display(Name = "Powierzchnia użytkowa")]
        public double UsableArea { get; set; }
        //building area total
        [Display(Name = "Powierszchnia całkowita")]
        public double TotalArea { get; set; }
        //number of occupants        
        [Display(Name = "Liczba mieszkańców")]
        public short NumberOfOccupants { get; set; }
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
        //building_type

        public virtual Survey Survey { get; set; }
    }
}
