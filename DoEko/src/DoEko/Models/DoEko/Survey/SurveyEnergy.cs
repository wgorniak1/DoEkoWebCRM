using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Survey
{
    [Table(nameof(SurveyEnergy))]
    public class SurveyEnergy : Survey
    {
        public SurveyEnergy() : base(SurveyType.Energy)
        {
        }
        public SurveyEnergy(Guid investmentId, SurveyRSETypeEnergy rseType) : this()
        {
            this.InvestmentId = investmentId;
            this.RSEType = rseType;
        }
        [Column("RSEType")]
        public SurveyRSETypeEnergy RSEType { get; set; }



    }
}
