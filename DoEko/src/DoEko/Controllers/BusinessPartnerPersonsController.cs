using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.DoEko;

namespace DoEko.Controllers
{
    public class BusinessPartnerPersonsController : Controller
    {
        private readonly DoEkoContext _context;

        public BusinessPartnerPersonsController(DoEkoContext context)
        {
            _context = context;    
        }

        // GET: BusinessPartnerPersons
        public async Task<IActionResult> Index()
        {
            var doEkoContext = _context.BPPersons.Include(b => b.Address);
            return View(await doEkoContext.ToListAsync());
        }

        // GET: BusinessPartnerPersons/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var businessPartnerPerson = await _context.BPPersons.SingleOrDefaultAsync(m => m.BusinessPartnerId == id);
            if (businessPartnerPerson == null)
            {
                return NotFound();
            }

            return View(businessPartnerPerson);
        }

        // GET: BusinessPartnerPersons/Create
        public IActionResult Create(Investment NewInvestment = null, string ReturnUrl = null )
        {
            BusinessPartnerPerson BpPerson = new BusinessPartnerPerson();

            if (NewInvestment != null)
            {
                BpPerson.Address = NewInvestment.Address;
                ViewData["CountryId"] = AddressesController.GetCountries(_context, BpPerson.Address.CountryId);
                ViewData["StateId"] = AddressesController.GetStates(_context, BpPerson.Address.StateId);
                ViewData["DistrictId"] = AddressesController.GetDistricts(_context, BpPerson.Address.StateId, BpPerson.Address.DistrictId);
                ViewData["CommuneId"] = AddressesController.GetCommunes(_context, BpPerson.Address.StateId, BpPerson.Address.DistrictId, BpPerson.Address.CommuneId, BpPerson.Address.CommuneType);
            }
            else
            {
                ViewData["CountryId"] = AddressesController.GetCountries(_context, 0);
                ViewData["StateId"] = AddressesController.GetStates(_context, 0);
                ViewData["DistrictId"] = AddressesController.GetDistricts(_context, 0, 0);
                ViewData["CommuneId"] = AddressesController.GetCommunes(_context, 0, 0, 0, 0);
            }

            ViewData["ReturnUrl"] = ReturnUrl;

//            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "BuildingNo");
            return View(BpPerson);
        }

        // POST: BusinessPartnerPersons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BusinessPartnerPerson businessPartnerPerson, string ReturnUrl = null)
        {
            if (ModelState.IsValid)
            {
                businessPartnerPerson.BusinessPartnerId = Guid.NewGuid();
                _context.Add(businessPartnerPerson);
                await _context.SaveChangesAsync();
                if (!string.IsNullOrEmpty(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                else return RedirectToAction("Index");
            }
//            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "BuildingNo", businessPartnerPerson.AddressId);
            return View(businessPartnerPerson);
        }

        // GET: BusinessPartnerPersons/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var businessPartnerPerson = await _context.BPPersons.SingleOrDefaultAsync(m => m.BusinessPartnerId == id);
            if (businessPartnerPerson == null)
            {
                return NotFound();
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "BuildingNo", businessPartnerPerson.AddressId);
            return View(businessPartnerPerson);
        }

        // POST: BusinessPartnerPersons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("BusinessPartnerId,AddressId,Email,PhoneNumber,TaxId,BirthDate,FirstName,IdNumber,LastName,Pesel")] BusinessPartnerPerson businessPartnerPerson)
        {
            if (id != businessPartnerPerson.BusinessPartnerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(businessPartnerPerson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusinessPartnerPersonExists(businessPartnerPerson.BusinessPartnerId))
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
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "BuildingNo", businessPartnerPerson.AddressId);
            return View(businessPartnerPerson);
        }

        // GET: BusinessPartnerPersons/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var businessPartnerPerson = await _context.BPPersons.SingleOrDefaultAsync(m => m.BusinessPartnerId == id);
            if (businessPartnerPerson == null)
            {
                return NotFound();
            }

            return View(businessPartnerPerson);
        }

        // POST: BusinessPartnerPersons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var businessPartnerPerson = await _context.BPPersons.SingleOrDefaultAsync(m => m.BusinessPartnerId == id);
            _context.BPPersons.Remove(businessPartnerPerson);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool BusinessPartnerPersonExists(Guid id)
        {
            return _context.BPPersons.Any(e => e.BusinessPartnerId == id);
        }
    }
}
