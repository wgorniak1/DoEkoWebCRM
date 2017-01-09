using DoEko.Models.DoEko.Survey;
using DoEko.Models.DoEko;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoEko.ViewModels.ReportsViewModels
{
    public class SurveyExtractViewModel
    {
        public SelectList ProjectList { get; set; }
        [Display(Name = "Projekt")]
        public int ProjectId { get; set; }
        public SelectList ContractList { get; set; }
        [Display(Name ="Umowa")]
        public int ContractId { get; set; }

    }
}
