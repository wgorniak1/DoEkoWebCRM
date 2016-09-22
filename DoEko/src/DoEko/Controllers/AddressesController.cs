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

namespace DoEkoWebCRM.Controllers
{
    public class AddressesController : Controller
    {
        private readonly DoEkoContext _context;

        public AddressesController(DoEkoContext context)
        {
            _context = context;    
        }

        // GET: Addresses
        public async Task<IActionResult> Index()
        {
            var doEkoContext = _context.Addresses.Include(a => a.Commune).Include(a => a.Country).Include(a => a.District).Include(a => a.State);
            return View(await doEkoContext.ToListAsync());
        }

        // GET: Addresses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.Addresses.SingleOrDefaultAsync(m => m.AddressId == id);
            if (address == null)
            {
                return NotFound();
            }

            return View(address);
        }

        // GET: Addresses/Create
        public IActionResult Create()
        {
            IList<Country> countries = _context.Countries.OrderBy(c => c.Name).ToList();
            countries.Insert(0, new Country { CountryId = 0, Name = "Wybierz" });
            ViewData["CountryId"] = new SelectList(countries, "CountryId", "Name",countries.SingleOrDefault(c => c.Key == "PL").CountryId);

            IList<State> states = _context.States.OrderBy(s => s.Text).ToList();
            states.Insert(0, new State { StateId = 0, Text = "Wybierz" });
            ViewData["StateId"] = new SelectList(states, "StateId", "Text");
            return View();
        }

        // POST: Addresses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AddressId,ApartmentNo,BuildingNo,City,CommuneId,CommuneType,CountryId,DistrictId,PostalCode,StateId,Street")] Address address)
        {
            if (ModelState.IsValid)
            {
                _context.Add(address);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["StateId"] = new SelectList(_context.Communes, "StateId", "Text", address.StateId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Key", address.CountryId);
            ViewData["StateId"] = new SelectList(_context.Districts, "StateId", "Text", address.StateId);
            ViewData["StateId"] = new SelectList(_context.States, "StateId", "CapitalCity", address.StateId);
            return View(address);
        }

        // GET: Addresses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.Addresses.SingleOrDefaultAsync(m => m.AddressId == id);
            if (address == null)
            {
                return NotFound();
            }
            ViewData["StateId"] = new SelectList(_context.Communes, "StateId", "Text", address.StateId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Key", address.CountryId);
            ViewData["StateId"] = new SelectList(_context.Districts, "StateId", "Text", address.StateId);
            ViewData["StateId"] = new SelectList(_context.States, "StateId", "CapitalCity", address.StateId);
            return View(address);
        }

        // POST: Addresses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AddressId,ApartmentNo,BuildingNo,City,CommuneId,CommuneType,CountryId,DistrictId,PostalCode,StateId,Street")] Address address)
        {
            if (id != address.AddressId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(address);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddressExists(address.AddressId))
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
            ViewData["StateId"] = new SelectList(_context.Communes, "StateId", "Text", address.StateId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Key", address.CountryId);
            ViewData["StateId"] = new SelectList(_context.Districts, "StateId", "Text", address.StateId);
            ViewData["StateId"] = new SelectList(_context.States, "StateId", "CapitalCity", address.StateId);
            return View(address);
        }

        // GET: Addresses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.Addresses.SingleOrDefaultAsync(m => m.AddressId == id);
            if (address == null)
            {
                return NotFound();
            }

            return View(address);
        }

        // POST: Addresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var address = await _context.Addresses.SingleOrDefaultAsync(m => m.AddressId == id);
            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool AddressExists(int id)
        {
            return _context.Addresses.Any(e => e.AddressId == id);
        }

        public JsonResult GetCommuneTypeByName(string name)
        {
            CommuneType result;
            Enum.TryParse(name, out result);
            return Json(result);
        }
        public JsonResult GetDistrictsByStateId(int id)
        {
            IList<District> districts = new List<District>();
            if (id > 0)
            {
                districts = _context.Districts
                    .Where(d => d.StateId == id)
                    .OrderBy(d => d.Text)
                    .ToList<District>();
            }
            else
            {
                districts.Insert(0, new District { DistrictId = 0, Text = "--Wybierz województwo--" });
            }
            var result = (from d in districts
                          select new
                          {
                              id = d.DistrictId,
                              name = d.Text
                          }).ToList();

            return Json(result);
        }

        public JsonResult GetCommunesByStateDistrictId(int stateid, int districtid)
        {
            IList<Commune> communes = new List<Commune>();
            if (districtid > 0)
            {
                communes = _context.Communes
                    .Where(c => c.StateId == stateid && c.DistrictId  == districtid)
                    .OrderBy(c => c.Text)
                    .ToList();
            }
            else
            {
                communes.Insert(0, new Commune { CommuneId = 0, Text = "--Wybierz powiat--" });
            }
            var result = (from d in communes
                          select new
                          {
                              id = d.CommuneId,
                              name = d.Text + " (" + d.Type.DisplayName() + ")"
                          }).ToList();
            return Json(result);
        }


    }
}
