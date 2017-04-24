using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoEko.Models.DoEko;
using System.ComponentModel.DataAnnotations;

namespace DoEko.ViewComponents.ViewModels
{
    public class ContractDetailsViewModel : Contract
    {
        public Boolean EditMode { get; set; }
    }
}
