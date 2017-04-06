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

    public enum UEFundsLevel
    {
        [Display(Name = "Brak")]
        NoFunds = 0,
        [Display(Name = "60%")]
        One = 60,
        [Display(Name = "85%")]
        Two = 85,
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
        [EnumDataType(typeof(UEFundsLevel), ErrorMessage = "Enum Type Error")]
        [Display(Description = "Poziom dofinansowania określa procentową dopłatę do inwestycji",
                 Name = "Poziom dofinansowania",
                 ShortName = "Poziom dofinansowania",
                Prompt = "Wybierz poziom dofinasowania")]
        public UEFundsLevel UEFundsLevel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //[Display(Description = "",
        //         Name = "Dofinansowanie Netto / Brutto[x]",
        //         ShortName = "Netto / Brutto[x]",
        //        Prompt = "Netto/Brutto[x]")]
        //public bool GrossNetFundsType {get; set; }

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
        //{
        //    get
        //    {
        //        //var _array = Array.ConvertAll<String, String>(AttachmentsAsString.Split('|'), null);
        //        //return _array.ToList();
        //    }
        //    set
        //    {
        //        var _data = value;
        //        AttachmentsAsString = String.Join("|", _data.Select(p => p.ToString()).ToArray());
        //    }
        //}
        /// <summary>
        /// 
        /// </summary>
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public string AttachmentsAsString { get; set; }

    }
}
