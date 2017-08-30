using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoEko.Models.DoEko;
using System.ComponentModel.DataAnnotations.Schema;
using DoEko.Models.Identity;
using System.Collections.ObjectModel;

namespace DoEko.Models.Payroll
{
    public class Employee : BusinessPartnerPerson
    {
        public Guid EmployeeId { get { return BusinessPartnerId; } set { BusinessPartnerId = value; } }

        public virtual ICollection<EmployeeUser> Users { get; set; }

        public virtual ICollection<EmployeeBasicPay> BasicPay { get; set; }

        public Employee()
        {
        }
        
        static public Employee CreateFromUser(ApplicationUser user)
        {
            Employee ee = new Employee()
            {
                EmployeeId = Guid.NewGuid(),
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };
            EmployeeUser eu = new EmployeeUser()
            {
                EmployeeId = ee.EmployeeId,
                Employee = ee,
                UserId = Guid.Parse(user.Id),
                Start = DateTime.MinValue,
                End = DateTime.MaxValue
            };

            ee.Users = new Collection<EmployeeUser>
            {
                eu
            };

            return ee;

        }
    }
}
