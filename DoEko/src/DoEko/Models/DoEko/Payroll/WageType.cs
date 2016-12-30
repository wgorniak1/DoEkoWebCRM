using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Payroll
{
    public class WageType
    {
        [Key()]
        public int WageTypeId { get; set; }
        public string ShortDescription { get; set; }
    }
}
