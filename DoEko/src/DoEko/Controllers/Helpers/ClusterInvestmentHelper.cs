using DoEko.Models.DoEko;
using DoEko.Models.DoEko.ClusterImport;
using DoEko.ViewModels.API.ClusterInvestmentViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Controllers.Helpers
{
    public static class ClusterInvestmentHelper
    {

        public static ClusterInvestmentVM ToViewModel()
        {
            return new ClusterInvestmentVM();
        }

        public static ClusterInvestment ToModel( ClusterInvestmentVM vm)
        {
            ClusterInvestment model = new ClusterInvestment
            {
                ClustInvestmentId = vm.ClusterInvestmentId,
                ContractId = vm.ContractId
            };

            if (vm.Person != null)
            {
                model.MemberType = BusinessPartnerType.Person;
                model.Address = vm.Person.Address;
                model.Email = vm.Person.Email;
                model.Name = vm.Person.FirstName;
                model.Name2 = vm.Person.LastName;
                model.Phone = vm.Person.PhoneNumber;
            }
            else
            {
                model.MemberType = vm.Organization.Type;
                model.Address = vm.Organization.Address;
                model.Email = vm.Organization.Email;
                model.Name = vm.Organization.Name;
                model.Name2 = vm.Organization.Name2;
                model.Phone = vm.Organization.PhoneNumber;
                model.CompanySize = vm.Organization.CompanySize;
                model.TaxId = vm.Organization.TaxId;
            }

            if (vm.NewInstallationFarm != null)
            {
                model.NewInstallation = true;
                model.Description = vm.NewInstallationFarm.Description;
                model.PvPower = vm.NewInstallationFarm.PvPower;
            }
            else if(vm.NewInstallationPros != null)
            {
                model.NewInstallation = true;
                model.EnYearlyConsumption = vm.NewInstallationPros.EnYearlyConsumption;
                model.PvPower = vm.NewInstallationPros.PvPower;
            }
            else if(vm.ExistingInstallation != null)
            {
                model.NewInstallation = false;
                model.PvPower = vm.ExistingInstallation.PvPower;
                model.PvYearlyProduction = vm.ExistingInstallation.PvYearlyProduction;
            }
            else
            {

            }
            
            return model;
        }
    }
}
