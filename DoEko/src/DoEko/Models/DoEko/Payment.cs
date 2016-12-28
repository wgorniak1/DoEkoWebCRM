using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [Column(TypeName = "money")]
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [Display(Description = "", Name = "Kwota wpłaty", ShortName = "Kwota")]
        public decimal Amount { get; set; }
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
        public Boolean RseFotovoltaic { get; set; }
        public Boolean RseSolar { get; set; }
        public Boolean RseHeatPump { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //[NotMapped]
        //public SurveyType[] Types
        //{
        //    get
        //    {
        //        int[] types = Array.ConvertAll(TypeAsString.Split(';'), int.Parse);

        //        var enuTypes = new SurveyType[types.Count()];

        //        for (int i = 0; i < types.Count(); i++)
        //        {
        //            enuTypes[i] = (SurveyType)types[i];
        //        }
        //        return enuTypes;
        //    }
        //    set
        //    {
        //        var _data = value;
        //        TypeAsString = String.Join(";", _data.Select(p => p.ToString()).ToArray());
        //    }
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public string TypeAsString { get; set; }

        public Guid? InvestmentId { get; set; }

        public int ContractId { get; set; }
        /// <summary>
        /// When user marks payment as not applicable for this contract
        /// </summary>
        public Boolean NotNeeded { get; set; }
    }
}
