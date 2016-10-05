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
    public class InvestmentsController : Controller
    {
        private readonly DoEkoContext _context;

        public InvestmentsController(DoEkoContext context)
        {
            _context = context;
        }

        // GET: Investments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Investments.ToListAsync());
        }

        // GET: Investments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var investment = await _context.Investments
                .Include(i=> i.Contract).ThenInclude(c=>c.Project)
                .Include( i => i.Address).ThenInclude(a=>a.Commune)
                .Include( i => i.InvestmentOwners).ThenInclude(io=>io.Owner).ThenInclude(o=>o.Address).ThenInclude(a=>a.Commune)
                .SingleOrDefaultAsync(m => m.InvestmentId == id);
            if (investment == null)
            {
                return NotFound();
            }

            return View(investment);
        }

        // GET: Investments/Create
        public IActionResult Create(int? ContractId, string ReturnUrl = null)
        {
            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                ViewData["ReturnUrl"] = ReturnUrl;
            }

            ViewData["CountryId"] = AddressesController.GetCountries(_context, 0);
            ViewData["StateId"] = AddressesController.GetStates(_context, 0);
            ViewData["DistrictId"] = AddressesController.GetDistricts(_context, 0, 0);
            ViewData["CommuneId"] = AddressesController.GetCommunes(_context, 0, 0, 0, 0);

            Investment model = new Investment();
            model.Address = new Models.DoEko.Addresses.Address();
            model.Address.CountryId = _context.Countries.SingleOrDefault(c => c.Key == "PL").CountryId;
            if (ContractId.HasValue)
            {
                model.ContractId = ContractId.Value;
                ViewData["ContractId"] = ContractId.Value;
            }

            return View(model);
        }

        // POST: Investments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Investment investment, string ReturnUrl = null)
        {
            
            investment.Address.CommuneType = (CommuneType) Enum.ToObject(typeof(CommuneType), investment.Address.CommuneId % 10);
            investment.Address.CommuneId /= 10;
            if (ModelState.IsValid)
            {
                investment.InvestmentId = Guid.NewGuid();

                _context.Add(investment.Address);
                _context.Add(investment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "InvestmentOwners",new { InvestmentId = investment.InvestmentId, ReturnUrl = ReturnUrl});
                //if (!string.IsNullOrEmpty(ReturnUrl))
                //{
                //    return Redirect(ReturnUrl);
                //}
                //else
                //return RedirectToAction("Index");
            }
            ViewData["CountryId"] = AddressesController.GetCountries(_context, investment.Address.CountryId);
            ViewData["StateId"] = AddressesController.GetStates(_context, investment.Address.StateId);
            ViewData["DistrictId"] = AddressesController.GetDistricts(_context, investment.Address.StateId, investment.Address.DistrictId);
            ViewData["CommuneId"] = AddressesController.GetCommunes(_context, investment.Address.StateId, investment.Address.DistrictId, investment.Address.CommuneId, investment.Address.CommuneType);//(CommuneType)Enum.Parse(typeof(CommuneType), (company.Address.CommuneId % 10).ToString()));

            return View(investment);

        }

        // GET: Investments/Edit/5
        public async Task<IActionResult> Edit(Guid? id, string ReturnUrl = null)
        {
            if (id == null)
            {
                return NotFound();
            }

            var investment = await _context.Investments
                .Include(i => i.Address)
                .Include(i => i.InvestmentOwners)
                .SingleOrDefaultAsync(m => m.InvestmentId == id);
            if (investment == null)
            {
                return NotFound();
            }
            ViewData["CountryId"] = AddressesController.GetCountries(_context, investment.Address.CountryId);
            ViewData["StateId"] = AddressesController.GetStates(_context, investment.Address.StateId);
            ViewData["DistrictId"] = AddressesController.GetDistricts(_context, investment.Address.StateId, investment.Address.DistrictId);
            ViewData["CommuneId"] = AddressesController.GetCommunes(_context, investment.Address.StateId, investment.Address.DistrictId, investment.Address.CommuneId, investment.Address.CommuneType);

            investment.Address.CommuneId *= 10;
            investment.Address.CommuneId += (int)investment.Address.CommuneType;
            return View(investment);
        }

        // POST: Investments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Investment investment, string ReturnUrl = null)
        {
            investment.Address.CommuneType = (CommuneType)Enum.ToObject(typeof(CommuneType), investment.Address.CommuneId % 10);
            investment.Address.CommuneId /= 10;

            if (id != investment.InvestmentId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(investment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvestmentExists(investment.InvestmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (string.IsNullOrEmpty(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                else
                    return RedirectToAction("Index");
            }

            ViewData["CountryId"] = AddressesController.GetCountries(_context, investment.Address.CountryId);
            ViewData["StateId"] = AddressesController.GetStates(_context, investment.Address.StateId);
            ViewData["DistrictId"] = AddressesController.GetDistricts(_context, investment.Address.StateId, investment.Address.DistrictId);
            ViewData["CommuneId"] = AddressesController.GetCommunes(_context, investment.Address.StateId, investment.Address.DistrictId, investment.Address.CommuneId, investment.Address.CommuneType);

            return View(investment);
        }

        [HttpPost]
        public async Task<IActionResult> AssignInspector(Guid InspectorId, Guid[] InvestmentId, string ReturnUrl)
        {
            //update
            foreach (Guid item in InvestmentId)
            {
                Investment singleInvestment = await _context.Investments.SingleOrDefaultAsync(m => m.InvestmentId == item);
                if (singleInvestment != null)
                {
                    singleInvestment.InspectorId = InspectorId;

                    _context.Update(singleInvestment);
                }
            }
            //Save changes
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {

                throw;
            }

            return Redirect(ReturnUrl);
        }

        // GET: Investments/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var investment = await _context.Investments.SingleOrDefaultAsync(m => m.InvestmentId == id);
            if (investment == null)
            {
                return NotFound();
            }

            return View(investment);
        }

        // POST: Investments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var investment = await _context.Investments.SingleOrDefaultAsync(m => m.InvestmentId == id);
            _context.Investments.Remove(investment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool InvestmentExists(Guid id)
        {
            return _context.Investments.Any(e => e.InvestmentId == id);
        }
    }
}
