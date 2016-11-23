using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Addresses
{
    [Table(nameof(District))]
    public class District
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)]
        [Display(Description = "", Name = "Id Woj.", ShortName = "Woj.")]
        public int StateId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 2)]
        [Display(Description = "", Name = "Id Pow.", ShortName = "Pow.")]
        public int DistrictId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Column(Order = 3)]
        [StringLength(50, ErrorMessage = "Pole {0} nie może być dłuższe niż {1} ani krótsze niż {2}")]
        [Display(Description = "", Name = "Nazwa Powiatu", ShortName = "Nazwa")]
        public string Text { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "", Name = "Województwo", ShortName = "Woj.")]
        [ForeignKey("StateId")]
        public virtual State State { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "", Name = "Gminy", ShortName = "Gminy")]
        [ForeignKey("StateId, DistrictId")]
        public virtual ICollection<Commune> Communes { get; set; }
    }
}
