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
using System.Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DoEko.Controllers.Extensions;
using System.Runtime.Remoting.Contexts;
using Microsoft.AspNetCore.Http;
using DoEko.Models.DoEko.Survey;

namespace DoEko.Controllers
{
    [Authorize(Roles = Roles.Admin + "," + Roles.User + "," + Roles.Neo)]
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
            var contracts = _context.Contracts.Include(c => c.Project);

            if (User.IsInRole(Roles.Neo))
            {
                return View("Neo");
            }
            else
            {
                return View(await contracts.ToListAsync());
            }

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
                .Include(c => c.Investments).ThenInclude(i => i.Address).ThenInclude(a => a.Commune)
                .Include(c => c.Investments).ThenInclude(i => i.InvestmentOwners).ThenInclude(io => io.Owner)
                .Include(C => C.Investments).ThenInclude(I => I.Surveys).ThenInclude(S=>S.ResultCalculation)
                .Include(c => c.ClusterDetails).ThenInclude(cd=>cd.Commune)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }

            if (User.IsInRole(Roles.Neo))
            {
                return View("NeoDetails", contract);
            }

            if (TempData.ContainsKey("FileUploadResult"))
            {
                ViewData["FileUploadType"] = TempData["FileUploadType"];
                ViewData["FileUploadResult"] = TempData["FileUploadResult"];
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
        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
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
                }
                model.Project = _context.Projects.Include(p => p.Company).SingleOrDefault(p => p.ProjectId == ProjectId);
                model.ProjectId = model.Project.ProjectId;
                model.Company = model.Project.Company;
                model.CompanyId = model.Project.CompanyId;

                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(p => p.ProjectId != 1 && p.Status != ProjectStatus.Closed && p.Status != ProjectStatus.Completed), "ProjectId", "ShortDescription", model.Project);
                ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Name", model.Company);
                ViewData["ReturnUrl"] = ReturnUrl;

            }
            else
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(p => p.ProjectId != 1 && p.Status != ProjectStatus.Closed && p.Status != ProjectStatus.Completed), "ProjectId", "ShortDescription");
                ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Name");
                ViewData["ReturnUrl"] = ReturnUrl;
            }
            model.Number = CalculateNewNumber(model.Type, model.ContractDate);

            ViewData["StateId"] = AddressesController.GetStates(_context, 0);
            ViewData["DistrictId"] = AddressesController.GetDistricts(_context, 0, 0);
            ViewData["CommuneId"] = AddressesController.GetCommunes(_context, 0, 0, 0, 0);

            return View(model);
        }
        // POST: Contracts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create(Contract contract, string ReturnUrl = null)
        {
            if (contract.Type != ContractType.Cluster)
            {
                foreach (var item in ModelState.FindKeysWithPrefix(nameof(ClusterDetails)))
                {
                    ModelState.Remove(item.Key);
                }

                contract.ClusterDetails = null;
            }
            else
            {
                contract.ClusterDetails.CommuneId = contract.ClusterDetails.CommuneId / 10;
            }

            if (ModelState.IsValid)
            {
                contract.Status = ContractStatus.Draft;
                _context.Add(contract);
                int result = await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Projects", new { Id = contract.ProjectId });
            }

            if (contract.ProjectId != 0)
            {
                contract.Project = _context.Projects.Include(p => p.Company).SingleOrDefault(p => p.ProjectId == contract.ProjectId);
                contract.Company = contract.Project.Company;
                contract.CompanyId = contract.Project.CompanyId;

                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(p => p.ProjectId != 1 && p.Status != ProjectStatus.Closed && p.Status != ProjectStatus.Completed), "ProjectId", "ShortDescription", contract.Project);
                ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Name", contract.Company);
                ViewData["ReturnUrl"] = ReturnUrl;
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(p => p.ProjectId != 1 && p.Status != ProjectStatus.Closed && p.Status != ProjectStatus.Completed), "ProjectId", "ShortDescription");
                ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Name");
            }

            ViewData["ReturnUrl"] = ReturnUrl;
            
            ViewData["StateId"] = AddressesController.GetStates(_context, contract.ClusterDetails.StateId);
            ViewData["DistrictId"] = AddressesController.GetDistricts(_context, contract.ClusterDetails.StateId, contract.ClusterDetails.DistrictId);
            ViewData["CommuneId"] = AddressesController.GetCommunes(_context, contract.ClusterDetails.StateId, contract.ClusterDetails.DistrictId, contract.ClusterDetails.CommuneId, contract.ClusterDetails.CommuneType);

            return View(contract);
        }

        // GET: Contracts/Edit/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int? id, string ReturnUrl = null)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Company)
                .Include(c => c.Project)
                .Include(c => c.Investments)
                .SingleOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }
            ViewData["ReturnUrl"] = ReturnUrl;
            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(p=>p.ProjectId != 1).ToList(), "ProjectId", "ShortDescription", contract.ProjectId);
            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Name", contract.CompanyId);

            
            ViewData["StateId"] = AddressesController.GetStates(_context, 0);
            ViewData["DistrictId"] = AddressesController.GetDistricts(_context, 0, 0);
            ViewData["CommuneId"] = AddressesController.GetCommunes(_context, 0, 0, 0, 0);

            return View(contract);
        }

        // POST: Contracts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
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
                    return RedirectToAction("Details", new { Id = contract.ContractId });
                }
            }
            ViewData["ReturnUrl"] = ReturnUrl;
            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(p=>p.ProjectId != 1).ToList(), "ProjectId", "ShortDescription", contract.ProjectId);
            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Name", contract.CompanyId);

            ViewData["StateId"] = AddressesController.GetStates(_context, contract.ClusterDetails.StateId);
            ViewData["DistrictId"] = AddressesController.GetDistricts(_context, contract.ClusterDetails.StateId, contract.ClusterDetails.DistrictId);
            ViewData["CommuneId"] = AddressesController.GetCommunes(_context, contract.ClusterDetails.StateId, contract.ClusterDetails.DistrictId, contract.ClusterDetails.CommuneId, contract.ClusterDetails.CommuneType);


            return View(contract);
        }

        // GET: Contracts/Delete/5
        [Authorize(Roles = Roles.Admin)]
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
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int result;
            try
            {
                var contract = await _context.Contracts
                    .Include(c => c.Investments).ThenInclude(i => i.Payments)
                    .Include(c => c.Investments).ThenInclude(i => i.Surveys)
                    .Include(c => c.Investments).ThenInclude(i => i.InvestmentOwners).ThenInclude(io => io.Owner).ThenInclude(o => o.Address)
                    .Include(c => c.Investments).ThenInclude(i => i.Address)
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

        [HttpPost]
        [Authorize(Roles = "Administrator, NeoEnergetyka")]
        public IActionResult UploadNeoenergetykaResults(IFormFile neoenergetyka, int contractId)
        {

            _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

            //fields validation
            string[] allowedFileTypes = {
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                //"application/vnd.ms-excel" 
            };

            if (neoenergetyka == null)
            {
                ModelState.AddModelError(nameof(neoenergetyka), "Nieodnaleziono pliku");
                return BadRequest(ModelState);
            }
            if (!allowedFileTypes.Any(s => s == neoenergetyka.ContentType))
            {
                ModelState.AddModelError(nameof(neoenergetyka), "Nieobs³ugiwany format pliku");
                return BadRequest(ModelState);
            }

            //
            IList<string> errMessage = new List<string>();
            DataTable dt = new DataTable();

            try
            {

                //read file
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(stream: neoenergetyka.OpenReadStream(), isEditable: false))
                {
                    dt = doc.RetrieveDataTable(true);
                }

                //get survey Ids from excel table
                var surveyIds = dt.Rows.OfType<DataRow>().Where(dr => !string.IsNullOrEmpty(dr.Field<string>(1))).Select(dr => Guid.Parse(dr.Field<string>(1))).ToList();
                if (surveyIds.Count == 0)
                {
                    ModelState.AddModelError(nameof(neoenergetyka), "B³ad odczytu kolumny z ID ankiety");
                    return BadRequest(ModelState);
                }

                //get all surveys that will be updated.
                var surveys = _context.Surveys
                    .Where(s => surveyIds.Contains<Guid>(s.SurveyId))
                    .Include(s => s.Investment).ThenInclude(i=>i.Surveys)
                    .Include(s => s.ResultCalculation)
                    .ToList();

                if (surveys.Count == 0)
                {
                    ModelState.AddModelError(nameof(neoenergetyka), "Nie znaleziono w systemie ¿adnej ankiety dla danych z excela");
                    return BadRequest(ModelState);
                }

                foreach (var srv in surveys)
                {
                    try
                    {

                        var surveyExcel = dt.Rows.OfType<DataRow>().Single(dr => dr.Field<string>(1) == srv.SurveyId.ToString());
                        srv.Investment.GeoPortal = surveyExcel.Field<string>(2);

                        if (srv.ResultCalculation != null) 
                        {
                            srv.ResultCalculation.FillProperties(srv.Type, srv.GetRSEType(), surveyExcel);
                        }
                        else
                        {
                            srv.ResultCalculation = new SurveyResultCalculations(srv.Type, srv.GetRSEType(), surveyExcel);
                        }
                        //overall investment
                        if (srv.Investment.Surveys.All(s => s.ResultCalculation != null && 
                                                            s.ResultCalculation.Completed == true))
                        {
                            srv.Investment.Calculate = false;
                        }

                        if (srv.Investment.Status == InvestmentStatus.InReview)
                        {
                            srv.Investment.Status = InvestmentStatus.Initial;
                        }

                        _context.Update(srv.Investment);
                    }
                    catch (Exception exc)
                    {
                        ModelState.AddModelError("Ankieta: " + srv.SurveyId.ToString(), exc.Message);
                    }
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _context.UpdateRange(surveys);
                var result = _context.SaveChanges();                  
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("B³¹d systemu", exc.InnerException != null ? exc.InnerException.Message : exc.Message);
                return BadRequest(ModelState);
            }


            //    i += 1;
            //    if (!(i > 1))
            //    {
            //        continue;
            //    }
            //    using (var transaction = _context.Database.BeginTransaction())
            //    {
            //        try
            //        {
            //            //set line to parse
            //            uploadhelper.Record = CsvRecord;

            //            //Parse Investment Address - also check if address has already been registered in the db
            //            InvestmentAddress = uploadhelper.ParseInvestmentAddress();

            //            Investment = uploadhelper.ParseInvestment();
            //            Investment.Address = InvestmentAddress;

            //            OwnerAddress = uploadhelper.ParseOwnerAddress();
            //            Owner = (BusinessPartnerPerson)uploadhelper.ParseInvestmentOwner();

            //            if (OwnerAddress.SingleLine == InvestmentAddress.SingleLine)
            //            {
            //                Owner.Address = InvestmentAddress;
            //                OwnerAddress = null;
            //            }
            //            else
            //            {
            //                Owner.Address = OwnerAddress;
            //            }

            //            InvestmentOwner = new InvestmentOwner
            //            {
            //                Investment = Investment,
            //                InvestmentId = Investment.InvestmentId,
            //                Owner = Owner,
            //                OwnerId = Owner.BusinessPartnerId
            //            };

            //            Surveys = uploadhelper.ParseSurveys();
            //            foreach (var Survey in Surveys)
            //            {
            //                Survey.Investment = Investment;
            //                Survey.InvestmentId = Investment.InvestmentId;
            //            };
            //            if (InvestmentAddress.AddressId == 0)
            //            {
            //                _context.Add(InvestmentAddress);
            //            }
            //            //int x = _context.SaveChanges();
            //            _context.Add(Investment);
            //            //x = _context.SaveChanges();
            //            if (OwnerAddress != null && OwnerAddress.AddressId == 0)
            //            {
            //                _context.Add(OwnerAddress);
            //                //x = _context.SaveChanges();
            //            }
            //            _context.Add(Owner);
            //            //x = _context.SaveChanges();
            //            _context.Add(InvestmentOwner);
            //            //x = _context.SaveChanges();
            //            _context.AddRange(Surveys);
            //            //x = _context.SaveChanges();

            //            int x = _context.SaveChanges();

            //            transaction.Commit();
            //            //transaction.Dispose();
            //            success += 1;
            //        }
            //        catch (Exception exc)
            //        {
            //            transaction.Rollback();
            //            if (exc.Message.Contains("See the inner exception for details"))
            //                errMessage.Add("B³¹d w wierszu nr " + i.ToString() + ": " + exc.InnerException.Message.ToString());
            //            else
            //                errMessage.Add("B³¹d w wierszu nr " + i.ToString() + ": " + exc.Message.ToString());
            //        }
            //    }
            //}
            ////catch (ArgumentOutOfRangeException)
            ////catch (NullReferenceException)
            ////catch (ArgumentNullException)
            ////catch (InvalidOperationException)
            ////catch (OutOfMemoryException)
            ////catch (FormatException)

            //sr.Close();

            //if (errMessage.Count != 0)
            //{
            //    TempData["FileUploadType"] = "Import Inwestycji";
            //    TempData["FileUploadResult"] = 8;
            //    TempData["FileUploadError"] = errMessage;
            //    if (success != 0)
            //    {
            //        TempData["FileUploadResult"] = 4;
            //        TempData["FileUploadSuccess"] = "Pomyœlnie wczytano inwestycje dla " + success.ToString() + " wierszy.";
            //    }
            //}
            //else
            //{
            //    TempData["FileUploadType"] = "Import Inwestycji";
            //    TempData["FileUploadResult"] = 0;
            //    TempData["FileUploadSuccess"] = "Pomyœlnie wczytano inwestycje dla " + success.ToString() + " wierszy.";
            //}

            return Json(0);
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

        public static SelectList GetOpenContracts(DoEkoContext context, int contractId)
        {
            var list = context.Contracts
                .Include(c=>c.Project)
                .Where(c => c.Status != ContractStatus.Completed)
                .Select(c => new { Text = c.Number + " " + c.Project.ShortDescription, Value = c.ContractId })
                .OrderBy(c => c.Text)
                .ToList();

            return new SelectList(list, "Value", "Text", contractId);

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
                case ContractType.Cluster:
                    category = "K";
                    break;
                default:
                    category = "X";
                    break;
            }
            string number = _context.Contracts.Any() ? (_context.Contracts.OrderByDescending(c=>c.ContractId).Select(c=>c.ContractId).First() + 1).ToString() : 1.ToString();

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
