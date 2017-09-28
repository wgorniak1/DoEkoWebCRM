using DoEko.Models.DoEko;
using DoEko.Models.DoEko.Addresses;
using DoEko.Models.DoEko.ClusterImport;
using DoEko.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.ViewModels.API.ClusterInvestmentViewModels
{
    public class ClusterInvestmentVM
    {
        [Display(Name = "Id")]
        public Guid ClusterInvestmentId { get; set; }

        [ForeignKey("Contract")]
        [Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        [Display(Description = "", Name = "Gmina", ShortName = "")]
        public int ContractId { get; set; }

        public virtual Contract Contract { get; set; }

        [Display(Name = "Osoba fizyczna")]
        public BusinessPartnerPerson Person { get; set; }
        [Display(Name = "Organizacja")]
        public BusinessPartnerEntity Organization { get; set; }
        [Display(Name = "Istniejąca instalacja")]
        public ExistingInvestment ExistingInstallation { get; set; }
        [Display(Name = "Nowa Instalacja(Prosument)")]
        public NewInvestmentPros NewInstallationPros { get; set; }
        [Display(Name = "Nowa Instalacja(Farma)")]
        public NewInvestmentFarm NewInstallationFarm { get; set; }

        public class ExistingInvestment
        {
            [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
            [Range((double)0.1, (double)10000.0, ErrorMessage = "Prawidłowa wartość mieści się w przedziale {0} - {1}")]
            [Display(Name = "Moc instalacji")]
            public double PvPower { get; set; }

            [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
            [Range((double)0.1, (double)100000.0, ErrorMessage = "Prawidłowa wartość mieści się w przedziale {0} - {1}")]
            [Display(Name = "Roczny uzysk")]
            public double PvYearlyProduction { get; set; }
        }
        public class NewInvestmentPros
        {
            [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
            [Range((double)0.1, (double)10000.0, ErrorMessage = "Prawidłowa wartość mieści się w przedziale {0} - {1}")]
            [Display(Name = "Proponowana moc")]
            public double PvPower { get; set; }

            [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
            [Range((double)0.1, (double)50000.0, ErrorMessage = "Prawidłowa wartość mieści się w przedziale {0} - {1}")]
            [Display(Name ="Roczne zużycie En. elektr.")]
            public double EnYearlyConsumption { get; set; }
        }
        public class NewInvestmentFarm
        {
            [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
            [Range((double)0.1, (double)10000.0, ErrorMessage = "Prawidłowa wartość mieści się w przedziale {0} - {1}")]
            [Display(Name ="Proponowana moc")]
            public double PvPower { get; set; }

            [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
            [MaxLength(1000, ErrorMessage = "Maksymalna długość pola wynosi {0}")]
            [Display(Name ="Stan zaawansowania prac")]
            public string Description { get; set; }
        }
    }


}
