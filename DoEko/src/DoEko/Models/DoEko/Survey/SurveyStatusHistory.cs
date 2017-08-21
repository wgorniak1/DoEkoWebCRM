using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Survey
{
    public class SurveyStatusHistory
    {
        public SurveyStatusHistory()
        {

        }
        public SurveyStatusHistory(Guid surveyId, DateTime start, Guid userId, SurveyStatus status)
        {
            SurveyId = surveyId;
            Start = start;
            End = DateTime.MaxValue;
            UserId = userId;
            Status = status;
        }

        [ForeignKey("Survey")]
        public Guid SurveyId { get; set; }
        public virtual Survey Survey { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Guid UserId { get; set; }
        public SurveyStatus Status { get; set; }
    }
}
