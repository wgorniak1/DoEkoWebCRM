using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.DoEko;
using DoEko.Models.DoEko.Addresses;

namespace DoEko.Controllers
{
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
            IList<Country> countries = _context.Countries.OrderBy(c => c.Name).ToList();
            countries.Insert(0, new Country { CountryId = 0, Name = "Wybierz" });
            //Country Poland = countries.SingleOrDefault(c => c.Key == "PL");

            ViewData["CountryId"] = new SelectList(countries, "CountryId", "Name", countries.SingleOrDefault(c => c.Key == "PL").CountryId);

            IList<State> states = _context.States.OrderBy(s => s.Text).ToList();
            states.Insert(0, new State { StateId = 0, Text = "Wybierz" });
            ViewData["StateId"] = new SelectList(states, "StateId", "Text");

            Company model = new Company();
            model.Address = new Models.DoEko.Addresses.Address();
            model.Address.CountryId = countries.SingleOrDefault(c => c.Key == "PL").CountryId;
            return View(model);
        }

        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Address,Email,KRSId,Name,Name2,PhoneNumber,RegonId,TaxId")] Company company)
        {
            if (ModelState.IsValid)
            {
                _context.Add(company.Address);
                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "BuildingNo", company.AddressId);
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CompanyId,AddressId,Email,KRSId,Name,Name2,PhoneNumber,RegonId,TaxId")] Company company)
        {
            if (id != company.CompanyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "BuildingNo", company.AddressId);
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
