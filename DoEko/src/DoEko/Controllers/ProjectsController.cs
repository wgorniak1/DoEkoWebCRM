using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.DoEko;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace DoEko.Controllers
{ 
    [Authorize(Roles = "Admin")]
    public class ProjectsController : Controller
    {
        private readonly DoEkoContext   _context;
        private readonly IConfiguration _configuration;

        public ProjectsController(DoEkoContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
                .Include(p => p.Company)
                .SingleOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            project.Attachments = GetAttachments(project.ProjectId);

            return View(project);
        }

        // GET: Projects/Create

        public IActionResult Create(int? ParentId, string ReturnUrl = null)
        {
            if (ParentId.HasValue)
            {
                if (!ProjectExists(ParentId.Value))
                {
                    //return RedirectToAction("Index", new { StatusMessage = 1 } );
                    return NotFound();
                }

                ViewData["ParentProjectIdDL"] = new SelectList(_context.Projects, "ProjectId", "ShortDescription", ParentId.Value);
                ViewData["ParentProjectId"]   = ParentId.Value;
            }

            //Return link
            if (!Url.IsLocalUrl(ReturnUrl))
            {
                ViewData["ReturnUrl"] = Url.Action("Index","Projects");
            }
            else
            {
                ViewData["ReturnUrl"] = ReturnUrl;
            }
            //Comp.code
            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Name", _context.Companies.FirstOrDefault().CompanyId);

            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("CompanyId,Description,EndDate,ParentProjectId,RealEnd,RealStart,ShortDescription,StartDate,UEFundsLevel")] Project project, string ReturnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ParentProjectId"] = new SelectList(_context.Projects, "ProjectId", "ShortDescription", project.ParentProjectId);
                ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Name", project.CompanyId);

                return View(project);                
            }
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

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id, string ReturnUrl = null)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var project = await _context.Projects
                .Include( p => p.Company)
                .SingleOrDefaultAsync(m => m.ProjectId == id);
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
        public async Task<IActionResult> Edit(int id, Project project, string ReturnUrl = null)
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
            ViewData["ReturnUrl"] = ReturnUrl;
            ViewData["ParentProjectId"] = new SelectList(_context.Projects, "ProjectId", "ShortDescription", project.ParentProjectId);
            return View(project);
        }

        [HttpGet]
        public async Task<IActionResult> Unlink(int? id, string ReturnUrl = null)
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

            if (!Url.IsLocalUrl(ReturnUrl))
            {
                ViewData["ReturnUrl"] = Url.Action("Index", "Projects");
            }
            else
            {
                ViewData["ReturnUrl"] = ReturnUrl;
            }

            return View(project);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Unlink")]
        public async Task<IActionResult> UnlinkConfirmed(int id, string ReturnUrl = null)
        {

            var project = await _context.Projects.SingleOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            try
            {
                project.ParentProjectId = null;
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

        private IList<File> GetAttachments(int id)
        {
            AzureStorage account = new AzureStorage(_configuration.GetConnectionString("doekostorage_AzureStorageConnectionString"));

            CloudBlobContainer Container = account.GetBlobContainer(enuAzureStorageContainerType.Project);
            var ContainerBlockBlobs = Container.ListBlobs(prefix: id.ToString(), useFlatBlobListing: true).OfType<CloudBlockBlob>();

            List<File> FileList = new List<File>();

            foreach (var BlockBlob in ContainerBlockBlobs)
            {
                FileList.Add(new File
                {
                    Name = WebUtility.UrlDecode(BlockBlob.Uri.Segments.Last()),
                    ChangedAt = BlockBlob.Properties.LastModified.Value.LocalDateTime,
                    Url = BlockBlob.Uri.ToString()
                });
            };
            return FileList;

        }
    }
}
