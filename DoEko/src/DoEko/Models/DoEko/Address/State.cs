using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Addresses
{
    [Table(nameof(State))]
    public class State
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)]
        public int StateId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Column(Order = 2)]
        [StringLength(5, ErrorMessage = "Pole {0} nie może być dłuższe niż {1} ani krótsze niż {2}",MinimumLength = 2)]
        [Display(Description = "", Name = "Skrót", ShortName = "Skrót")]
        public string Key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Column(Order = 3)]
        [StringLength(50, ErrorMessage = "Pole {0} nie może być dłuższe niż {1} ani krótsze niż {2}",MinimumLength = 10)]
        [Display(Description = "", Name = "Nazwa", ShortName = "Nazwa")]
        public string Text { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 4)]
        [Required]
        [StringLength(50, ErrorMessage = "Pole {0} nie może być dłuższe niż {1} ani krótsze niż {2}", MinimumLength = 10)]
        [Display(Description = "", Name = "Stolica Województwa", ShortName = "Stolica")]
        public string CapitalCity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual ICollection<District> Districts { get; set; }
    }
}
