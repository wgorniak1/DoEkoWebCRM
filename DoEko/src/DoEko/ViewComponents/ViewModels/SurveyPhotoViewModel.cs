using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoEko.Models.DoEko.Survey;
using System.ComponentModel.DataAnnotations;
using DoEko.Models.DoEko;

namespace DoEko.ViewComponents.ViewModels
{
    public class SurveyPhotoViewModel
    {
        public Guid InvestmentId { get; set; }
        public Guid SurveyId { set; get; }
        public SurveyType Type { set; get; }
        public SurveyRSETypeCentralHeating Type_CH { get; set; }
        public SurveyRSETypeHotWater Type_HW { get; set; }
        public SurveyRSETypeEnergy Type_EN { get; set; }

        public SurveyPhotoViewModel()
        {
            Attachments = new Dictionary<string, SurveyPhoto>();    
        }
        public Dictionary<string,SurveyPhoto> Attachments { get; set; }
    }

    public class SurveyPhoto
    {
        //public string Key { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
