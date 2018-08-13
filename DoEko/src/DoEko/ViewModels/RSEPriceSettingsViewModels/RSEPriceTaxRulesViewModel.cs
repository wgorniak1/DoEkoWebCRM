using DoEko.Models.DoEko;
using DoEko.Models.DoEko.Survey;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.ViewModels.RSEPriceSettingsViewModels
{
    public class RSEPriceTaxRulesViewModel : IValidatableObject
    {
        [Required]
        public int ProjectId { get; set; }// = new Project { ShortDescription = "Wartości domyślne" };
        [Required]
        public ICollection<RSEPriceTaxRule> TaxRules { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //Sort tax rules
            TaxRules.OrderBy(f => new { f.ProjectId, f.SurveyType, f.RSEType, f.UsableAreaMin, f.UsableAreaMax });

            //Find gaps, overlappings,no min or no max range for each group of rules
            foreach (var TaxRuleGroup in TaxRules.Select((value,index) => new {
                                                    value.ProjectId,
                                                    value.RSEType,
                                                    value.SurveyType,
                                                    value.InstallationLocalization,
                                                    value.BuildingPurpose,
                                                    value.UsableAreaMin,
                                                    value.UsableAreaMax,
                                                    value.VAT,
                                                    Index = index })
                                                .GroupBy(t => new {
                                                    t.ProjectId,
                                                    t.RSEType,
                                                    t.SurveyType,
                                                    t.InstallationLocalization,
                                                    t.BuildingPurpose }))
            {
                //1. Check if whole range 0 - 9999999999 is set
                if (TaxRuleGroup.Min(f => f.UsableAreaMin) != (Double)0)
                {
                    yield return new ValidationResult("Pierwszy przedział nie pokrywa całego zakresu", new List<string>() { "TaxRules["+ TaxRuleGroup.First().Index + "].usableAreaMin" });
                }
                if (TaxRuleGroup.Max(f => f.UsableAreaMax) != double.MaxValue)
                {
                    yield return new ValidationResult("Ostatni przedział nie pokrywa całego zakresu", new List<string>() { "TaxRules["+ TaxRuleGroup.Last().Index + "].usableAreaMax" });
                }

                if (TaxRuleGroup.Count() > 1)
                {
                    var ruleGaps = TaxRuleGroup
                        .Zip(TaxRuleGroup.Skip(1), (first, second) => new { first.Index, first.UsableAreaMax, second.UsableAreaMin })
                        .Where(z => z.UsableAreaMin != z.UsableAreaMax);

                    foreach (var ruleGap in ruleGaps)
                    {
                        if (ruleGap.UsableAreaMin > ruleGap.UsableAreaMax)
                        {
                            yield return new ValidationResult("Brak pokrycia pomiędzy przedziałami", new List<string>() { "TaxRules[" + ruleGap.Index + "]" });
                        }
                        else
                        {
                            yield return new ValidationResult("Przedział pokrywa następny przedział", new List<string>() { "TaxRules[" + ruleGap.Index + "]" });
                        }

                    }

                }
            }
            //yield return ValidationResult.Success;
        }
    }
    
}
