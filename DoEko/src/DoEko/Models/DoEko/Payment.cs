using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko
{
    [Table(name:"Payment")]
    public class Payment
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public Guid PaymentId { get; set; }
        /// <summary>
        /// Amount paid by the customer
        /// </summary>
        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        [Display(Description = "", Name = "Kwota wpłaty", ShortName = "Kwota")]
        public double Amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [Display(Description = "", Name = "Data płatności", ShortName = "Data płatności")]
        public DateTime PaymentDate { get; set; }
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [Display(Description = "", Name = "Data zaksięgowania", ShortName = "Data zaks.")]
        public DateTime PostingDate { get; set; }
        /// <summary>
        /// technical field with source 
        /// </summary>
        public string SourceRow { get; set;}
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid CreatedBy { get; set; }
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
