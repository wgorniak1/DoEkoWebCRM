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
        [Display(Name = "Wyniki analizy techn. - Tytuł")]
        Title,
        [Display(Name = "Wyniki analizy techn. - PV")]
        PhotoVoltaic,
        [Display(Name = "Wyniki analizy techn. - Solar")]
        Solar,
        [Display(Name = "Wyniki analizy techn. - Pompa G.(CWU)")]
        HWHeatPump,
        [Display(Name = "Wyniki analizy techn. - Pompa G.(CO)")]
        CHHeatPump,
        [Display(Name = "Wyniki analizy techn. - Pompa")]
        HeatPumpAir,
        [Display(Name = "Wyniki analizy techn. - Kocioł")]
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
