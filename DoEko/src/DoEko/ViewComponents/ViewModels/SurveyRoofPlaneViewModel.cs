using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoEko.Models.DoEko.Survey;
using System.ComponentModel.DataAnnotations;

namespace DoEko.ViewComponents.ViewModels
{
    public class SurveyRoofPlaneViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid SurveyId { get; set; }
        public SurveyDetRoof Plane { get; set; }
        public int RoofNumber { get; set; }
        public int RoofTotal { get; set; }
    }
}
