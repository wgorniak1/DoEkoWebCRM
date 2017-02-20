using DoEko.Models.DoEko.Addresses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DoEko.Models.DoEko.Survey;
using System.Collections;
using System.Linq;

namespace DoEko.Models.DoEko
{
    public enum InvestmentStatus
    {
        [Display(Name = "")]
        ValueToDelete = 0,
        [Display(Name = "Inicjalny")]
        Initial = 1,
        [Display(Name = "Zakończona")]
        Completed,
        [Display(Name = "Zamknięta")]
        Closed,
        [Display(Name = "Anulowana")]
        Cancelled
    }
    public enum InspectionStatus
    {
        [Display(Name = "")]
        ValueToDelete = 0,
        [Display(Name = "Nie utworzono")]
        NotExists = 1,
        [Display(Name = "W trakcie realizacji")]
        Draft,
        [Display(Name = "Do korekty")]
        Rejected,
        [Display(Name = "Do zatwierdzenia")]
        Submitted,
        [Display(Name = "Zweryfikowana")]
        Approved,
        [Display(Name = "Ukończona")]
        Completed
    }

    public enum BusinessActivity
    {
        [Display(Name = "Żadna")]
        None = 0,
        [Display(Name = "Gospodarcza")]
        Office,
        [Display(Name = "Rolnicza")]
        Agricultural,
        [Display(Name = "Gospodarcza i rolnicza")]
        Both
    }

    public enum BuildingType
    {
        [Display(Name = "Wolnostojący")]
        DetachedHouse,
        [Display(Name = "Bliźniak")]
        TwinHouse,
        [Display(Name = "Szeregowy środkowy")]
        SerialMiddle
    }
    public enum BuildingStage
    {
        [Display(Name = "Isniejący")]
        Completed = 1,
        [Display(Name = "W budowie")]
        InProgress = 2
    }

    public enum CentralHeatingType
    {
        [Display(Name = "Brak C.O.")]
        None = 1,
        [Display(Name = "Piec na paliwo gazowe")]
        GasFuelHeater,
        [Display(Name = "Grzejniki elektryczne")]
        ElectricHeaters,
        [Display(Name = "Piec na paliwo ciekłe")]
        LiquidFuelHeater,
        [Display(Name = "Kominek z płaszczem wodnym")]
        Fireplace,
        [Display(Name = "Kominek bez płaszcza wodnego")]
        FirePlace2,
        [Display(Name = "Pompa ciepła")]
        HeatPump,
        [Display(Name = "Piec na paliwo stałe")]
        CoalFuealHeater,
        [Display(Name = "Inne")]
        Other
    }
    public enum HotWaterType
    {
        [Display(Name = "Brak")]
        None = 1,
        [Display(Name = "Piec na paliwo stałe")]
        SolidFuelHeater,
        [Display(Name = "Piec na paliwo ciekłe")]
        LiquidFuelHeater,
        [Display(Name = "Piec na paliwo gazowe")]
        GasFuelHeater,
        [Display(Name = "Podgrzewacze przepływowe")]
        ElectricHeater,
        [Display(Name = "Pompa ciepła")]
        HeatPump,
        [Display(Name = "Kolektory słoneczne")]
        Solary,

    }
    public enum FuelType
    {
        [Display(Name = "Nie dotyczy")]
        NotApplicable = 1,
        [Display(Name = "Węgiel kamienny")]
        Coal,
        [Display(Name = "Drewno")]
        Wood,
        [Display(Name = "Gaz ziemny")]
        Gas,
        [Display(Name = "Pellet")]
        Pellet,
        [Display(Name = "Owies")]
        Oats,
        [Display(Name = "Gaz płynny")]
        LiquidGas,
        [Display(Name = "Energia elektryczna")]
        Eletricity,
        [Display(Name = "Olej opałowy")]
        Oil
    }

