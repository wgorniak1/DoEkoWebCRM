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
        public Guid InvestmentId { get; set; }
        public virtual Investment Investment { get; set; }
        public Guid OwnerId { get; set; }
        public virtual BusinessPartner Owner { get; set; }
        public Boolean Sponsor { get; set; }
    }
}
