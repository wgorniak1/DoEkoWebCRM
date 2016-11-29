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
    public class InvestmentOwnerDataViewComponent : ViewComponent
    {
        private DoEkoContext _context;

        public InvestmentOwnerDataViewComponent(DoEkoContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(Guid? investmentId, Guid? ownerId, SurveyViewMode viewMode)
        {
            InvestmentOwnerPersonViewModel model = new InvestmentOwnerPersonViewModel();
            List<InvestmentOwner> IOList = await _context.InvestmentOwners
                .Include(i=>i.Investment)
                .ThenInclude(ii=>ii.Address)
                .Where(i => i.InvestmentId == investmentId).ToListAsync();

            InvestmentOwner IO;

            if (ownerId != null)
            {
                //Edit existing owner
                IO = IOList.Single(i => i.OwnerId == ownerId);

                model.OwnerNumber   = IOList.IndexOf(IO) + 1;
                model.Owner         = _context.BPPersons.Include(p=>p.Address).Single(p => p.BusinessPartnerId == IO.OwnerId);
                model.OwnerTotal    = IOList.Count;
                model.InvestmentId  = IO.InvestmentId;
                model.OwnershipType = IO.OwnershipType;
                model.SameAddress   = IO.Investment.AddressId == IO.Owner.AddressId ? true : false;
            }
            else
            {
                //Create new owner
                IO = IOList.Last();

                model.OwnerNumber = IOList.Count + 1;
                model.Owner = new BusinessPartnerPerson() { Address = IO.Investment.Address };
                model.OwnerTotal = IOList.Count + 1;
                model.InvestmentId = IO.InvestmentId;
                model.SameAddress = true;
            }

            ViewData["OwnAddrStateId"] = AddressesController.GetStates(_context, model.Owner.Address.StateId);
            ViewData["OwnAddrDistrictId"] = AddressesController.GetDistricts(_context, model.Owner.Address.StateId, model.Owner.Address.DistrictId);
            ViewData["OwnAddrCommuneId"] = AddressesController.GetCommunes(_context, model.Owner.Address.StateId, model.Owner.Address.DistrictId, model.Owner.Address.CommuneId, model.Owner.Address.CommuneType);

            model.Owner.Address.CommuneId = model.Owner.Address.CommuneId * 10 + (int)model.Owner.Address.CommuneType;

            return View("OwnerPerson", model);
        }
    }
}
