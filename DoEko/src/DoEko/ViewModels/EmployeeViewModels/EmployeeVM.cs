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
        public ICollection<EmployeeUserVM> Users { get; set; }
    }
}
