using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DoEko.Models.DoEko;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DoEko.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.DoEko.Survey;
using DoEko.ViewModels.SurveyViewModels;
using DoEko.Models.DoEko.Addresses;

namespace DoEko.Controllers
{
    [Authorize()]
    public class SurveysController : Controller
    {
        private readonly DoEkoContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SurveysController(DoEkoContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> RoleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = RoleManager;
        }
        
        [HttpGet]
        public async Task<IActionResult> Details(Guid Id, string ReturnUrl = null)
        {

            SurveyHotWater SurveyHW = await _context.SurveysHW
                .Include(s => s.Investment).ThenInclude(i => i.Address)
                .Include(s => s.Investment).ThenInclude(i => i.InvestmentOwners).ThenInclude(io => io.Owner)
                .SingleOrDefaultAsync(s => s.SurveyId == Id);
            if (SurveyHW != null)
            {
                return View("DetailsHW", SurveyHW);
            }

            SurveyEnergy SurveyEN = await _context.SurveysEN
                .Include(s => s.Investment).ThenInclude(i => i.Address)
                .Include(s => s.Investment).ThenInclude(i => i.InvestmentOwners).ThenInclude(io => io.Owner)
                .SingleOrDefaultAsync(s => s.SurveyId == Id);
            if (SurveyEN != null)
            {
                return View("DetailsEN", SurveyEN);
            }

            SurveyCentralHeating SurveyCH = await _context.SurveysCH
                .Include(s => s.Investment).ThenInclude(i => i.Address)
                .Include(s => s.Investment).ThenInclude(i => i.InvestmentOwners).ThenInclude(io => io.Owner).ThenInclude(o => o.Address)
                .SingleOrDefaultAsync(s => s.SurveyId == Id);
            if (SurveyCH != null)
            {
                DetailsCHViewModel model = new DetailsCHViewModel(SurveyCH);

                //ViewData["InvAddrCountryId"] = AddressesController.GetCountries(_context, InvestmentVM.Address.CountryId);
                ViewData["InvAddrStateId"] = AddressesController.GetStates(_context, model.InvestmentAddress.StateId);
                ViewData["InvAddrDistrictId"] = AddressesController.GetDistricts(_context, model.InvestmentAddress.StateId, model.InvestmentAddress.DistrictId);
                ViewData["InvAddrCommuneId"] = AddressesController.GetCommunes(_context, model.InvestmentAddress.StateId, model.InvestmentAddress.DistrictId, model.InvestmentAddress.CommuneId, model.InvestmentAddress.CommuneType);

                for (int i = 0; i < model.Owners.Count(); i++)
                {
                    Address _adr = model.Owners.ElementAt(i).Address;
                    ViewData["OwnerStateId_" + i.ToString()] = AddressesController.GetStates(_context, _adr.StateId);
                    ViewData["OwnerDistrictId_" + i.ToString()] = AddressesController.GetDistricts(_context, _adr.StateId, _adr.DistrictId);
                    ViewData["OwnerCommuneId_" + i.ToString()] = AddressesController.GetCommunes(_context, _adr.StateId, _adr.DistrictId, _adr.CommuneId, _adr.CommuneType);

                }

                return View("DetailsCH", model);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsCH(DetailsCHViewModel model, string ReturnUrl = null)
        {
            //Adres gmina i typ sklejone w jednym polu        
            model.InvestmentAddress.CommuneType = (CommuneType)Enum.ToObject(typeof(CommuneType), model.InvestmentAddress.CommuneId % 10);
            model.InvestmentAddress.CommuneId /= 10;
            for (int i = 0; i < model.Owners.Count; i++)
            {
                model.Owners[i].Address.CommuneType = (CommuneType)Enum.ToObject(typeof(CommuneType), model.Owners[i].Address.CommuneId % 10);
                model.Owners[i].Address.CommuneId /= 10;
            }

            SurveyCentralHeating survey = _context.SurveysCH.SingleOrDefault(s => s.SurveyId == model.Survey.SurveyId);
            
            switch (survey.Status)
            {
                case SurveyStatus.New:
                    survey.Status = SurveyStatus.Draft;
                    break;
                case SurveyStatus.Draft:
                    break;
                case SurveyStatus.Approval:
                    break;
                case SurveyStatus.Rejected:
                    survey.Status = SurveyStatus.Draft;
                    break;
                case SurveyStatus.Approved:
                    break;
                case SurveyStatus.Cancelled:
                    break;
                default:
                    break;
            }

            //_context.SurveysCH.Update(model);
            await _context.SaveChangesAsync();
             return RedirectToAction("Details", "Investments", new { Id = model.Survey.InvestmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsEN(SurveyEnergy model, string ReturnUrl = null)
        {
            _context.SurveysEN.Update(model);
            await _context.SaveChangesAsync();
             return RedirectToAction("Details", "Investments", new { Id = model.InvestmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsHW(SurveyHotWater model, string ReturnUrl = null)
        {
            _context.SurveysHW.Update(model);
            await _context.SaveChangesAsync();
            //if (User.IsInRole("Inspector"))
            //{
            //    return RedirectToAction("List");
            //}
             return RedirectToAction("Details", "Investments", new { Id = model.InvestmentId });
        }

    }
}