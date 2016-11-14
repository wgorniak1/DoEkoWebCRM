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
        //boiler_room_exists
        //Czy istnieje pomieszczenie techniczne(kotłownia)
        [Display(Name = "Czy istnieje kotłownia?")]
        public Boolean RoomExists { get; set; }
        //Czy zbiornik c.w.u.zmieści się przez drzwi do kotłowni(80 cm)
        //boiler_height_less_than_door_height
        [Display(Name = "Drzwi kotłowni min. 80 cm")]
        public Boolean IsDoorSizeEnough { get; set; }
        //Wymiary kotłowni(wpisz w osobnych komórkach w wierszu szerokość x długość x wysokość)
        [Display(Name = "Szer.")]
        public double Width { get; set; }
        [Display(Name = "Wys.")]
        public double Height { get; set; }
        [Display(Name = "Dług.")]
        public double Length { get; set; }
        //Czy są 3 uziemione gniazda w pomieszczeniu kotłowni (jeśli nie - poinformować o konieczności ich wykonania do czasu montażu solarów)
        //boiler_room_3_power_supplies_exists
        //boiler_room_grounded_power_supply
        [Display(Name = "Czy są 3 uziemione gniazda el.?")]
        public Boolean GroundedPowerSupply { get; set; }
        //boiler_room_hp_exisits_or_available
        [Display(Name = "Czy jest instalacja 400V?")]
        public bool HighVoltagePowerSupply { get; set; }
        //Czy istnieje w kotłowni wolny przewód wentylacyjny(nieeksploatowany)
        //boiler_room_free_air_channel_exists
        [Display(Name = "Czy istn. wolny przewód went.?")]
        public Boolean AirVentilationExists { get; set; }
        //boiler_room_is_dry_warm
        [Display(Name = "Pomieszczenie suche z temp. > 0st.C")]
        public bool IsDryAndWarm { get; set; }
        //boiler_room_volume
        [Display(Name = "Kubatura [m3]")]
        public double Volume { get; set; }

        public virtual Survey Survey { get; set; }
    }
}
