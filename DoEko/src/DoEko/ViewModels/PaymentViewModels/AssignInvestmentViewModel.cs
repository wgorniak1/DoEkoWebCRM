using DoEko.Migrations.DoEko;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.ViewModels.PaymentViewModels
{
    public class AssignInvestmentViewModel
    {
        public Payment Payment { get; set; }
        public Guid InvestmentId { get; set; }
        
    }
}
