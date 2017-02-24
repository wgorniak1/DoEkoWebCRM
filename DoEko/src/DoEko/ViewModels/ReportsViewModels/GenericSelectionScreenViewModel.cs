using DoEko.Models.DoEko.Survey;
using DoEko.Models.DoEko;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DoEko.ViewModels.ReportsViewModels
{
    public class GenericSelectionScreenViewModel
    {
        public GenericSelectionScreenViewModel()
        {
        }

        public GenericSelectionScreenViewModel(DoEkoContext _context)
        {
           this.Initialize(_context);
        }

        public SelectList ProjectList { get; set; }
        [Display(Name = "Projekt")]
        public int ProjectId { get; set; }
        public SelectList ContractList { get; set; }
        [Display(Name ="Umowa")]
        public int ContractId { get; set; }

        private async void Initialize(DoEkoContext _context)
        {
            ProjectList = new SelectList(_context.Projects.Select(p => new SelectListItem()
            {
                Value = p.ProjectId.ToString(),
                Text = p.ShortDescription + " (" +
                       p.StartDate.ToShortDateString() + " - " +
                       p.EndDate.ToShortDateString() + ")"
            }).ToList(), "Value", "Text", null);

            ContractList = new SelectList(_context.Contracts.Select(c => new SelectListItem()
            {
                Value = c.ContractId.ToString(),
                Text = c.FullfilmentDate.HasValue ?
                        c.Number + ' ' +
                        c.ContractDate.ToShortDateString() + " - " +
                        c.FullfilmentDate.Value.ToShortDateString() + ' ' +
                        c.ShortDescription :

                        c.Number + " " +
                        c.ContractDate.ToShortDateString() + " " +
                        c.ShortDescription
            }).ToList(), "Value", "Text", null);
        }
    }
}
