using DoEko.Models.DoEko.Survey;
using DoEko.Models.DoEko;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DoEko.ViewModels.ReportsViewModels
{
    public class ProgressSummaryViewModel
    {
        public IList<ContractStatistic> Contracts { get; set; }

        public ProgressSummaryViewModel()
        {
            Contracts = new List<ContractStatistic>();
        }
    }
    public class ContractStatistic
    {
        public ContractStatistic()
        {
            Surveys = new List<SurveyStatistic>();
        }
        public Contract Contract { get; set; }
        public int InvestmentCount { get; set; }

        public IList<SurveyStatistic> Surveys { get; set; }

    }

    public class SurveyStatistic
    {
        [Display(Name = "Rodzaj Energii")]
        public SurveyType Type { get; set; }
        [Display(Name = "Źródło OZE")]
        public int RSEType { get; set; }
        [Display(Name = "Nie rozpoczętych")]
        public int NotStarted { get; set; }
        [Display(Name = "Nie przypisanych")]
        public int NotStartedNotassigned { get; set; }
        [Display(Name = "W trakcie")]
        public int Draft { get; set; }
        [Display(Name = "Odrzuconych")]
        public int Rejected { get; set; }
        [Display(Name = "W akceptacji")]
        public int InApproval { get; set; }
        [Display(Name = "Anulowanych")]
        public int Cancelled { get; set; }
        [Display(Name = "Zakończonych")]
        public int Completed { get; set; }
        [Display(Name = "W sumie")]
        public int Total{ get; set; }
    }

}
