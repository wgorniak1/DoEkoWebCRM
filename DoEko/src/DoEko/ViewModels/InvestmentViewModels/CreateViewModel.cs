using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoEko.Models.DoEko.Addresses;
using DoEko.Models.DoEko;
using System.ComponentModel.DataAnnotations;
using DoEko.Models.DoEko.Survey;

namespace DoEko.ViewModels.InvestmentViewModels
{
    public class CreateViewModel : Investment
    {
        public CreateViewModel()
        {
            this.Address = new Address();
        }
        public CreateViewModel(Contract Contract)
            : this()
        {
            this.Contract = Contract;
            this.ContractId = Contract.ContractId;
        }

        public CreateViewModel(Contract Contract, int CountryId)
            : this(Contract)
        {
            this.Address.CountryId = CountryId;
        }
        public CreateViewModel(int CountryId)
            : this()
        {
            this.Address.CountryId = CountryId;
        }

        public Investment AsBase()
        {
            Investment baseclass = new Investment();

            baseclass.Address = this.Address;
            baseclass.AddressId = this.AddressId;
            baseclass.Contract = this.Contract;
            baseclass.ContractId = this.ContractId;
            baseclass.InspectionStatus = this.InspectionStatus;
            baseclass.InspectorId = this.InspectorId;
            baseclass.InvestmentId = this.InvestmentId;
            baseclass.InvestmentOwners = this.InvestmentOwners;
            baseclass.LandRegisterNo = this.LandRegisterNo;
            baseclass.Payments = this.Payments;
            baseclass.PlotNumber = this.PlotNumber;
            baseclass.Status = this.Status;

            baseclass.Address.CommuneType = (CommuneType)Enum.ToObject(typeof(CommuneType), this.Address.CommuneId % 10);
            baseclass.Address.CommuneId /= 10;

            return baseclass;
        }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "", Name = "Audyt pod E.E.", ShortName = "E.E.")]
        public bool SurveyEE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "", Name = "Audyt pod C.O.", ShortName = "C.O.")]
        public bool SurveyCH { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "", Name = "Audyt pod C.W.U.", ShortName = "C.W.U.")]
        public bool SurveyHW { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "", Name = "Typ OZE", ShortName = "OZE")]
        public SurveyRSETypeEnergy RSETypeEE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "", Name = "Typ OZE", ShortName = "OZE")]
        public SurveyRSETypeCentralHeating RSETypeCH { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "", Name = "Typ OZE", ShortName = "OZE")]
        public SurveyRSETypeHotWater RSETypeHW { get; set; }
    }
}
