using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DoEko.Models.DoEko;
using Microsoft.AspNetCore.Authorization;
using DoEko.Models.Identity;
using DoEko.ViewModels.RSEPriceSettingsViewModels;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.DoEko.Survey;

namespace DoEko.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class RSEPriceSettingsController : Controller
    {
        private readonly DoEkoContext _context;


        public RSEPriceSettingsController(DoEkoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Edit(int id = 0, string returnUrl = "")
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["ProjectId"] = id;
            ViewData["ProjectDescription"] = id != 0 ? _context.Projects.Single(p => p.ProjectId == id).ShortDescription : "Ustawienia domyœlne";

            return View();
        }

        //private async Task<ICollection<RSEPriceTaxRule>> GetTaxRules(int projectId)
        //{
        //    ICollection<RSEPriceTaxRule> rules = _context.RSEPriceTaxRules.Where(r => r.ProjectId == projectId).ToList();

        //    if (rules.Count != 0)
        //        return rules;
            
        //    rules = projectId == 0 ? GetDefaultTaxRules() : await GetTaxRules(0);

        //    foreach (var rule in rules)
        //    {
        //        rule.ProjectId = projectId;
        //    }
        //    _context.AddRange(rules);
        //    await _context.SaveChangesAsync();
        //    return rules;
        //}
        
        //private async Task<ICollection<RSEPriceRule>> GetPriceRules(int projectId)
        //{
        //    ICollection<RSEPriceRule> rules = await _context.RSEPriceRules.Where(r => r.ProjectId == projectId).ToListAsync();
        //    if (rules.Count > 0)
        //        return rules;
            
        //    rules = projectId == 0 ? GetDefaultPriceRules() : await GetPriceRules(0);

        //    _context.AddRange(rules);
        //    await _context.SaveChangesAsync();
        //    return rules;
            
        //}


        public IActionResult Index()
        {
            return View();
        }
    }
}