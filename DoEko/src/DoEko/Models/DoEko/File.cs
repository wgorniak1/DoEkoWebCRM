using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko
{
    public class File
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string ParentType { get; set; }
        public int? ProjectId { get; set; }
        public int? ContractId { get; set; }
        public Guid? ParentId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime ChangedAt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid ChangedBy { get; set; }
    }
}
