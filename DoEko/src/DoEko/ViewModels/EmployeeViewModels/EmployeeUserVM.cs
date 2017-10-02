using DoEko.Models.Identity;
using DoEko.Models.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.ViewModels.EmployeeViewModels
{
    public class EmployeeUserVM : EmployeeUser
    {
        public virtual ApplicationUser User { get; set; }
    }
}
