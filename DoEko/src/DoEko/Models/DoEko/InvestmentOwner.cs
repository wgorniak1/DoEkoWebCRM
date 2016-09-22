using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko
{
    [Table(nameof(InvestmentOwner))]
    public class InvestmentOwner
    {   
        public int InvestmentId { get; set; }
        public virtual Investment Investment { get; set; }
        public int OwnerId { get; set; }
        public virtual Owner Owner { get; set; }
        public Boolean Sponsor { get; set; }
    }
}
