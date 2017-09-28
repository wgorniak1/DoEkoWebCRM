using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DoEko.Models.DoEko;

namespace DoEko.Controllers
{
    public class WageTypeCatalogController : Controller
    {
        private readonly DoEkoContext _context;

        public WageTypeCatalogController( DoEkoContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet()]
        public IActionResult Create()
        {
            return PartialView("_MaintainPartial", new DoEko.Models.Payroll.WageTypeDefinition());
        }
        [HttpGet()]
        public IActionResult Edit(int wageTypeDefinitionId)
        {
            var model = _context.WageTypeCatalog.Single(wt => wt.WageTypeDefinitionId == wageTypeDefinitionId);
            if (model != null)
            {
                return PartialView("_MaintainPartial", model);
            }
            else
            {
                return NotFound(wageTypeDefinitionId);
            }
        }
    }
}