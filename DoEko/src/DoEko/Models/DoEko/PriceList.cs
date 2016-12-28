using DoEko.Models.DoEko.Addresses;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using DoEko.Models.DoEko.Survey;
using System.ComponentModel.DataAnnotations;

namespace DoEko.Models.DoEko
{
    public class PriceList
    {
        public SurveyType SurveyType { get; set; }
        public int RSEType { get; set; }
        public int StateId { get; set; }
        public int DistrictId { get; set; }
        public int CommuneId { get; set; }
        public CommuneType CommuneType { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        public virtual Commune Commune { get; set; }
    }
}
