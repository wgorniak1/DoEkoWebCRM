using DoEko.Models.Payroll;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.Payroll
{
    public class PayrollCluster
    {
        public Guid PayrollClusterId { get; set; }
        public Guid EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
        public DateTime ChangedAt { get; set; }
        public Guid ChangedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }

        public DateTime PeriodFor { get; set; }
        public DateTime PeriodIn { get; set; }
        public short SequenceNo { get; set; }
        
        public ICollection<PayrollResult> Results { get; set; }

        [NotMapped]
        public double TotalPayout {
            get {
                return this.Results
                    .Where(r => r.Code == "TOTL")
                    .Sum(r => r.Amount);
            }
            private set {

            }
        }
    }
    
    [ComplexType]
    public class PayrollResult : IWageType
    {
        public Guid PayrollResultId { get; set; }
        public Guid PayrollClusterId { get; set; }
        [ForeignKey("PayrollClusterId")]
        public virtual PayrollCluster PayrollCluster { get; set; }

        public string Code { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public double Number { get; set; }
        public double Rate { get; set; }
        public string ShortDescription { get; set; }
        public WageTypeUnit Unit { get; set; }
        public Guid SurveyId { get; set; }
    }
}
