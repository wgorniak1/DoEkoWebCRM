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
    }
}
