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
using DoEko.Models.Identity;
using DoEko.Services;
using Microsoft.AspNetCore.Identity;

namespace DoEko.Controllers
{
    [Authorize(Roles = Roles.Admin + "," + Roles.User)]
    public class ProjectsController : Controller
    {
        private readonly DoEkoContext   _context;
        //private readonly IConfiguration _configuration;
        private readonly IFileStorage _fileStorage;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectsController(DoEkoContext context, IFileStorage fileStorage, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _fileStorage = fileStorage;
            _userManager = userManager;
            //_configuration = configuration;
        }

        // GET: Projects
        public async Task<IActionResult> Index(int? StatusMessage)
        {
            if (StatusMessage.HasValue)
            {
                ViewData["StatusMessage"] = StatusMessage.Value;
            }

            if (TempData.ContainsKey("ProjectDeleteError"))
            {
                ViewData["ProjectDeleteFinished"] = true;
                ViewData["ProjectDeleteResult"]   = false;
                ViewData["ProjectDeleteMessage"]  = TempData["ProjectDeleteError"];
            }
            else if (TempData.ContainsKey("ProjectDeleteSuccess"))
            {
                ViewData["ProjectDeleteFinished"] = true;
                ViewData["ProjectDeleteResult"]   = true;
                ViewData["ProjectDeleteMessage"]  = TempData["ProjectDeleteSuccess"];
            }
            else
            {
                ViewData["ProjectDeleteFinished"] = false;
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
        [Authorize(Roles = Roles.Admin)]
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
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create(Project project, string ReturnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ParentProjectId"] = new SelectList(_context.Projects, "ProjectId", "ShortDescription", project.ParentProjectId);
                ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Name", project.CompanyId);

                return View(project);                
            }
            //user for change fields
            _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

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
        [Authorize(Roles = Roles.Admin)]
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
        [Authorize(Roles = Roles.Admin)]
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
                    //user for change fields
                    _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

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
        [Authorize(Roles = Roles.Admin)]
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
        [Authorize(Roles = Roles.Admin)]
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
        [Authorize(Roles = Roles.Admin)]
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
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            List<BusinessPartner> owners = new List<BusinessPartner>();

            var project = await _context.Projects
                .Include(p => p.Contracts)
                    .ThenInclude(c=>c.Investments)
                        .ThenInclude(i=>i.Address)                
                .Include(p => p.Contracts)
                    .ThenInclude(c => c.Investments)
                        .ThenInclude(i => i.InvestmentOwners)
                            .ThenInclude(io=>io.Owner)
                .SingleOrDefaultAsync(m => m.ProjectId == id);

            foreach (var contract in project.Contracts)
            {
                foreach (var inv in contract.Investments)
                {
                    foreach (var io in inv.InvestmentOwners)
                    { owners.Add(io.Owner); }

                    _context.InvestmentOwners.RemoveRange(inv.InvestmentOwners);
                    _context.Addresses.Remove(inv.Address);
                }

                _context.Investments.RemoveRange(contract.Investments);

                _context.Payments.RemoveRange(_context.Payments.Where(p => p.ContractId == contract.ContractId).ToList());

            }

            _context.Contracts.RemoveRange(project.Contracts);

            _context.BusinessPartners.RemoveRange(owners);

            _context.Projects.Remove(project);

            try
            {
                Task<bool> result;

                await _context.SaveChangesAsync();

                //delete contract's attachments
                foreach (var contract in project.Contracts)
                {
                    result = _fileStorage.DeleteFolderAsync(EnuAzureStorageContainerType.Contract, contract.ContractId.ToString());
                }
                //delete project's attachments
                result = _fileStorage.DeleteFolderAsync(EnuAzureStorageContainerType.Project, project.ProjectId.ToString());


            }
            catch (Exception exc)
            {
                //IList<string> errMessage = new List<string>();
                TempData["ProjectDeleteError"] = "Wyst¹pi³ b³¹d podczas usuwania projektu: " + exc.Message;
                return RedirectToAction("Index");
            }
            TempData["ProjectDeleteSuccess"] = "Projekt " + project.ShortDescription + " zosta³ trwale usuniêty.";
            return RedirectToAction("Index");

        }

        [HttpGet]
        public JsonResult GetProjectsAjax()
        {
            return Json(new SelectList(_context.Projects.Select(p => new SelectListItem()
            {
                Value = p.ProjectId.ToString(),
                Text = p.ShortDescription + " (" +
                       p.StartDate.ToShortDateString() + " - " +
                       p.EndDate.ToShortDateString() + ")"
            }).ToList(), "Value", "Text", null));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }

        private IList<File> GetAttachments(int id)
        {
            //AzureStorage account = new AzureStorage(_configuration.GetConnectionString("doekostorage_AzureStorageConnectionString"));

            CloudBlobContainer Container = _fileStorage.GetBlobContainer(EnuAzureStorageContainerType.Project);//account.GetBlobContainer(enuAzureStorageContainerType.Project);
            var ContainerBlockBlobs = Container.ListBlobs(prefix: id.ToString(), useFlatBlobListing: true).OfType<CloudBlockBlob>();

            List<File> FileList = new List<File>(10); //each row represent different picture type

            foreach (var BlockBlob in ContainerBlockBlobs)
            {
                var name = BlockBlob.Name;
                //FileList.Add(new File
                //{
                //    Name = WebUtility.UrlDecode(BlockBlob.Uri.Segments.Last()),
                //    ChangedAt = BlockBlob.Properties.LastModified.Value.LocalDateTime,
                //    Url = BlockBlob.Uri.ToString(),
                    
                //});
            };
            return FileList;

        }
    }
}
