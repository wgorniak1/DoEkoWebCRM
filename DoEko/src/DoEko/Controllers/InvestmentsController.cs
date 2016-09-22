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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var investment = await _context.Investments.Include(i => i.Address)
                                                       .Include(i => i.InvestmentOwners)
                                                       .Include(i => i.Surveys)
                                                       .Include(i => i.Contract)
                                                       .SingleOrDefaultAsync(m => m.InvestmentId == id);
            if (investment == null)
            {
                return NotFound();
            }

            return View(investment);
        }

        // GET: Investments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Investments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InvestmentId,InspectionStatus,LandRegisterNo,PlotNumber,Status")] Investment investment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(investment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(investment);
        }

        // GET: Investments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var investment = await _context.Investments.Include(i => i.Address)
                                                       .Include(i => i.InvestmentOwners)
                                                       .Include(i => i.Surveys)
                                                       .Include(i => i.Contract)
                                                       .SingleOrDefaultAsync(m => m.InvestmentId == id);
            if (investment == null)
            {
                return NotFound();
            }
            return View(investment);
        }

        // POST: Investments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InvestmentId,InspectionStatus,LandRegisterNo,PlotNumber,Status")] Investment investment)
        {
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
                return RedirectToAction("Index");
            }
            return View(investment);
        }

        // GET: Investments/Delete/5
        public async Task<IActionResult> Delete(int? id)
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var investment = await _context.Investments.SingleOrDefaultAsync(m => m.InvestmentId == id);
            _context.Investments.Remove(investment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool InvestmentExists(int id)
        {
            return _context.Investments.Any(e => e.InvestmentId == id);
        }
    }
}
