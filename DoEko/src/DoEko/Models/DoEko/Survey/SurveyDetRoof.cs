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
        Material_1,
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
        Material_14
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
        [Key, ForeignKey("Survey")]
        public Guid SurveyId { get; set; }
        //roof_type
        [Display(Name = "Rodzaj dachu")]
        public RoofType Type { get; set; }
        //roof_lighting_protection
        //Czy istnieje instalacja odgromowa?		
        [Display(Name = "Czy istn. inst. odgromowa?")]
        public Boolean LightingProtection { get; set; }
        //roof_plane_with_chimney
        //Czy są kominy na połaci na której montowana będzie instalacja?		
        [Display(Name = "Czy są kominy?")]
        public Boolean Chimney { get; set; }
        //Czy są okna na połaci lub na elewacji na której montowana będzie instalacja?
        //roof_plane_with_windows
        [Display(Name = "Czy są okna dach. lub na elew.")]
        public Boolean Windows { get; set; }
        //roof_plane_skylights
        //Czy są świetliki na dachu na którym będzie montowana instalacja?
        [Display(Name = "Czy są świetliki?")]
        public Boolean SkyLights { get; set; }
        //Czy są instalacje rozprowadzone pod pokryciem dachu?		
        //roof_plane_installation_under_plane
        [Display(Name = "Czy są instalacje pod pokryciem dachu")]
        public Boolean InstallationUnderPlane { get; set; }

        //public RoofPlane Plane1 { get; set; }
        //public RoofPlane Plane2 { get; set; }
        //public RoofPlane Plane3 { get; set; }
        //public RoofPlane Plane4 { get; set; }
        //public RoofPlane Plane5 { get; set; }
        public IList<RoofPlane> Planes { get; set; }
        public virtual Survey Survey { get; set; }
    }

    [ComplexType]
    [Table("SurveyDetRoofPlane")]
    public class RoofPlane
    {
        [Key]
        public Guid RoofPlaneId { get; set; }
        [ForeignKey("Roof")]
        public Guid SurveyId { get; set; }
        //1. Wysokość budynku[m]:		
        //roof_plane-building_height
        [Display(Name = "Wysokość budynku [m]")]
        public double BuildingHeight { get; set; }
        //2. Wysokość okapu[m]:		
        //roof_plane-okap_height
        [Display(Name = "Wysokość okapu [m]")]
        public double OkapHeight { get; set; }
        //3. Długość dachu[m]:		
        //roof_plane-roof_length
        [Display(Name = "Dł. Dachu [m]")]
        public double RoofLength { get; set; }
        //4. Długość krawędzi dachu		
        //roof_plane-roof_edge_length
        [Display(Name = "Dł. kraw. dachu")]
        public double EdgeLength { get; set; }
        //5. Długość grzbietu[m]:	
        //roof_plane-roof_ridge_length
        [Display(Name = "Dł. grzbietu [m]")]
        public double RidgeWeight { get; set; }
        //6. Kąt pochylenia dachu		
        //roof_plane-roof_slope_angle
        [Display(Name = "Kąt pochylenia dachu")]
        public double SlopeAngle { get; set; }
        //7a.Długość[m]:		
        //roof_plane-width
        [Display(Name = "Dług.")]
        public double Width { get; set; }
        //7b.Szerokość[m]:		
        //roof_plane-length
        [Display(Name = "Szer.")]
        public double Length { get; set; }
        //Pokrycie dachowe:		
        //roof_material_type
        [Display(Name = "Rodzaj pokrycia")]
        public RoofMaterial RoofMaterial { get; set; }
        //roof_plane-surface_area
        [Display(Name = "Powierzchnia")]
        public double SurfaceArea { get; set; }
        //roof_plane-surface_azimuth
        [Display(Name = "Azymut")]
        public double SurfaceAzimuth { get; set; }

        public virtual SurveyDetRoof Roof { get; set; }
    }
}
