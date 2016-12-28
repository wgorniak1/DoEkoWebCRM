using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoEko.Models.DoEko.Addresses;
using DoEko.Models.DoEko;
using System.ComponentModel.DataAnnotations;
using DoEko.Models.DoEko.Survey;

namespace DoEko.ViewModels.InvestmentViewModels
{
    public enum PageSize
    {
        [Display(Name ="5")]
        ps_5 = 5,
        [Display(Name ="10")]
        ps_10 = 10,
        [Display(Name ="25")]
        ps_25 = 25,
        [Display(Name ="50")]
        ps_50 = 50
    }
    public class InvestmentListViewModel
    {
        public InvestmentListViewModel()
        {
            Filtering = new InvestmentListFilter();
            Paging = new InvestmentListPaging();
            Sorting = new InvestmentListSorting();
            List = new List<InvestmentOwner>();
        }

        public InvestmentListFilter Filtering { get; set; }
        public InvestmentListPaging Paging { get; set; }
        public InvestmentListSorting Sorting { get; set; }
        public IList<InvestmentOwner> List { get; set; }
    }
    public class InvestmentListPaging
    {
        public int CurrentNumber { get; set; }
        public int TotalPages { get; set; }
        public PageSize PageSize { get; set; }
        public int TotalRecords { get; set; }

        internal void Calculate(int queryTotal)
        {
            if (queryTotal == 0)
            {
                TotalRecords = 0;
                TotalPages = 1;
                CurrentNumber = 1;
                PageSize = PageSize.ps_25;
                return;
            }
            TotalRecords = queryTotal;
            if (PageSize != 0)
            {
                //update Total pages
                TotalPages = queryTotal / (int)PageSize + ((queryTotal % (int)PageSize) > 0 ? 1 : 0);
                //adjust current page number
                CurrentNumber = CurrentNumber == 0 ? 1 : CurrentNumber > TotalPages ? TotalPages : CurrentNumber;
            }
            else
            {
                TotalPages = 1;
                CurrentNumber = 1;
                //shouldn't set default number of elements = 25?
            }

        }
    }
    public class InvestmentListSorting
    {
        public const string postfixUp = "_UP";
        public const string postfixDn = "_DN";

        public string sortBy { get; set; }
    }

    public class InvestmentListFilter
    {
        public InvestmentListFilter()
        {        }
        [Display(Name = "Gmina")]
        public string CommuneId { get; set; }
        [Display(Name = "Miejscowość")]
        public string City { get; set; }
        [Display(Name = "Szukana fraza")]
        public string FreeText { get; set; }
        public Guid UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ProjectId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ContractId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Status inspekcji")]
        public InspectionStatus Status { get; set; }

        public bool FilterByInspector { get; set; }

    }
}
