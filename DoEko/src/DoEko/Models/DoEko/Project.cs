//using DoEkoWebCRM.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko

{
    public enum ProjectStatus
    {
        [Display(Name = "Nowy (nie rozpoczęty)")]
        New,
        [Display(Name = "W trakcie")]
        InProgress,
        [Display(Name = "Zweryfikowany")]
        Verified,
        [Display(Name = "Zakończony")]
        Completed,
        [Display(Name = "Zamknięty")]
        Closed
    }

    //public enum UEFundsLevel
    //{
    //    [Display(Name = "Brak")]
    //    NoFunds = 0,
    //    [Display(Name = "60%")]
    //    One = 60,
    //    [Display(Name = "85%")]
    //    Two = 85,
    //}

    public enum ClimateZone
    {
        [Display(Name ="Strefa I   (śr. r. tmp. zewn.: 7,7 st.C) ")]
        I = 1,
        [Display(Name ="Strefa II  (śr. r. tmp. zewn.: 7,9 st.C) ")]
        II = 2,
        [Display(Name ="Strefa III (śr. r. tmp. zewn.: 7,6 st.C) ")]
        III = 3,
        [Display(Name ="Strefa IV  (śr. r. tmp. zewn.: 6,9 st.C) ")]
        IV = 4,
        [Display(Name ="Strefa V   (śr. r. tmp. zewn.: 5,5 st.C) ")]
        V = 5
    }

    [Table(nameof(Project))]
    public class Project
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        [Display(Description = "Opis", Name = "Nr Projektu", ShortName = "Nr Projektu")]
        public int ProjectId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [StringLength(200, ErrorMessage = "Maksymalna długość opisu to {1} znaków")]
        [Display(Description = "Opis", Name = "Opis", ShortName = "Opis")]
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym.")]
        [StringLength(50, ErrorMessage = "Maksymalna dlugość pola {0} wynosi {1} znaków")]
        [Display(Description = "Krótka nazwa identyfikująca projekt", Name = "Nazwa Projektu", ShortName = "Nazwa")]
        public string ShortDescription { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Description = "Opis", Name = "Planowany Początek", ShortName = "Plan. Początek")]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Description = "Opis", Name = "Planowany Koniec", ShortName = "Plan. Koniec")]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym.")]
        [EnumDataType(typeof(ProjectStatus), ErrorMessage = "Enum Type Error")]
        [Display(Description = "Opis", Name = "Status", ShortName = "Status")]
        public ProjectStatus Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Description = "Opis", Name = "Rzeczywista data rozpoczęcia", ShortName = "Rzecz. Początek")]
        public DateTime? RealStart { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Description = "Opis", Name = "Rzeczywista data zakończenia", ShortName = "Rzecz. Koniec")]
        public DateTime? RealEnd { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym.")]
        //[EnumDataType(typeof(UEFundsLevel), ErrorMessage = "Enum Type Error")]
        [Display(Description = "Poziom dofinansowania określa procentową dopłatę do inwestycji",
                 Name = "Poziom dofinans. [%]",
                 ShortName = "Poziom dofinans.[%]",
                Prompt = "Podaj poziom dof.")]
        [Range(0, 100, ErrorMessage = "Proszę podać warość od 0 do 100")]
        public int UEFundsLevel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "",
                 Name = "Typ dofinansowania",
                 ShortName = "Brutto",
                Prompt = "Brutto?")]
        public bool GrossNetFundsType { get; set; }
        /// <summary>
        /// This is used to calculate number of pv panels for the installation on a single investment.
        /// </summary>
        [Display(Description = "Moc nominalna ogniwa PV", Name = "Moc ogniwa PV", ShortName = "Moc PV")]
        [Range(1, 999, ErrorMessage = "Proszę podać warość od 1 do 999")]
        [UIHint("Watt")]
        public double PVNominalPower { get; set; }
        /// <summary>
        /// This is used to calculate yearly production 
        /// </summary>
        [Display(Description = "Moc nominalna ogniwa PV", Name = "Moc ogniwa PV", ShortName = "Moc PV")]
        [Range(1, 50000, ErrorMessage = "Proszę podać warość od 1 do 50000")]
        public double YearlyProductionFactor { get; set; }

        [Display(Description = "Strefa klimatyczna",Name = "Strefa Klimatyczna", ShortName ="Strefa Kl.")]
        [EnumDataType(typeof(ClimateZone), ErrorMessage = "Proszę podać prawidłową wartość")]
        public ClimateZone ClimateZone { get; set; }

        [Display(Description = "Projekt nadrzędny", Name = "Projekt nadrzędny", ShortName = "Proj.Nad.")]
        public int? ParentProjectId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "Projekt nadrzędny", Name = "Projekt nadrzędny", ShortName = "Proj.Nad.")]
        public Project ParentProject { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "Pod projekty", Name = "Pod projekty", ShortName = "Pod projekty")]
        public ICollection<Project> ChildProjects { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "Lista umów dla projektu", Name = "Umowy", ShortName = "Umowy")]
        public virtual ICollection<Contract> Contracts { get; set; }
        //inwestycje bez umów

        /// <summary>
        ///
        /// </summary>
        [Required(ErrorMessage = "{0} jest polem obowiązkowym.")]
        [Display(Description = "Jednostka gospodarcza (spółka)", Name = "Jednostka Gospodarcza", ShortName = "Jedn.Gosp.")]
        public int CompanyId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Company Company { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Description = "Załączniki", Name = "Załączniki", ShortName = "Załączniki")]
        [NotMapped]
        public IList<File> Attachments { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid CreatedBy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime ChangedAt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid ChangedBy { get; set; }

    }
}
