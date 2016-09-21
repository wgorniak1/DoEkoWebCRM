using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko
{
    [Table(nameof(Survey))]
    public class Survey
    {
        public int SurveyId { get; set; }
        public string version { get; set; }
        public virtual Investment Investment { get; set; }
    }
}
