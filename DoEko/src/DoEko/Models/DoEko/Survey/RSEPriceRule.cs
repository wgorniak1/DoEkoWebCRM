using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Survey
{
    public enum RSEPriceRuleUnit
    {
        [Display(Name="Moc Ostateczna")]
        FinalRSEPower = 1,
        [Display(Name= "Liczba Paneli")]
        FinalSolConfig = 2
    }

    public class RSEPriceRule
    {
       
        public RSEPriceRule()
        {

        }

        public RSEPriceRule(SurveyType surveyType, int rseType, RSEPriceRuleUnit unit = RSEPriceRuleUnit.FinalRSEPower, double numberMin = 0, double numberMax = 999999.99, decimal netPrice = 0, bool multiply = false, int projectId = 0)
        {
            this.ProjectId = projectId;
            this.SurveyType = surveyType;
            this.RSEType = rseType;
            this.Unit = unit;
            this.NumberMin = numberMin;
            this.NumberMax = numberMax;
            this.NetPrice = netPrice;
            this.Multiply = multiply;
        }

        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        [Display(Description = "Rodzaj Energii", Name = "Rodzaj Energii", ShortName = "Rodzaj Energii")]
        public SurveyType SurveyType { get; set; }
        [Display(Description = "Typ OZE", Name = "Typ OZE", ShortName = "")]
        public int RSEType { get; set; }

        [Display(Description = "Badana Wartość", Name = "Wartość", ShortName = "Jedn.")]
        public RSEPriceRuleUnit Unit { get; set; }

        [Display(Description = "Od", Name = "Od", ShortName = "Od")]    
        public double NumberMin { get; set; }
        [Display(Description = "Do", Name = "Do", ShortName = "Do")]    
        public double NumberMax { get; set; }

        [Display(Description = "Cena", Name = "Cena", ShortName = "Jedn.")]
        public decimal NetPrice { get; set; }

        [Display(Description = "Cena w przeliczeniu na jednostkę badanej wartości.", Name = "Cena jednostkowa", ShortName = "Cena Jedn.")]
        public bool Multiply { get; set; }

    }
}
