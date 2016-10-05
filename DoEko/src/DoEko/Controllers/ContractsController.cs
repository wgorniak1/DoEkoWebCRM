using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.DoEko;
using Microsoft.AspNetCore.Identity;
using DoEko.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DoEko.Controllers
{
    public class ContractsController : Controller
    {
        private readonly DoEkoContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ContractsController(DoEkoContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> RoleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = RoleManager;
        }

        // GET: Contracts
        public async Task<IActionResult> Index()
        {
            var doEkoContext = _context.Contracts.Include(c => c.Project);
            return View(await doEkoContext.ToListAsync());
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id, bool editInspector = false, string returnUrl = null)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Company)
                .Include(c => c.Project)
                .Include(c => c.Investments).ThenInclude(i => i.Address).ThenInclude(a=>a.Commune)
                .Include(c => c.Investments).ThenInclude(i => i.InvestmentOwners).ThenInclude(io => io.Owner)
                .SingleOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }
            ViewData["EditInspector"] = editInspector;
            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Name", contract.CompanyId);
            ViewData["ReturnUrl"] = returnUrl;
            IdentityRole inspectorRole = await _roleManager.FindByNameAsync(Roles.Inspector);

            var users = _userManager.Users.Where(u => u.Roles.Select(r => r.RoleId).Contains(inspectorRole.Id)).ToList();

            ViewData["InspectorId"] = new SelectList(users, "Id", "UserName");
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
                Project project = _context.Projects.SingleOrDefault(p => p.ProjectId == ProjectId);

                ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Name", project.CompanyId);
            }
            else
            {
                ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Name");
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
        public async Task<IActionResult> Create([Bind("ContractDate,FullfilmentDate,Number,ProjectId,ShortDescription,Type, CompanyId")] Contract contract, string ReturnUrl = null)
        {
            if (ModelState.IsValid)
            {
                contract.Status = ContractStatus.Draft;
                _context.Add(contract);
                await _context.SaveChangesAsync();

                //if (ReturnUrl != null)
                //{
                //    return Redirect(ReturnUrl);
                //}
                //else
                //{
                    return RedirectToAction("Edit",new { Id = contract.ContractId, ReturnUrl = ReturnUrl });
                //}
                //contract = await _context.Contracts.Include(c => c.Investments).SingleOrDefaultAsync(c => c.ContractId == contract.ContractId);
            }
            ViewData["ReturnUrl"]   = ReturnUrl;
            ViewData["ProjectIdDL"] = new SelectList(_context.Projects, "ProjectId", "ShortDescription", contract.ProjectId);
            return View(contract);
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id, string ReturnUrl = null)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include( c => c.Company)
                .Include( c => c.Project)
                .Include( c => c.Investments)
                .SingleOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }
            ViewData["ReturnUrl"] = ReturnUrl;
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ShortDescription", contract.ProjectId);
            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Name", contract.CompanyId);
            return View(contract);
        }

        // POST: Contracts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract, string ReturnUrl = null)
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
                if (ReturnUrl != null)
                {
                    return Redirect(ReturnUrl);               
                }
                else
                {
                    return RedirectToAction("Details",new { Id = contract.ContractId});
                }
            }
            ViewData["ReturnUrl"] = ReturnUrl;
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ShortDescription", contract.ProjectId);
            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Name",contract.CompanyId);

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
