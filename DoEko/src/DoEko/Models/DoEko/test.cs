using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko
{
    public class test
    {
        [Key]
        public Guid PaymentId { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime ChangedAt { get; set; }
        public bool checkme { get; set; }
        public string dfg { get; set; }
    }
    public class test1
    {
        [Key]
        public int PaymentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ChangedAt { get; set; }
        public bool checkme { get; set; }
        public string dfg { get; set; }
    }

}
