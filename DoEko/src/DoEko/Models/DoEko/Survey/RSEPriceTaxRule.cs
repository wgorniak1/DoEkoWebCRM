using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Survey
{
    public class RSEPriceTaxRule
    {
        public RSEPriceTaxRule()
        {

            ProjectId = 1;
            BuildingPurpose = BuildingPurpose.Housing;
            InstallationLocalization = InstallationLocalization.Ground;
            UsableAreaMin = 0;
            UsableAreaMax = double.MaxValue;
        }

        public RSEPriceTaxRule(SurveyType surveyType, int rseType, InstallationLocalization localization, BuildingPurpose purpose, double areaMin = 0, double areaMax = double.MaxValue, short tax = 0, int projectId = 0)
        {
            this.ProjectId = projectId;
            this.SurveyType = surveyType;
            this.RSEType = rseType;
            this.InstallationLocalization = localization;
            this.BuildingPurpose = purpose;
            this.UsableAreaMin = areaMin;
            this.UsableAreaMax = areaMax;
            this.VAT = tax;
        }

        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
        [Display(Description = "Rodzaj Energii", Name = "Rodzaj Energii", ShortName = "Rodzaj Energii")]
        public SurveyType SurveyType { get; set; }
        [Display(Description = "Typ OZE", Name = "Typ OZE", ShortName = "")]
        public int RSEType { get; set; }
        [Display(Name = "Przeznaczenie budynku")]
        public BuildingPurpose BuildingPurpose { get; set; }
        [Display(Name = "Lokalizacja instalacji")]
        public InstallationLocalization InstallationLocalization { get; set; }
        [Display(Name = "Powierzchnia min.")]
        public double UsableAreaMin { get; set; }
        [Display(Name = "Powierzchnia maks.")]
        public double UsableAreaMax { get; set; }
        [Display(Name = "Stawka VAT")]
        public short VAT { get; set; }

    }
}
