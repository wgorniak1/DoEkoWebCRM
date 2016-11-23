﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoEko.Models.DoEko.Survey;
using System.ComponentModel.DataAnnotations;

namespace DoEko.ViewComponents.ViewModels
{
    public class SurveyAuditEN
    {
        public SurveyAuditEN(SurveyDetEnergyAudit audit)
        {
            SurveyId = audit.SurveyId;
        }
        /// <summary>
        /// 
        /// </summary>
        public Guid SurveyId { get; set; }
    }
}
