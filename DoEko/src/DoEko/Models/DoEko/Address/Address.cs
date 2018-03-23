using DoEko.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoEko.Models.DoEko.Addresses
{
    [Table(nameof(Address))]
    public class Address
    {   
        public Address()
        {
            //Polish address by default
            CountryId = 11;
        }
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
        [StringLength(6, ErrorMessage = "Proszę wprowadzić {0} w formacie 00-000", MinimumLength = 6)]
        [DataType(DataType.PostalCode)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:##-###}")]
        [Display(Description = "", Name = "Kod Poczty", ShortName = "Kod P.",Prompt = "00-000")]
        [RegularExpression(pattern: @"^([0-9]{2}-[0-9]{3})$",ErrorMessage = "Proszę wprowadzić {0} w formacie 00-000")]
        public string PostalCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [Column(Order = 8)]
        [StringLength(50, ErrorMessage = "Proszę podać prawidłową nazwę miejscowości", MinimumLength = 3)]
        [Display(Description = "", Name = "Miejscowość", ShortName = "Miejscowość")]
        public string City { get; set; }
        //[Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [Column(Order = 9)]
        [StringLength(50, ErrorMessage = "Proszę podać prawidłową nazwę poczty", MinimumLength = 3)]
        [Display(Description = "", Name = "Poczta", ShortName = "Poczta")]
        public string PostOfficeLocation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 10)]
        [StringLength(50, ErrorMessage = "Proszę podać prawidłową nazwę ulicy", MinimumLength = 1)]
        [Display(Description = "Ulica", Name = "Ulica", ShortName = "Ulica")]
        public string Street { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [Column(Order = 11)]
        [StringLength(5, ErrorMessage = "Długość pola {0} nie może przekroczyć {1} znaków", MinimumLength = 1)]
        [Display(Description = "Opis", Name = "Nr Budynku", ShortName = "Nr Bud.")]
        public string BuildingNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 12)]
        [StringLength(5, ErrorMessage = "Nr mieszkania nie może przekroczyć 5 znaków", MinimumLength = 0)]
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
        
        public string SearchTerm { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public string FirstLine
        {
            get
            {
                string addressLine  = (string.IsNullOrEmpty(Street)) ? City + " " + BuildingNo : Street + " " + BuildingNo;
                addressLine += (string.IsNullOrEmpty(ApartmentNo)) ? null : ("/" + ApartmentNo).ToString();

                return addressLine;
            }
            private set
            {

            }
        }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public string SecondLine
        {
            get
            {
                string addressLine = PostalCode + " ";
                if (Commune != null)
                {
                    addressLine += (string.IsNullOrEmpty(Street)) ? Commune.FullName : City;
                }
                else
                {
                    addressLine += City;
                }
                return addressLine;
            }
            private set
            {

            }
        }
        [NotMapped]
        public string SingleLine
        {
            get
            {
                string address = FirstLine + ", " + SecondLine;
                return address;
            }
            private set
            {

            }
        }
        public static string GetSearchTerm( Address address)
        {
            return string.Concat(address.State?.Text ?? "", '/',
                                                 address.District?.Text ?? "", '/',
                                                 address.Commune?.Text ?? "", '/',
                                                 address.PostalCode, '/',
                                                 address.City, '/',
                                                 address.Street, '/',
                                                 address.BuildingNo, '/',
                                                 address.ApartmentNo).ToUpper();
        }
    }
}
