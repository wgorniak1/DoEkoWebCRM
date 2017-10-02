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

        public ICollection<EmployeeUser> Users { get; set; }

        public ICollection<EmployeeBasicPay> BasicPay { get; set; }

        [NotMapped]
        public Guid CurrentUserId
        {
            get
            {
                try
                {
                    var user = Users
                        .FirstOrDefault(u => u.Start <= DateTime.Now &&
                                               u.End >= DateTime.Now);
                    if (user != null)
                    {
                        return user.UserId;
                    }
                    else
                    {
                        return Guid.Empty;
                    }

                }
                catch (Exception exc)
                {
                    return Guid.Empty;
                }
            }
            private set { }
        }

        //actions
        //
        //payroll results
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
                PhoneNumber = user.PhoneNumber ?? "+48 111 222 333",
                Address = new DoEko.Addresses.Address
                {
                    StateId = 12,
                    DistrictId = 61,
                    CommuneId = 1,
                    CommuneType = DoEko.Addresses.CommuneType.City,
                    City = "Kraków",
                    Street = "os.Bociana",
                    BuildingNo = "4A",
                    ApartmentNo = "49",
                    CountryId = 11,
                    PostalCode = "31-231"
                }
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
