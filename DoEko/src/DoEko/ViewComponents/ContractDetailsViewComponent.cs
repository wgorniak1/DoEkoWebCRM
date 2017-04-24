using DoEko.Controllers;
using DoEko.Controllers.Helpers;
using DoEko.Models.DoEko;
using DoEko.Models.Identity;
using DoEko.ViewComponents.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.ViewComponents
{
    [ViewComponent]
    public class ContractDetailsViewComponent : ViewComponent
    {
        private readonly DoEkoContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ContractDetailsViewComponent(DoEkoContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync(int contractId, bool editMode)
        {
            ContractDetailsViewModel model = 
                (ContractDetailsViewModel)(await _context.Contracts
                .SingleAsync(c => c.ContractId == contractId));

            model.EditMode = editMode;

            if (User.IsInRole(Roles.Admin))
                return View("DefaultAdmin", model);
            else
                return View("DefaultOther", model);

        }
    }
}
