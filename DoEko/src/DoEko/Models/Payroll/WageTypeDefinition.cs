using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoEko.Models.Payroll
{
    public enum WageTypeUnit
    {
        [Display(Name ="szt.")]
        Item = 1,
        [Display(Name = "godz.")]
        Hour,
        [Display(Name = "dn.")]
        Day,
    }

    [Table(name: "WageTypeCatalog")]
    public class WageTypeDefinition : IWageType
    {
        [Key]
        public int WageTypeDefinitionId { get; set; }
        public string Code { get; set; }
        public string ShortDescription { get; set; }
        public double Number { get; set; }
        public WageTypeUnit Unit { get; set; }
        public double Rate { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
    }
}
