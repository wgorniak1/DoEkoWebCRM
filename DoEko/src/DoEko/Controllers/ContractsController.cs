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
using Microsoft.AspNetCore.Authorization;

namespace DoEko.Controllers
{
    [Authorize(Roles = Roles.Admin + "," + Roles.User)]
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
            if (TempData.ContainsKey("FileUploadResult"))
            {
                ViewData["FileUploadType"]    = TempData["FileUploadType"];
                ViewData["FileUploadResult"]  = TempData["FileUploadResult"];
                ViewData["FileUploadSuccessMessage"] = TempData["FileUploadSuccess"];
                ViewData["FileUploadErrorMessage"] = TempData["FileUploadError"];
                ViewData["FileUploadFinished"] = true;                
            } 
            else
            {
                ViewData["FileUploadFinished"] = false;
            }
            
            //Payments
            ViewData["PaymentsExists"] = _context.Payments.Where(p => p.ContractId == id && p.NotNeeded == false && p.InvestmentId == null).Any();
            // end of payments

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

            Contract model = new Contract
            {
                ContractDate = DateTime.Today,
                Status = ContractStatus.Draft,
                Type = ContractType.WithCommune
            };

            if (ProjectId.HasValue)
            {
                if (!ProjectExists(ProjectId.Value))
                {
                    return RedirectToAction("Index", new { StatusMessage = 1 });
                    //return NotFound();
                }
                model.Project = _context.Projects.Include(p=>p.Company).SingleOrDefault(p => p.ProjectId == ProjectId);
                model.ProjectId = model.Project.ProjectId;
                model.Company = model.Project.Company;
                model.CompanyId = model.Project.CompanyId;

                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(p=> p.Status != ProjectStatus.Closed && p.Status != ProjectStatus.Completed), "ProjectId", "ShortDescription", model.Project);
                ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Name", model.Company);
                ViewData["ReturnUrl"] = ReturnUrl;

            }
            else
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(p => p.Status != ProjectStatus.Closed && p.Status != ProjectStatus.Completed), "ProjectId", "ShortDescription");
                ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Name");
                ViewData["ReturnUrl"] = ReturnUrl;
            }
            model.Number = CalculateNewNumber(model.Type, model.ContractDate);
            
            return View(model);
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
                int result = await _context.SaveChangesAsync();

                //if (ReturnUrl != null)
                //{
                //    return Redirect(ReturnUrl);
                //}
                //else
                //{
                return RedirectToAction("Details","Projects", new { Id = contract.ProjectId });
                //}
                //contract = await _context.Contracts.Include(c => c.Investments).SingleOrDefaultAsync(c => c.ContractId == contract.ContractId);
            }

            if (contract.ProjectId != 0)
            {
                contract.Project = _context.Projects.Include(p => p.Company).SingleOrDefault(p => p.ProjectId == contract.ProjectId);
                contract.Company = contract.Project.Company;
                contract.CompanyId = contract.Project.CompanyId;

                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(p => p.Status != ProjectStatus.Closed && p.Status != ProjectStatus.Completed), "ProjectId", "ShortDescription", contract.Project);
                ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Name", contract.Company);
                ViewData["ReturnUrl"] = ReturnUrl;
            }
            else
            { 
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(p => p.Status != ProjectStatus.Closed && p.Status != ProjectStatus.Completed), "ProjectId", "ShortDescription");
                ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Name");
            }

            ViewData["ReturnUrl"]   = ReturnUrl;

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
            int result;
            try
            {
                var contract = await _context.Contracts
                    .Include(c=>c.Investments).ThenInclude(i=>i.Payments)
                    .Include(c=>c.Investments).ThenInclude(i=>i.Surveys)
                    .Include(c=>c.Investments).ThenInclude(i=>i.InvestmentOwners).ThenInclude(io=>io.Owner).ThenInclude(o=>o.Address)
                    .Include(c=>c.Investments).ThenInclude(i=>i.Address)
                    .SingleAsync(m => m.ContractId == id);

                foreach (var inv in contract.Investments.ToList())
                {
                    _context.Payments.RemoveRange(inv.Payments);

                    _context.InvestmentOwners.RemoveRange(inv.InvestmentOwners);
                    result = _context.SaveChanges();

                    foreach (var item in inv.InvestmentOwners)
                    {
                        _context.Addresses.Remove(item.Owner.Address);
                        _context.BusinessPartners.Remove(item.Owner);
                    }
                    
                    _context.Surveys.RemoveRange(inv.Surveys);
                    _context.Addresses.Remove(inv.Address);
                    _context.Investments.Remove(inv);

                    result = _context.SaveChanges();


                }


                _context.Contracts.Remove(contract);

                result = await _context.SaveChangesAsync();

                return Ok(result);

            }
            catch (Exception exc)
            {
                if (exc.Message.Contains("See the inner exception"))
                {
                    return BadRequest(exc.InnerException.Message);
                }
                else return BadRequest(exc.Message);
            }
        }

        [HttpGet]
        public JsonResult GetContractsAjax(int? projectId)
        {

            var cq = projectId.HasValue ? _context.Contracts.Where(c=>c.ProjectId == projectId) : _context.Contracts;

            var result = new SelectList(cq.Select(c => new SelectListItem()
            {
                Value = c.ContractId.ToString(),
                Text = c.FullfilmentDate.HasValue ?
                        c.Number + ' ' +
                        c.ContractDate.ToShortDateString() + " - " +
                        c.FullfilmentDate.Value.ToShortDateString() + ' ' +
                        c.ShortDescription :

                        c.Number + " " +
                        c.ContractDate.ToShortDateString() + " " +
                        c.ShortDescription
            }).ToList(), "Value", "Text", null);

            return Json(result);
        }

        [HttpGet]
        public IActionResult CalculateNewNumberAjax(ContractType type, DateTime contractDate)
        {
            return Json(CalculateNewNumber(type, contractDate));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="contractDate"></param>
        /// <returns></returns>
        private string CalculateNewNumber(ContractType type, DateTime contractDate)
        {
            string category = "X";

            switch (type)
            {
                case ContractType.WithCommune:
                    category = "G";
                    break;
                case ContractType.WithPerson:
                    category = "O";
                    break;
                case ContractType.Other:
                    category = "I";
                    break;
                default:
                    category = "X";
                    break;
            }
            string number = (_context.Contracts.OrderByDescending(c=>c.ContractId).Select(c=>c.ContractId).First() + 1).ToString();

            return number + "/" + category + "/" + contractDate.Month.ToString() + "/" + contractDate.Year.ToString();
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
