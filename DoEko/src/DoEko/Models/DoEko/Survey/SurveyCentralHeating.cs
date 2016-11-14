using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Survey
{
    [Table(nameof(SurveyCentralHeating))]
    public class SurveyCentralHeating : Survey
    {
        public SurveyRSETypeCentralHeating RSEType { get; set; }

        //heat_pump_side_split_available
        //Istnienie możliwości montażu jednostki zewnętrznej powietrznej pompy ciepła typu split na ścianie budynku 		

        [Display(Name = "Możliwy montaż jedn. zewn. pompy ciepła na ścianie")]
        public bool OnWallPlacementAvailable { get; set; }
    }
}
