
using DoEko.Models.DoEko;
using DoEko.Models.DoEko.Survey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.ViewModels.RSEPriceSettingsViewModels
{
    public class EditVM
    {
        public Project Project { get; set; } = new Project { ShortDescription = "Wartości domyślne" };
        public ICollection<RSEPriceRule> PriceRules { get; set; }
        public ICollection<RSEPriceTaxRule> TaxRules { get; set; }
    }
}
