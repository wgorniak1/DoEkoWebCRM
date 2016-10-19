using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko
{
    public class Survey
    {
        public Guid SurveyId { get; set; }
        public SurveyType Type { get; set; }
    }
}
