using DoEko.Models.Identity;
using DoEko.Models.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.ViewModels.EmployeeViewModels
{
    public class EmployeeVM : Employee
    {
        public virtual ApplicationUser CurrentUser { get; set; }
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        public ICollection<EmployeeUserVM> Users { get; set; }
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    }
}
