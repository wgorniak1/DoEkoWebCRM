using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.DoEko;
using DoEko.Models.DoEko.Addresses;
using DoEko.Models;
using DoEko.Controllers.Extensions;

namespace DoEko.Controllers
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
            //IList<Country> countries = _context.Countries.OrderBy(c => c.Name).ToList();
            //countries.Insert(0, new Country { CountryId = 0, Name = "Wybierz" });
            //ViewData["CountryId"] = new SelectList(countries, "CountryId", "Name",countries.SingleOrDefault(c => c.Key == "PL").CountryId);

            //IList<State> states = _context.States.OrderBy(s => s.Text).ToList();
            //states.Insert(0, new State { StateId = 0, Text = "Wybierz" });
            //ViewData["StateId"] = new SelectList(states, "StateId", "Text");
            ViewData["CountryId"] = GetCountries(_context, 0);
            ViewData["StateId"] = GetStates(_context, 0);
            ViewData["DistrictId"] = GetDistricts(_context, 0, 0);
            ViewData["CommuneId"] = GetCommunes(_context, 0, 0, 0, 0);
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
                address.CommuneId /= 10; //drop down related conversion
                _context.Add(address);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["CountryId"] = GetCountries(_context, address.CountryId);
            ViewData["StateId"] = GetStates(_context, address.StateId);
            ViewData["DistrictId"] = GetDistricts(_context, address.StateId, address.DistrictId);
            ViewData["CommuneId"] = GetCommunes(_context, address.StateId, address.DistrictId, address.CommuneId / 10, (CommuneType) Enum.Parse(typeof(CommuneType), (address.CommuneId % 10).ToString()));
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
            ViewData["CountryId"] = GetCountries(_context, address.CountryId);
            ViewData["StateId"] = GetStates(_context, address.StateId);
            ViewData["DistrictId"] = GetDistricts(_context, address.StateId, address.DistrictId);
            ViewData["CommuneId"] = GetCommunes(_context, address.StateId, address.DistrictId, address.CommuneId, address.CommuneType);
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
            ViewData["CountryId"] = GetCountries(_context, address.CountryId);
            ViewData["StateId"] = GetStates(_context, address.StateId);
            ViewData["DistrictId"] = GetDistricts(_context, address.StateId, address.DistrictId);
            ViewData["CommuneId"] = GetCommunes(_context, address.StateId, address.DistrictId, address.CommuneId / 10, (CommuneType)Enum.Parse(typeof(CommuneType), (address.CommuneId % 10).ToString()));
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

        public JsonResult GetCommuneTypeAJAX(int communeId)
        {
            CommuneType result = (CommuneType)Enum.ToObject(typeof(CommuneType), communeId % 10);
            
            return Json(result);
        }
        public JsonResult GetDistrictsAJAX(int id)
        {
            return Json( GetDistricts(_context, id, 0)
                            .Select(d => new { id = d.Value, name = d.Text })
                            .ToList());
        }

        public JsonResult GetCommunesAJAX(int stateId, int districtId)
        {
            return Json( GetCommunes(_context, stateId, districtId, 0,0)
                            .Select(c => new { id = c.Value, name = c.Text })
                            .ToList() );
        }

        public static SelectList GetStates(DoEkoContext context, int currentStateId)
        {
            List<State> states = context.States.OrderBy(s => s.Text).ToList<State>();
            
            return new SelectList(states, "StateId", "Text", currentStateId);
        }

        public static SelectList GetDistricts(DoEkoContext context, int currentStateId, int currentDistrictId)
        {
            IList<District> districts;
            if (currentStateId != 0)
            {
                districts = context
                    .Districts
                    .Where(d => d.StateId == currentStateId)
                    .OrderBy(d => d.Text)
                    .ToList();


            }
            else
            {
                districts = new List<District>();
            }

            return new SelectList(districts, "DistrictId", "Text", currentDistrictId);
        }

        public static SelectList GetCommunes(DoEkoContext context, int currentStateId, int currentDistrictId, int currentCommuneId, CommuneType currentCommuneType)
        {
            IList<Commune> communes;

            if (currentDistrictId != 0)
            {
                communes = context
                    .Communes.Where(c => c.StateId == currentStateId && c.DistrictId == currentDistrictId)
                    .Select(m => new Commune {
                        StateId = m.StateId,
                        DistrictId = m.DistrictId,
                        CommuneId = m.CommuneId * 10 + Convert.ToUInt16(m.Type),
                        Type = m.Type,
                        Text = m.Text
                    })
                    .OrderBy(c => c.Text)
                    .ToList();

                for (int i = 0; i < communes.Count; i++)
                    communes[i].Text = communes[i].Text + " (" + communes[i].Type.DisplayName() + ")";

            }
            else
            {
                communes = new List<Commune>();
            }

            return new SelectList(communes, "CommuneId", "Text", currentCommuneId * 10 + (int)currentCommuneType);
        }

        public static SelectList GetCountries(DoEkoContext context, int currentCountryId)
        {
            IList<Country> countries = context.Countries.OrderBy(c => c.Name).ToList();
            if (currentCountryId == 0)
            {
                currentCountryId = countries.SingleOrDefault(c => c.Key == "PL").CountryId;
            }

            return new SelectList(countries, "CountryId", "Name", currentCountryId);
        }
    }
}
