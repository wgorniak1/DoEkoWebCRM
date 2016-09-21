using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.DoEko;
using System.Reflection;

namespace DoEko.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly DoEkoContext _context;

        public ProjectsController(DoEkoContext context)
        {
            _context = context;    
        }

        // GET: Projects
        public async Task<IActionResult> Index(int? StatusMessage)
        {
            if (StatusMessage.HasValue)
            {
                ViewData["StatusMessage"] = StatusMessage.Value;
            }

            var doEkoContext = _context.Projects.Include(p => p.ParentProject);
            return View(await doEkoContext.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.ChildProjects)
                .Include(p => p.ParentProject)
                .Include(p => p.Contracts)
                .SingleOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create

        public IActionResult Create(int? ParentId, string ReturnUrl = null)
        {
            //Parent Project
            if (ParentId.HasValue)
            {
                if (!ProjectExists(ParentId.Value))
                {
                    return RedirectToAction("Index", new { StatusMessage = 1 } );
                    //return NotFound();
                }

                ViewData["ParentProjectIdDL"] = new SelectList(_context.Projects, "ProjectId", "ShortDescription", ParentId);
                ViewData["ParentProjectId"] = ParentId;
            }
            if (!Url.IsLocalUrl(ReturnUrl))
            {
                ViewData["ReturnUrl"] = Url.Action("Index","Projects");
            }
            else
            {
                ViewData["ReturnUrl"] = ReturnUrl;
            }
            
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Description,EndDate,ParentProjectId,RealEnd,RealStart,ShortDescription,StartDate,UEFundsLevel")] Project project, string ReturnUrl = null)
        {
            if (ModelState.IsValid)
            {

                project.Status = ProjectStatus.New;
                _context.Add(project);
                await _context.SaveChangesAsync();
                if (!Url.IsLocalUrl(ReturnUrl))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return Redirect(ReturnUrl);
                }
                
            }
            ViewData["ParentProjectId"] = new SelectList(_context.Projects, "ProjectId", "ShortDescription", project.ParentProjectId);
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id, string ReturnUrl = null)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var project = await _context.Projects.SingleOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["ReturnUrl"] = ReturnUrl;
            ViewData["ParentProjectId"] = new SelectList(_context.Projects, "ProjectId", "ShortDescription", project.ParentProjectId);
            //ViewData["Status"] = new SelectList(from ProjectStatus e in Enum.GetValues(typeof(ProjectStatus)) select new { Id = e, Name = e.ToString() }, "Id", "Name", project.Status);
            //ViewData["UEFundsLevel"] = new SelectList(from UEFundsLevel e in Enum.GetValues(typeof(UEFundsLevel)) select new { Id = e, Name = e.ToString() }, "Id", "Name", project.UEFundsLevel);
            
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProjectId,Description,EndDate,ParentProjectId,RealEnd,RealStart,ShortDescription,StartDate,Status,UEFundsLevel")] Project project, string ReturnUrl = null)
        {
            if (id != project.ProjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.ProjectId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (ReturnUrl != null)
                {
                    return Redirect(ReturnUrl);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            ViewData["ParentProjectId"] = new SelectList(_context.Projects, "ProjectId", "ShortDescription", project.ParentProjectId);
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.SingleOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.SingleOrDefaultAsync(m => m.ProjectId == id);
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }
    }
}
