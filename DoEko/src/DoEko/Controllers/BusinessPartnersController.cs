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
    public class BusinessPartnersController : Controller
    {
        private readonly DoEkoContext _context;

        public BusinessPartnersController(DoEkoContext context)
        {
            _context = context;    
        }

        // GET: BusinessPartners
        public async Task<IActionResult> Index()
        {
            var doEkoContext = _context.BusinessPartners.Include(b => b.Address);
            return View(await doEkoContext.ToListAsync());
        }

        // GET: BusinessPartners/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var businessPartner = await _context.BusinessPartners.SingleOrDefaultAsync(m => m.BusinessPartnerId == id);
            if (businessPartner == null)
            {
                return NotFound();
            }

            return View(businessPartner);
        }

        // GET: BusinessPartners/Create
        public IActionResult Create()
        {
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "BuildingNo");
            return View();
        }

        // POST: BusinessPartners/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BusinessPartnerId,AddressId,Email,PhoneNumber,TaxId")] BusinessPartner businessPartner)
        {
            if (ModelState.IsValid)
            {
                businessPartner.BusinessPartnerId = Guid.NewGuid();
                _context.Add(businessPartner);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "BuildingNo", businessPartner.AddressId);
            return View(businessPartner);
        }

        // GET: BusinessPartners/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var businessPartner = await _context.BusinessPartners.SingleOrDefaultAsync(m => m.BusinessPartnerId == id);
            if (businessPartner == null)
            {
                return NotFound();
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "BuildingNo", businessPartner.AddressId);
            return View(businessPartner);
        }

        // POST: BusinessPartners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("BusinessPartnerId,AddressId,Email,PhoneNumber,TaxId")] BusinessPartner businessPartner)
        {
            if (id != businessPartner.BusinessPartnerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(businessPartner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusinessPartnerExists(businessPartner.BusinessPartnerId))
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
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "BuildingNo", businessPartner.AddressId);
            return View(businessPartner);
        }

        // GET: BusinessPartners/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var businessPartner = await _context.BusinessPartners.SingleOrDefaultAsync(m => m.BusinessPartnerId == id);
            if (businessPartner == null)
            {
                return NotFound();
            }

            return View(businessPartner);
        }

        // POST: BusinessPartners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var businessPartner = await _context.BusinessPartners.SingleOrDefaultAsync(m => m.BusinessPartnerId == id);
            _context.BusinessPartners.Remove(businessPartner);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool BusinessPartnerExists(Guid id)
        {
            return _context.BusinessPartners.Any(e => e.BusinessPartnerId == id);
        }
    }
}
