using DoEko.Models.DoEko;
using DoEko.Models.DoEko.Survey;
using DoEko.Models.DoEko.Addresses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DoEko.ViewModels.TestViewModels
{
    public class TestViewModel
    {
        public TestViewModel()
        {

        }
        
        public bool checkbox { get; set; }

        public bool option { get; set; }
    }
}
