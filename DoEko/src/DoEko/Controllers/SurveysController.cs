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

namespace DoEko.Controllers
{
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
                .Include(s => s.Investment).ThenInclude(i => i.InvestmentOwners).ThenInclude(io => io.Owner)
                .SingleOrDefaultAsync(s => s.SurveyId == Id);
            if (SurveyCH != null)
            {
                return View("DetailsCH", SurveyCH);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsCH(SurveyCentralHeating model, string ReturnUrl = null)
        {
            _context.SurveysCH.Update(model);
            await _context.SaveChangesAsync();
             return RedirectToAction("Details", "Investments", new { Id = model.InvestmentId });
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