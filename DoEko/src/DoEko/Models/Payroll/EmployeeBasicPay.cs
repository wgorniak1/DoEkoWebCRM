using DoEko.Models.Payroll;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.Payroll
{
    public class EmployeeBasicPay : IWageType
    {
        public Guid EmployeeId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        //        public WageType WageType { get; set; }
        public string Code { get; set; }
        public string ShortDescription { get; set; }
        public double Number { get; set; }
        public WageTypeUnit Unit { get; set; }
        public double Rate { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
    }

    [ComplexType()]
    public class WageType : IWageType
    {
        public double Amount { get; set; }
        public string Code { get; set; }
        public string Currency { get; set; }
        public double Number { get; set; }
        public double Rate { get; set; }
        public string ShortDescription { get; set; }
        public WageTypeUnit Unit { get; set; }
    }
}
