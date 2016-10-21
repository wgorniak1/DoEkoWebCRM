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
            Survey GenericSurvey = await _context.Surveys
                .Include(s=>s.Investment).ThenInclude(i=>i.Address)
                .Include(s => s.Investment).ThenInclude(i => i.InvestmentOwners).ThenInclude(io=>io.Owner)
                .SingleOrDefaultAsync(s => s.SurveyId == Id);
            switch (GenericSurvey.Type)
            {
                case SurveyType.Solar:
                    return View("DetailsHW", (SurveyHotWater)GenericSurvey);
                case SurveyType.Fotovoltaic:
                    return View("DetailsEN", (SurveyEnergy)GenericSurvey);
                case SurveyType.HeatPump:
                    return View("DetailsCH", (SurveyCentralHeating)GenericSurvey);
                default:
                    return View(GenericSurvey);
            }
        }
    }
}