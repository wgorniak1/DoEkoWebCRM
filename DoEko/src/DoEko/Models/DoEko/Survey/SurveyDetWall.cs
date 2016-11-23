using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Survey
{
    [ComplexType]
    public class SurveyDetWall
    {
        [Key, ForeignKey("Survey")]
        public Guid SurveyId { get; set; }
        public virtual Survey Survey { get; set; }

        [Display(Name = "Szerokość")]
        public double Width { get; set; }
        [Display(Name = "Wysokość")]
        public double Height{ get; set; }
        [Display(Name = "Powierzchnia")]
        public double UsableArea { get; set; }
        [Display(Name = "Azymut")]
        public double Azimuth { get; set; }
    }
}
