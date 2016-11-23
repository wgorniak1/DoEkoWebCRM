using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.DoEko;
using DoEko.Models.Identity;
using Microsoft.AspNetCore.Authorization;

namespace DoEko.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class SettingsController : Controller
    {
        private readonly DoEkoContext _context;

        public SettingsController(DoEkoContext context)
        {
            _context = context;    
        }

        // GET: Settings
        public async Task<IActionResult> Index()
        {
            return View(await _context.Settings.ToListAsync());
        }

        // GET: Settings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var controlParameter = await _context.Settings.SingleOrDefaultAsync(m => m.Id == id);
            if (controlParameter == null)
            {
                return NotFound();
            }

            return View(controlParameter);
        }

        // GET: Settings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Settings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Name,Value")] ControlParameter controlParameter)
        {
            if (ModelState.IsValid)
            {
                _context.Add(controlParameter);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(controlParameter);
        }

        // GET: Settings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var controlParameter = await _context.Settings.SingleOrDefaultAsync(m => m.Id == id);
            if (controlParameter == null)
            {
                return NotFound();
            }
            return View(controlParameter);
        }

        // POST: Settings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Name,Value")] ControlParameter controlParameter)
        {
            if (id != controlParameter.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(controlParameter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ControlParameterExists(controlParameter.Id))
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
            return View(controlParameter);
        }

        // GET: Settings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var controlParameter = await _context.Settings.SingleOrDefaultAsync(m => m.Id == id);
            if (controlParameter == null)
            {
                return NotFound();
            }

            return View(controlParameter);
        }

        // POST: Settings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var controlParameter = await _context.Settings.SingleOrDefaultAsync(m => m.Id == id);
            _context.Settings.Remove(controlParameter);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ControlParameterExists(int id)
        {
            return _context.Settings.Any(e => e.Id == id);
        }
    }
}
