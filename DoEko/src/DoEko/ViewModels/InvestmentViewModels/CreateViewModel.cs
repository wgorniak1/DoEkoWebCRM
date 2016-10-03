using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoEko.Models.DoEko.Addresses;
using DoEko.Models.DoEko;

namespace DoEko.ViewModels.InvestmentViewModels
{
    public class CreateViewModel : IAddressRelated
    {
        public CreateViewModel(int? ContractId)
        {
            Investment = new Investment();
            if (ContractId != null)
            {
                Investment.ContractId = ContractId.Value;
            }

            Address = Investment.Address;

            owners = new List<InvestmentOwner>(5);

            foreach (var io in owners)
            {
                io.Investment = this.Investment;
            }

        }
        public int AddressId { get; set; }
        public Address Address { get; set; }
        public Investment Investment { get; set; }
        public ICollection<InvestmentOwner> owners {get; set;}
    }
}
