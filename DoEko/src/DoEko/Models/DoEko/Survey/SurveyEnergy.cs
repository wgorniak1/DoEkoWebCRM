using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Survey
{
    [Table(nameof(SurveyEnergy))]
    public class SurveyEnergy : Survey
    {
        public SurveyRSETypeEnergy RSEType { get; set; }

    }
}
