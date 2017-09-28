using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using DoEko.Models.DoEko.ClusterImport;
using DoEko.ViewModels.API.ClusterInvestmentViewModels;

namespace DoEko.Models.Validation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ClusterInvestmentAttribute : ValidationAttribute
    {
        public ClusterInvestmentAttribute()
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            //ClusterInvestmentVM inv = (ClusterInvestmentVM)value;
            //if (inv.Organization != null && inv.Organization.Type == DoEko.BusinessPartnerType.Company)
            //{
            //    if (inv.Organization.CompanySize == 0)
            //    {
                    
            //    }
            //    if (string.IsNullOrEmpty(inv.Organization.TaxId))
            //    {

            //    }
            //}

            //if (inv.NewInstallation == true)
            //{
            //    //chce instalację
            //    if (!(inv.PvPower > 0))
            //    {

            //    }
            //    if (inv.Type == InstallationType.Prosument)
            //    {
            //        if (!(inv.PvYearlyProduction > 0))
            //        {

            //        }
            //    }
            //    else if (inv.Type == InstallationType.PVFarm)
            //    {
            //        if (string.IsNullOrEmpty(inv.Description))
            //        {

            //        }
            //    }
            //    else { };
            //}
            //else
            //{
            //    //mam instalację

            //    if (!(inv.PvPower > 0))
            //    {

            //    }

            //    if (!(inv.EnYearlyConsumption > 0))
            //    {

            //    }
            //}

            return base.IsValid(value, validationContext);
        }

        public override string FormatErrorMessage(string name)
        {
            return base.FormatErrorMessage(name);
        }
    }

}
