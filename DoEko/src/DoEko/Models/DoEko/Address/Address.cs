using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoEko.Models.DoEko.Addresses
{
    [Table(nameof(Address))]
    public class Address
    {   
        /// <summary>
        /// 
        /// </summary>
        [Key]
        [Display(Description ="",Name ="Id",ShortName ="Id")]
        [Column(Order = 1)]
        public int AddressId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [Column(Order = 2)]
        [Display(Description = "", Name = "Kraj", ShortName = "Kraj")]
        public int CountryId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [Column(Order = 3)]
        [Display(Description = "", Name = "Województwo", ShortName = "Woj.")]
        public int StateId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [Column(Order = 4)]
        [Display(Description = "", Name = "Powiat", ShortName = "Powiat")]
        public int DistrictId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [Column(Order = 5)]
        [Display(Description = "", Name = "Gmina", ShortName = "Gmina")]
        public int CommuneId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [Column(Order = 6)]
        [Display(Description = "", Name = "Typ Gminy", ShortName = "Typ Gminy")]
        public CommuneType CommuneType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage ="{0} jest polem obowiązkowym")]
        [Column(Order = 7)]
        [StringLength(6, ErrorMessage = "Kod pocztowy ma długość 5 cyfr oddzielonych znakiem '-'", MinimumLength = 6)]
        [DataType(DataType.PostalCode)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:##-###}")]
        [Display(Description = "", Name = "Kod Poczty", ShortName = "Kod P.")]
        [RegularExpression(pattern: "^[0-9]{2}-[0-9]{3}$",ErrorMessage = "Kod pocztowy należy wprowadzić w formacie 00-000")]
        public string PostalCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [Column(Order = 8)]
        [StringLength(50, ErrorMessage = "Proszę podać prawidłową nazwę miejscowości", MinimumLength = 3)]
        [Display(Description = "", Name = "Miejscowość", ShortName = "Miejscowość")]
        public string City { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 9)]
        [StringLength(50, ErrorMessage = "Proszę podać prawidłową nazwę ulicy", MinimumLength = 1)]
        [Display(Description = "Ulica", Name = "Ulica", ShortName = "Ulica")]
        public string Street { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [Column(Order = 10)]
        [StringLength(5, ErrorMessage = "Proszę ", MinimumLength = 1)]
        [Display(Description = "Opis", Name = "Nr Budynku", ShortName = "Nr Bud.")]
        public string BuildingNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 11)]
        [StringLength(5, ErrorMessage = "Proszę", MinimumLength = 1)]
        [Display(Description = "", Name = "Nr Mieszkania", ShortName = "Nr Mieszk.")]
        public string ApartmentNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Country Country { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public State State { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public District District { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Commune Commune { get; set; }
    }
}
