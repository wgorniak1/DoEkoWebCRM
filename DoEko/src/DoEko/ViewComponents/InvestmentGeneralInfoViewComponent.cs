using DoEko.Controllers;
using DoEko.Controllers.Helpers;
using DoEko.Models.DoEko;
using DoEko.ViewComponents.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.ViewComponents
{
    [ViewComponent]
    public class InvestmentGeneralInfoViewComponent : ViewComponent
    {
        private DoEkoContext _context;

        public InvestmentGeneralInfoViewComponent(DoEkoContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(Guid? investmentId, SurveyViewMode viewMode)
        {
            var model = await _context.Investments
                .Include(i => i.Address)
                .SingleAsync(i => i.InvestmentId == investmentId);

            ViewData["InvAddrStateId"] = AddressesController.GetStates(_context, model.Address.StateId);
            ViewData["InvAddrDistrictId"] = AddressesController.GetDistricts(_context, model.Address.StateId, model.Address.DistrictId);
            ViewData["InvAddrCommuneId"] = AddressesController.GetCommunes(_context, model.Address.StateId, model.Address.DistrictId, model.Address.CommuneId, model.Address.CommuneType);

            model.Address.CommuneId = model.Address.CommuneId * 10 + (int)model.Address.CommuneType;

            return View("Default", model);
        }
    }
}
