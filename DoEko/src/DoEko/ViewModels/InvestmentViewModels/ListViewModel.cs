using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoEko.Models.DoEko;
namespace DoEko.ViewModels.InvestmentViewModels
{
    public class ListViewModel : Investment
    {
        public BusinessPartner FirstOwner { get; set; }
        public string OwnerFullName { get; set; }
    }
}
