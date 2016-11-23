using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoEko.Models.DoEko.Survey;
using System.ComponentModel.DataAnnotations;
using DoEko.Models.DoEko;

namespace DoEko.ViewComponents.ViewModels
{
    public class SurveyAuditViewModel
    {
        public SurveyAuditViewModel()
        {
            Investment = new Investment();
            Audit = new SurveyDetEnergyAudit();
        }
        public SurveyAuditViewModel(Survey survey)
        {
            SurveyId = survey.SurveyId;
            Investment = survey.Investment;
            if (survey.Audit == null)
            {
                Audit = new SurveyDetEnergyAudit() { SurveyId = survey.SurveyId };
            }
            else Audit = survey.Audit;
        }
        /// <summary>
        /// 
        /// </summary>
        public Investment Investment { get; set; }
        public Guid SurveyId { get; set; }

        public SurveyDetEnergyAudit Audit { get; set; }
       
    }
}
