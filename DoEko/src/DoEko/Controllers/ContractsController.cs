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
    public class ContractsController : Controller
    {
        private readonly DoEkoContext _context;

        public ContractsController(DoEkoContext context)
        {
            _context = context;    
        }

        // GET: Contracts
        public async Task<IActionResult> Index()
        {
            var doEkoContext = _context.Contracts.Include(c => c.Project);
            return View(await doEkoContext.ToListAsync());
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.SingleOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // GET: Contracts/Create
       
        public IActionResult Create(int? ProjectId, string ReturnUrl = null)
        {
            if (ProjectId.HasValue)
            {
                if (!ProjectExists(ProjectId.Value))
                {
                    return RedirectToAction("Index", new { StatusMessage = 1 });
                    //return NotFound();
                }
                ViewData["ProjectId"] = ProjectId;
            }

            ViewData["ReturnUrl"] = ReturnUrl;
            ViewData["ProjectIdDL"] = new SelectList(_context.Projects, "ProjectId", "ShortDescription");
            return View();
        }

        // POST: Contracts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContractDate,FullfilmentDate,Number,ProjectId,ShortDescription,Type")] Contract contract, string ReturnUrl = null)
        {
            if (ModelState.IsValid)
            {
                contract.Status = ContractStatus.Draft;
                _context.Add(contract);
                await _context.SaveChangesAsync();

                if (ReturnUrl != null)
                {
                    return Redirect(ReturnUrl);
                }
                else
                {
                    return RedirectToAction("Index");
                }
                //contract = await _context.Contracts.Include(c => c.Investments).SingleOrDefaultAsync(c => c.ContractId == contract.ContractId);
            }
            ViewData["ReturnUrl"]   = ReturnUrl;
            ViewData["ProjectIdDL"] = new SelectList(_context.Projects, "ProjectId", "ShortDescription", contract.ProjectId);
            return View(contract);
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.SingleOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ShortDescription", contract.ProjectId);
            return View(contract);
        }

        // POST: Contracts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContractId,ContractDate,FullfilmentDate,Number,ProjectId,ShortDescription,Status,Type")] Contract contract)
        {
            if (id != contract.ContractId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contract);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.ContractId))
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
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ShortDescription", contract.ProjectId);
            return View(contract);
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.SingleOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.SingleOrDefaultAsync(m => m.ContractId == id);
            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.ContractId == id);
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(p => p.ProjectId == id); 
        }
    }
}
