using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Survey
{
    public enum RoofMaterial
    {
        [Display(Name = "Blacha Falista")]
        Material_1 = 1,
        [Display(Name = "Blacha Trapezowa")]
        Material_2,
        [Display(Name = "Blacha Na Zakładkę")]
        Material_3,
        [Display(Name = "Blacha Płaska")]
        Material_4,
        [Display(Name = "Papa Na Betonie")]
        Material_5,
        [Display(Name = "Beton")]
        Material_6,
        [Display(Name = "Dachówka Ceramiczna")]
        Material_7,
        [Display(Name = "Dachówka Betonowa")]
        Material_8,
        [Display(Name = "Karpiówka")]
        Material_9,
        [Display(Name = "Łupek")]
        Material_10,
        [Display(Name = "Eternit - Deklaracja Wymiany")]
        Material_11,
        [Display(Name = "Blachodachówka")]
        Material_12,
        [Display(Name = "Onduline")]
        Material_13,
        [Display(Name = "Gont")]
        Material_14,
        [Display(Name = "Inne")]
        Material_15
    }
    public enum RoofType
    {
        [Display(Name = "Płaski")]
        Flat,
        [Display(Name = "Jedno spadowy")]
        Pitched_1,
        [Display(Name = "Dwu spadowy")]
        Pitched_2,
        [Display(Name = "Cztero spadowy")]
        Pitched_4
    }

    [ComplexType]
    public class SurveyDetRoof
    {
        [Key]
        public Guid RoofPlaneId { get; set; }
        [ForeignKey("Survey")]
        public Guid SurveyId { get; set; }
        public virtual Survey Survey { get; set; }

        [Display(Name = "Rodzaj dachu")]
        public RoofType Type { get; set; }
        [Display(Name = "1.Wysokość budynku [m]")]
        public double BuildingHeight { get; set; }
        [Display(Name = "2.Wysokość okapu [m]")]
        public double OkapHeight { get; set; }
        [Display(Name = "3.Dł. Dachu [m]")]
        public double RoofLength { get; set; }
        [Display(Name = "4.Dł. kraw. dachu")]
        public double EdgeLength { get; set; }
        [Display(Name = "5.Dł. grzbietu [m]")]
        public double RidgeWeight { get; set; }
        [Display(Name = "6.Kąt pochylenia dachu")]
        public double SlopeAngle { get; set; }
        [Display(Name = "7a.Dług.")]
        public double Width { get; set; }
        [Display(Name = "7b.Szer.")]
        public double Length { get; set; }
        [Display(Name = "Powierzchnia")]
        public double SurfaceArea { get; set; }
        [Display(Name = "Azymut")]
        public double SurfaceAzimuth { get; set; }
        [Display(Name = "Rodzaj pokrycia")]
        public RoofMaterial RoofMaterial { get; set; }
        [Display(Name = "Czy istn. inst. odgromowa?")]
        public Boolean LightingProtection { get; set; }
        [Display(Name = "Czy są kominy?")]
        public Boolean Chimney { get; set; }
        [Display(Name = "Czy są okna dach. lub na elew.")]
        public Boolean Windows { get; set; }
        [Display(Name = "Czy są świetliki?")]
        public Boolean SkyLights { get; set; }
        [Display(Name = "Czy są instalacje pod pokryciem dachu")]
        public Boolean InstallationUnderPlane { get; set; }
    }

    //[ComplexType]
    //[Table("SurveyDetRoofPlane")]
    //public class RoofPlane
    //{
    //    [Key]
    //    public Guid RoofPlaneId { get; set; }
    //    [ForeignKey("Roof")]
    //    public Guid SurveyId { get; set; }

    //    public virtual SurveyDetRoof Roof { get; set; }
    //}
}
