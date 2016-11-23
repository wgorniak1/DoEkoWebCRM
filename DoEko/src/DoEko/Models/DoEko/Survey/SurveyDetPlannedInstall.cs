using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Survey
{
    public enum HotWaterConfiguration
    {
        [Display(Name = "Brak preferencji")]
        Config_0,
        [Display(Name = "Kolektory płaskie")]
        Config_1,
        [Display(Name = "Kolektory próżniowe")]
        Config_2
    }

    [ComplexType]
    public class SurveyDetPlannedInstall
    {
        /// <summary>
        /// 
        /// </summary>
        [Key, ForeignKey("Survey")]
        public Guid SurveyId { get; set; }
         /// <summary>
         ///lokalizacja instalacji_1 budynek gospodarczy / mieszkalny 
         /// </summary>
        [Display(Name = "Przeznaczenie budynku")]
        public BuildingPurpose Purpose { get; set; }
        /// <summary>
        ///lokalizacja instalacji_2 dach grunt elewacja 
        /// </summary>
        [Display(Name = "Lokalizacja instalacji")]
        public InstallationLocalization Localization { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Możliwy montaż jednostki zewnętrznej pompy ciepła na ścianie")]
        public bool OnWallPlacementAvailable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Czy klient żąda zamontowania określonego typu kolektorów?")]
        public HotWaterConfiguration Configuration { get; set; }

        public virtual Survey Survey { get; set; }
    }
}
