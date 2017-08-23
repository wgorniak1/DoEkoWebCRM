using DoEko.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.Payroll
{
    public class EmployeeUser
    {
        
        public DateTime Start { get; set; }       
        public DateTime End { get; set; }
        public Guid EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
        public Guid UserId { get; set; }
        //public virtual ApplicationUser User { get; set; }
    }
}
