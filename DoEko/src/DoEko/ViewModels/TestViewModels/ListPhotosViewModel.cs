using DoEko.Models.DoEko.Survey;
using DoEko.Models.DoEko;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.ViewModels.TestViewModels
{
    public class ListPhotosViewModel
    {
        public Survey Survey { get; set; }
        public SortedList<string,SurveyAttachment> Attachments { get; set; }

        public ListPhotosViewModel(Survey survey)
        {
            Survey = survey;
            Attachments = new SortedList<string,SurveyAttachment>();
        }
    }

    public class SurveyAttachment
    {
        public string PictureId { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
    }
}
