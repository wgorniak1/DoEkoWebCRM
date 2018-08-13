using DoEko.Models.DoEko;
using DoEko.Models.DoEko.Survey;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.ViewModels.RSEPriceSettingsViewModels
{
    public class RSEPriceRulesViewModel : IValidatableObject
    {
        [Required]
        public int ProjectId { get; set; }// = new Project { ShortDescription = "Wartości domyślne" };
        [Required]
        public ICollection<RSEPriceRule> PriceRules { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //Sort tax rules
            PriceRules.OrderBy(f => new { f.ProjectId, f.SurveyType, f.RSEType, f.Unit, f.NumberMin, f.NumberMax });

            //Find gaps, overlappings,no min or no max range for each group of rules
            foreach (var PriceRuleGroup in PriceRules.Select((value,index) => new {
                                                    value.ProjectId,
                                                    value.SurveyType,
                                                    value.RSEType,
                                                    value.Unit,
                                                    value.NumberMin,
                                                    value.NumberMax,
                                                    value.NetPrice,
                                                    value.Multiply,
                                                    Index = index })
                                                .GroupBy(t => new {
                                                    t.ProjectId,
                                                    t.SurveyType,
                                                    t.RSEType,
                                                    t.Unit }))
            {
                //1. Check if whole range 0 - 9999999999 is set
                if (PriceRuleGroup.Min(f => f.NumberMin) != (Double)0)
                {
                    yield return new ValidationResult("Pierwszy przedział nie pokrywa całego zakresu", new List<string>() { "PriceRules["+ PriceRuleGroup.First().Index + "].numberMin" });
                }
                if (PriceRuleGroup.Max(f => f.NumberMax) != double.MaxValue)
                {
                    yield return new ValidationResult("Ostatni przedział nie pokrywa całego zakresu", new List<string>() { "PriceRules["+ PriceRuleGroup.Last().Index + "].numberMax" });
                }

                if (PriceRuleGroup.Count() > 1)
                {
                    var ruleGaps = PriceRuleGroup
                        .Zip(PriceRuleGroup.Skip(1), (first, second) => new { first.Index, first.NumberMax, second.NumberMin })
                        .Where(z => z.NumberMin != z.NumberMax);

                    foreach (var ruleGap in ruleGaps)
                    {
                        if (ruleGap.NumberMin > ruleGap.NumberMax)
                        {
                            yield return new ValidationResult("Brak pokrycia pomiędzy przedziałami", new List<string>() { "PriceRules[" + ruleGap.Index + "]" });
                        }
                        else
                        {
                            yield return new ValidationResult("Przedział pokrywa następny przedział", new List<string>() { "PriceRules[" + ruleGap.Index + "]" });
                        }

                    }

                }
            }
            //yield return ValidationResult.Success;
        }
    }
    
}
