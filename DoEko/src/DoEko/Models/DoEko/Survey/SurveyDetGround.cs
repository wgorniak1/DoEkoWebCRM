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
        North,
        [Display(Name="Północny Wschód")]
        NorthEast,
        [Display(Name="Północny Zachód")]
        NorthWest,
        [Display(Name="Południowy Wschód")]
        SouthEast,
        [Display(Name="Południowy Zachód")]
        SouthWest

    }

    [ComplexType]
    public class SurveyDetGround
    {
        [Key, ForeignKey("Survey")]
        public Guid SurveyId { get; set; }
        public virtual Survey Survey { get; set; }

        [Display(Name = "Czy teren okresowo podmokły?")]
        public bool WetLand { get; set; }
        [Display(Name = "Obecność instalacji podziemnych")]
        public bool OtherInstallation { get; set; }
        [Display(Name = "Rodzaj instalacji")]
        public string OtherInstallationType { get; set;}
        [Display(Name = "Grunt kamienisty")]
        public bool Rocks { get; set; }
        [Display(Name = "Były teren wojskowy lub wysypisko śmieci")]
        public bool FormerMilitary { get; set; }
        [Display(Name = "Kierunek nachylenia terenu")]
        public SlopeTerrain SlopeTerrain{ get;set; }
        //ground_area_of_intallation
        [Display(Name = "Pow. przeznaczona pod instalację [m2]")]
        public double Area { get; set; }

    }
}
