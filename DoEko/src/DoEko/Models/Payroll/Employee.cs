using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoEko.Models.DoEko;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoEko.Models.Payroll
{
    public class Employee : BusinessPartnerPerson
    {
        [NotMapped]
        public Guid EmployeeId { get { return BusinessPartnerId; } set { BusinessPartnerId = value; } }

        
        public virtual ICollection<EmployeeUser> Users { get; set; }

        public Employee()
        {
        }
    }
}
