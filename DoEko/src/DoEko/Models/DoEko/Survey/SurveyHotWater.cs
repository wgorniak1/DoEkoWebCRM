using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Survey
{

    [Table(nameof(SurveyHotWater))]
    public class SurveyHotWater : Survey
    {
        public SurveyRSETypeHotWater RSEType { get; set; }

        [Display(Name = "Ostateczny dobór kolektorów")]
        public string FinalSOLConfig { get; set; }
        [Display(Name = "Roczne zapotrzebowanie na en dla CWU - stan pierwotny")]
        public double HWRequiredEnYearly { get; set; }

    }
}
