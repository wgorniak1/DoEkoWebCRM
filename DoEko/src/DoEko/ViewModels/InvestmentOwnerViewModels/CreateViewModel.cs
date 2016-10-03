using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoEko.Models.DoEko.Addresses;
using DoEko.Models.DoEko;

namespace DoEko.ViewModels.InvestmentOwnerViewModels
{
    public class CreateViewModel : IAddressRelated
    {

        public BusinessPartnerPerson OwnerPerson { get; set; }
        public BusinessPartnerEntity OwnerEntity { get; set; }
        public Guid InvestmentId { get; set; }
        public int ContractId { get; set; }
        public Address Address { get; set; }
        public int AddressId { get; set; }

        public bool Sponsor { get; set; }
        public bool SameLocation { get; set; }
    }
}
