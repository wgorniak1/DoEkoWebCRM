using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DoEko.Models;
using DoEko.Controllers.Extensions;

namespace DoEko.Models.DoEko.Addresses
{
    public enum CommuneType
    {
        [Display( Name = "Gmina M.", Description = "Gmina miejska") ]
        City = 1,
        [Display(Name = "Gmina W.", Description = "Gmina wiejska" )]
        Valley = 2,
        [Display(Name = "Gmina M-W", Description = "Gmina miejsko-wiejska")]
        CityValley = 3,
        [Display(Name = "Miasto w Gm.M-W", Description = "Miasto w Gm.M-W")]
        CityInCityValley = 4,
        [Display(Name = "Obszar wiejski", Description = "Obszar wiejski")]
        ValleyArea = 5,
        [Display(Name = "Dz. Warszawa-Centr.", Description = "Dzielnice Gm. Warszawa-Centrum")]
        CityArea = 8,
        [Display(Name = "Deleg. i dzielnice", Description = "Delegatury i dzielnice")]
        OtherArea = 9
    }

    [Table(nameof(Commune))]
    public class Commune
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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 3)]
        [Display(Description = "", Name = "Id Gminy", ShortName = "Id Gminy")]
        public int CommuneId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 4)]
        [Display(Description = "", Name = "Typ gminy", ShortName = "Typ gm.")]
        [EnumDataType(typeof(CommuneType))]
        public CommuneType Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Column(Order = 5)]
        [StringLength(50, ErrorMessage = "Pole {0} nie może być dłuższe niż {1} ani krótsze niż {2}", MinimumLength = 10)]
        [Display(Description = "", Name = "Nazwa", ShortName = "Nazwa")]
        public string Text { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("StateId,DistrictId")]
        public virtual District District { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return Text + '(' + Type.DisplayName() + ')';
            }
            private set { }
        }
        
    }
}
