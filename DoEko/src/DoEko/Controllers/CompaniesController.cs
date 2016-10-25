using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.DoEko;
using DoEko.Models.DoEko.Addresses;
using DoEko.ViewModels;
using Microsoft.AspNetCore.Authorization;
using DoEko.Models.Identity;

namespace DoEko.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class CompaniesController : Controller
    {
        private readonly DoEkoContext _context;

        public CompaniesController(DoEkoContext context)
        {
            _context = context;    
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
            var doEkoContext = _context.Companies.Include(c => c.Address);
            return View(await doEkoContext.ToListAsync());
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.SingleOrDefaultAsync(m => m.CompanyId == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            ViewData["CountryId"] = AddressesController.GetCountries(_context, 0);
            ViewData["StateId"] = AddressesController.GetStates(_context, 0);
            ViewData["DistrictId"] = AddressesController.GetDistricts(_context, 0, 0);
            ViewData["CommuneId"] = AddressesController.GetCommunes(_context, 0, 0, 0, 0);

            Company model = new Company();
            model.Address = new Models.DoEko.Addresses.Address();
            model.Address.CountryId = _context.Countries.SingleOrDefault(c => c.Key == "PL").CountryId;

            return View(model);
        }

        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Address,Email,KRSId,Name,Name2,PhoneNumber,RegonId,TaxId")] Company company)
        {
            company.Address.CommuneType = (CommuneType)Enum.ToObject(typeof(CommuneType), company.Address.CommuneId % 10);
            company.Address.CommuneId /= 10;
            if (ModelState.IsValid)
            {
                _context.Add(company.Address);
                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewData["CountryId"] = AddressesController.GetCountries(_context, company.Address.CountryId);
            ViewData["StateId"] = AddressesController.GetStates(_context, company.Address.StateId);
            ViewData["DistrictId"] = AddressesController.GetDistricts(_context, company.Address.StateId, company.Address.DistrictId);
            ViewData["CommuneId"] = AddressesController.GetCommunes(_context, company.Address.StateId, company.Address.DistrictId, company.Address.CommuneId, company.Address.CommuneType);//(CommuneType)Enum.Parse(typeof(CommuneType), (company.Address.CommuneId % 10).ToString()));

            return View(company);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.Include(m => m.Address).SingleOrDefaultAsync(m => m.CompanyId == id);
            if (company == null)
            {
                return NotFound();
            }
           

            ViewData["CountryId"] = AddressesController.GetCountries(_context, company.Address.CountryId);
            ViewData["StateId"] = AddressesController.GetStates(_context, company.Address.StateId);
            ViewData["DistrictId"] = AddressesController.GetDistricts(_context, company.Address.StateId, company.Address.DistrictId);
            ViewData["CommuneId"] = AddressesController.GetCommunes(_context, company.Address.StateId, company.Address.DistrictId, company.Address.CommuneId, company.Address.CommuneType);

            company.Address.CommuneId *= 10;
            company.Address.CommuneId += (int)company.Address.CommuneType;
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CompanyId,Address,Email,KRSId,Name,Name2,PhoneNumber,RegonId,TaxId")] Company company)
        {
            if (id != company.CompanyId)
            {
                return NotFound();
            }
            company.Address.CommuneId /= 10;
            if (ModelState.IsValid)
            {
                try
                {
                   // _context.Update(company.Address);
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.CompanyId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }

            ViewData["CountryId"] = AddressesController.GetCountries(_context, company.Address.CountryId);
            ViewData["StateId"] = AddressesController.GetStates(_context, company.Address.StateId);
            ViewData["DistrictId"] = AddressesController.GetDistricts(_context, company.Address.StateId, company.Address.DistrictId);
            ViewData["CommuneId"] = AddressesController.GetCommunes(_context, company.Address.StateId, company.Address.DistrictId, company.Address.CommuneId, company.Address.CommuneType);

            return View(company);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.SingleOrDefaultAsync(m => m.CompanyId == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _context.Companies.SingleOrDefaultAsync(m => m.CompanyId == id);
            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.CompanyId == id);
        }
    }
}