    [Table(nameof(Investment))]
    public class Investment : IAddressRelated
    {   
        /// <summary>
        /// 
        /// </summary>
        [Key]
        [Display(Description = "Opis", Name = "Id Inwestycji",ShortName = "Id Inwestycji")]
        public Guid InvestmentId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [Display(Description = "Opis", Name = "Umowa", ShortName = "Umowa")]
        public int ContractId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "Opis", Name = "Umowa", ShortName = "Umowa")]
        public virtual Contract Contract { get; set; }
        //public virtual Project Project { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [Display(Description = "Opis", Name = "Adres", ShortName = "Adres")]
        public int AddressId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [Display(Description = "Opis", Name = "Adres", ShortName = "Adres")]
        public virtual Address Address { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //[StringLength(19,MinimumLength = 19,ErrorMessage = "Proszę podać nr formacie 112233_4.5678.123/1")]
        //[RegularExpression("^[0-9]{3}(/)[0-9]{1}$", ErrorMessage = "Proszę podać nr formacie 112233_4.5678.123/1")]
        [Display(Description = "Opis", Name = "Nr Działki", ShortName = "Nr Działki")]
        public string PlotNumber { get; set; }
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "Proszę podać nr formacie 0000")]
        [Display(Description = "Opis", Name = "Nr obrębu", ShortName = "Nr obrębu")]
        public string PlotAreaNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //[StringLength(15, MinimumLength = 15, ErrorMessage ="Proszę podać numer w formacie AA1A/12345678/1")]
        //[RegularExpression("^[A-Z]{2}[0-9]{1}[A-Z]{1}(/)[0-9]{8}(/)[0-9]{1}$", ErrorMessage = "Proszę podać numer w formacie AA1A/12345678/1")]
        [Display(Description = "Opis", Name = "Nr Ks.Wieczystej", ShortName = "Nr Ks.Wieczystej")]
        public string LandRegisterNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [EnumDataType(typeof(InvestmentStatus),ErrorMessage ="Błądmodel")]
        [Display(Description = "Opis", Name = "Status", ShortName = "Status")]
        public InvestmentStatus Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [EnumDataType(typeof(InspectionStatus),ErrorMessage ="Błądmodel")]
        [Display(Description = "Opis", Name = "Status inspekcji", ShortName = "Status inspekcji")]
        public InspectionStatus InspectionStatus { get; set; }

        //inspector
        [Display(Description = "Opis", Name = "Inspektor", ShortName = "Inspektor")]
        public Guid? InspectorId { get; set; }

        //wlasciciel / inwestor
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "Opis", Name = "Właściciele", ShortName = "Właściciele")]
        public virtual ICollection<InvestmentOwner> InvestmentOwners { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "Opis", Name = "Wpłaty", ShortName = "Wpłaty")]
        public ICollection<Payment> Payments { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "Ankiety", Name = "Ankiety", ShortName = "Ankiety")]
        public virtual ICollection<Survey.Survey> Surveys { get; set; }
        /// <summary>
        /// Stałe łącze internetowe w m. inwestycji
        /// </summary>
        [Display(Description = "Ankiety", Name = "Stacjonarny internet w miejscu inwestycji", ShortName = "Internet")]
        public bool InternetAvailable { get; set; }

        //czy w miejscu inwestycji prow.Jest dz.Gosp    gospod. / rolicza / gosp.I roln. / nie jest
        [Display(Name = "Rodzaj działalnośći")]
        public BusinessActivity BusinessActivity { get; set; }

        ///typ budynku wolnostojący / bliżniak / szeregowy środkowy
        [Display(Name = "Rodzaj budynku")]
        public BuildingType Type { get; set; }

        //stan budynku    istniejący / w budowie(z komentarzem)
        [Display(Name = "Stan budynku")]
        public BuildingStage Stage { get; set; }
        //rok budowy(rok zakonczenia czy rozpoczecia)
        [Display(Name = "Rok zakończenia budowy")]
        [Range(1900,2100,ErrorMessage ="Proszę wprowadzić wartość z przedziału {1} - {2}")]
        public short CompletionYear { get; set; }
        //building area
        [Display(Name = "Powierzchnia użytkowa [m kw.]")]
        [Range(0.1,99999.9,ErrorMessage ="Proszę wprowadzić wartość z przedziału {1} - {2}")]
        public double UsableArea { get; set; }
        //building area total
        [Display(Name = "Powierzchnia całkowita [m kw.]")]
        [Range(0.1, 99999.9, ErrorMessage = "Proszę wprowadzić wartość z przedziału {1} - {2}")]
        public double TotalArea { get; set; }
        [Display(Name = "Powierzchnia ogrzewana [m kw.]")]
        [Range(0.1, 99999.9, ErrorMessage = "Proszę wprowadzić wartość z przedziału {1} - {2}")]
        public double HeatedArea { get; set; }
        [Display(Name = "Liczba mieszkańców")]
        [Range(0,99,ErrorMessage ="Proszę wprowadzić wartość z przedziału {1} - {2}")]
        public short NumberOfOccupants { get; set; }
        /// <summary>
        /// Określa kolejność zgłoszenia oraz ustala priorytet w przypadku 
        /// ograniczenia liczby inwestycji, które mogą być zrealizowane w danym projekcie.
        /// </summary>
        [Display(Description = "", Name = "Typ głównego źr. C.O.", ShortName = "Typ C.O.")]
        public CentralHeatingType CentralHeatingType { get; set; }
        [Display(Name = "Paliwo gł. źródła C.O.")]
        public FuelType CentralHeatingFuel { get; set; }
        [Display(Name = "Rodzaj gł. źródła C.W.U.")]
        public HotWaterType HotWaterType { get; set; }
        [Display(Name = "Paliwo gł. źródła C.W.U.")]
        public FuelType HotWaterFuel { get; set; }
        [Display(Name = "Inne źródło")]
        public string CentralHeatingTypeOther { get; set; }
        /// <summary>
        /// Określa kolejność zgłoszenia oraz ustala priorytet w przypadku 
        /// ograniczenia liczby inwestycji, które mogą być zrealizowane w danym projekcie.
        /// </summary>
        [Display(Description = "", Name = "Priorytet", ShortName = "Priorytet")]
        public long PriorityIndex { get; set; }
        #region Metody
        [NotMapped]
        public string PlotFullNumber
        {
            get
            {
                string teryt = "";
                try
                {
                    teryt = Address.StateId.ToString() +
                            Address.DistrictId.ToString() +
                            Address.CommuneId.ToString() + '_' +
                            Address.CommuneType.ToString() + '.' +
                            PlotAreaNumber.ToString() + '.' + 
                            PlotNumber;

                    return teryt;

                }
                catch (Exception)
                {
                    return PlotNumber;
                }
            }
            private set
            {
            }
        }
        #endregion
    }
}