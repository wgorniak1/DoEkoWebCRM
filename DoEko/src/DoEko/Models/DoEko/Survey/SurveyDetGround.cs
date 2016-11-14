using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Survey
{
    public enum SlopeTerrain
    {
        [Display(Name="Nie")]
        None,
        [Display(Name="Wschód")]
        East,
        [Display(Name="Zachód")]
        West,
        [Display(Name="Południe")]
        South,
        [Display(Name="Północ")]
        North
    }

    [ComplexType]
    public class SurveyDetGround
    {
        [Key, ForeignKey("Survey")]
        public Guid SurveyId { get; set; }

        //ground_wetland
        [Display(Name = "Czy teren okresowo podmokły?")]
        public bool WetLand { get; set; }
        //ground_oth_installation
        //Obecność instalacji podziemnych: (jeżeli TAK proszę podać rodzaj, np.wodociąg, instalacja grzewcza, gazociąg)		
        [Display(Name = "Obecność instalacji podziemnych")]
        public bool OtherInstallation { get; set; }
        //ground_rock_exists
        //Obecność skał i kamieni w gruncie utrudniających prowadzenie robót ziemnych
        [Display(Name = "Grunt kamienisty")]
        public bool Rocks { get; set; }
        //ground_former_miltary
        [Display(Name = "Były teren wojskowy lub wysypisko śmieci")]
        public bool FormerMilitary { get; set; }
        //ground_slope_terrain
        [Display(Name = "Kierunek nachylenia terenu")]
        public SlopeTerrain SlopeTerrain{ get;set; }
        //ground_area_of_intallation
        [Display(Name = "Pow. przeznaczona pod instalację [m2]")]
        public double Area { get; set; }

        public virtual Survey Survey { get; set; }
    }
}
