using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoEko.Models.DoEko.Survey;
using System.ComponentModel.DataAnnotations;
using DoEko.Models.DoEko;

namespace DoEko.ViewComponents.ViewModels
{
    public enum OfficeTemplateType
    {
        [Display(Name = "(Raport Dla Mieszk.)|Tytuł")]
        Title,
        [Display(Name = "(Raport Dla Mieszk.)|PV")]
        PhotoVoltaic,
        [Display(Name = "(Raport Dla Mieszk.)|Solar")]
        Solar,
        [Display(Name = "(Raport Dla Mieszk.)|Pompa Ciepła (CWU)")]
        HWHeatPump,
        [Display(Name = "(Raport Dla Mieszk.)|Pompa Ciepła G(CO)")]
        CHHeatPump,
        [Display(Name = "(Raport Dla Mieszk.)|Pompa Ciepła P(CO)")]
        HeatPumpAir,
        [Display(Name = "(Raport Dla Mieszk.)|Kocioł")]
        PelletBoiler
    }

    public class ReportTemplateViewModel
    {
        public ReportTemplateViewModel()
        {
            Templates = new Dictionary<OfficeTemplateType, OfficeTemplate>();

            foreach (var item in Enum.GetValues(typeof(OfficeTemplateType)))
            {                
                Templates.Add((OfficeTemplateType)item, new OfficeTemplate());
            }
        }

        [Display(Name ="Szablony dokumentów")]
        public Dictionary<OfficeTemplateType,OfficeTemplate> Templates { get; set; }
    }

    public class OfficeTemplate
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
