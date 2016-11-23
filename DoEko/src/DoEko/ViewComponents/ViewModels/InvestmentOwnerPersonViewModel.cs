using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoEko.Models.DoEko;
using System.ComponentModel.DataAnnotations;

namespace DoEko.ViewComponents.ViewModels
{
    public class InvestmentOwnerPersonViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid InvestmentId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public BusinessPartnerPerson Owner { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Name ="Właściciel jest inwestorem")]
        public Boolean Sponsor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Name ="Typ własności")]
        public OwnershipType OwnershipType { get; set; }
        [Display(Name = "Adres taki sam jak inwestycji?")]
        public bool SameAddress { get; set; }

        public int OwnerNumber { get; set; }
        public int OwnerTotal { get; set; }
    }
}
