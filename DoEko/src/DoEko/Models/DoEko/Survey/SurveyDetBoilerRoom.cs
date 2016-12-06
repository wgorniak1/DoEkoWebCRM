using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Survey
{
    [ComplexType]
    public class SurveyDetBoilerRoom
    {
        [Key, ForeignKey("Survey")]
        public Guid SurveyId { get; set; }
        [Display(Name = "Czy istnieje kotłownia?")]
        public Boolean RoomExists { get; set; }
        [Display(Name = "Szerokość drzwi [cm]")]
        [Range(10,300,ErrorMessage = "Proszę podać szer. między 10 a 300 cm.")]
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        public double DoorHeight { get; set; }
        [Display(Name = "Szer.[m]")]
        [Range(1, 20, ErrorMessage ="Proszę podać szer. między 1 a 20 m.")]
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        public double Width { get; set; }
        [Display(Name = "Wys.[m]")]
        [Range(1, 4, ErrorMessage = "Proszę podać wys. między 1 a 4 m.")]
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        public double Height { get; set; }
        [Display(Name = "Dług.[m]")]
        [Range(1,20,ErrorMessage = "Proszę podać dł. między 1 a 20 m.")]
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        public double Length { get; set; }
        [Display(Name = "Czy istnieje instalacja C.W.U. ")]
        public bool HWInstalled { get; set; }
        [Display(Name = "Czy istn. cyrkulacja C.W.U.")]
        public bool HWCirculationInstalled { get; set; }
        [Display(Name = "Istnieją 3 uziemione gniazda w pomieszczeniu kotłowni")]
        public bool ThreePowerSuppliesExists { get; set; }
        //Czy istnieje w kotłowni wolny przewód wentylacyjny(nieeksploatowany)
        [Display(Name = "Czy istn. wolny przewód went.?")]
        public Boolean AirVentilationExists { get; set; }
        [Display(Name = "Czy istn. reduktor ciśń. C.W.U.")]
        public bool HWPressureReductorExists { get; set; }
        [Display(Name = "Pomieszczenie suche z temp. > 0st.C")]
        public bool IsDryAndWarm { get; set; }
        [Display(Name = "Czy jest instalacja 400V?")]
        public bool HighVoltagePowerSupply { get; set; }

        [Display(Name = "Kubatura [m3]")]
        public double Volume { get; set; }

        public virtual Survey Survey { get; set; }
    }
}
