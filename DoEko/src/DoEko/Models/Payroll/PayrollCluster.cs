using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Payroll
{
    public class PayrollCluster
    {
        [Key()]
        public Guid PayrollClusterId { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime ChangedAt { get; set; }
        public Guid ChangedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        
        [ForeignKey("WageType")]
        public int WageTypeId { get; set; }
        public decimal WageAmount { get; set; }
        public Guid WageReference { get; set; }
        public virtual WageType WageType { get; set; }
    }
}
