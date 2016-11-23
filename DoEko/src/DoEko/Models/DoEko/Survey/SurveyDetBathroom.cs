using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Survey
{
    [ComplexType]
    public class SurveyDetBathroom
    {
        [Key, ForeignKey("Survey")]
        public Guid SurveyId { get; set; }
        //no_of_bath_rooms
        [Display(Name = "Ilość łazienek")]
        public short NumberOfBathrooms { get; set; }
        //bath_exists
        [Display(Name = "Czy jest wanna?")]
        public bool BathExsists { get; set; }
        //bath_volume
        [Display(Name = "Pojemność wanny")]
        public double BathVolume { get; set; }
        //shower_exists
        [Display(Name = "Czy jest prysznic?")]
        public bool ShowerExists { get; set; }
        public virtual Survey Survey { get; set; }
    }
}
