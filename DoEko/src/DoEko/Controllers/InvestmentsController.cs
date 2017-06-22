using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.DoEko;
using DoEko.Models.DoEko.Addresses;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.IO;
using DoEko.ViewModels.InvestmentViewModels;
using Microsoft.AspNetCore.Identity;
using DoEko.Models.Identity;
using DoEko.Models;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using DoEko.Controllers.Helpers;
using DoEko.Controllers.Extensions;
using DoEko.Services;
using DoEko.Models.DoEko.Survey;

namespace DoEko.Controllers
{
    [Authorize()]
    public class InvestmentsController : Controller
    {
        private readonly DoEkoContext _context;
        private readonly IFileStorage _fileStorage;
        private readonly UserManager<ApplicationUser> _userManager;

        public InvestmentsController(DoEkoContext context, IFileStorage fileStorage, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _fileStorage = fileStorage;
            _userManager = userManager;    
        }

        // GET: Investments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Investments.ToListAsync());
        }
        [HttpGet]
        public IActionResult ListAjax(
            //InvestmentListFilter filter, 
            //InvestmentListPaging paging,
            //InvestmentListSorting sorting 
            //)
            int?  page,
            int?  pageSize,
            Guid? userId,
            //int?  stateId,
            //int?  districtId,
            //int?  communeType,
            //InvestmentStatus status,
            int?  status,
            int? projectId,
            int? contractId,
            bool filterByInspector,
            string communeId = null,
            string city = null,
            string freeSearch = null,
            string sortBy = null)
        {

            var model = new InvestmentListViewModel
            {
                Filtering = new InvestmentListFilter
                {
                    City = city,
                    CommuneId = communeId,
                    ContractId = contractId.HasValue ? contractId.Value : 0,
                    FreeText = freeSearch,
                    ProjectId = projectId.HasValue ? projectId.Value : 0,
                    Status = status.HasValue ? (InspectionStatus)status.Value : 0,
                    UserId = userId.HasValue ? userId.Value : Guid.Empty,
                    FilterByInspector = filterByInspector
                },
                Paging = new InvestmentListPaging
                {
                    CurrentNumber = page.HasValue ? page.Value : 1,
                    PageSize = pageSize.HasValue ? (PageSize)pageSize.Value : PageSize.ps_25
                },
                Sorting = new InvestmentListSorting
                {
                    sortBy = string.IsNullOrEmpty(sortBy) ? nameof(Address) + InvestmentListSorting.postfixUp : sortBy
                }
            };

            return ViewComponent("InvestmentList",new { model = model });
        }
        // GET: My Investments
        [HttpGet]
        public async Task<IActionResult> List(
            int? page,
            int? pageSize,
            //Guid? userId,
            //int? contractId,
            int? communeId,
            InvestmentStatus? status,
            string searchPhrase = null,
            string sortBy = null
            )
        {
            InvestmentListViewModel model = new InvestmentListViewModel();
            //
            model.Filtering.FilterByInspector = true;
            model.Filtering.UserId = Guid.Parse(_userManager.GetUserId(User));
            model.Paging.CurrentNumber = 1;
            model.Paging.PageSize = PageSize.ps_25;
            model.Sorting.sortBy = nameof(Investment.Address) + InvestmentListSorting.postfixUp;
            
            //needed to initiate table display, see view implementation
            model.List = await _context.InvestmentOwners
                .Where(i => i.Investment.InspectorId == model.Filtering.UserId)
                .Select(i => new InvestmentOwner { InvestmentId = i.InvestmentId })
                .Take(1)
                .ToListAsync();
            //needed to build filtering dropdowns
            List <Address> list = await _context.Investments
                .Where(i => i.InspectorId == model.Filtering.UserId)
                .Include(i => i.Address).ThenInclude(a=>a.Commune)
                .Include(i => i.InvestmentOwners).ThenInclude(io => io.Owner)
                .Select(l => new Address {
                    Commune = l.Address.Commune,
                    City = l.Address.City,
                    CommuneId = l.Address.CommuneId,
                    CommuneType = l.Address.CommuneType,
                    DistrictId = l.Address.DistrictId,
                    StateId = l.Address.StateId })    
                .Distinct()
                .ToListAsync();
            //filtering drop downs
            ViewData["CommuneList"] = new SelectList(
                list.Select(l => new {
                    Key = l.StateId.ToString() + "_" + 
                          l.DistrictId.ToString() + "_" +
                          l.CommuneId.ToString() + "_" + l.CommuneType.ToString(),
                    Value = l.Commune.FullName })
                    .Distinct().ToList(), "Key", "Value");

            ViewData["CityList"] = new SelectList(
                list.Select(l => new {
                    Key = l.City,
                    Value = l.City })
                    .Distinct().ToList(), "Key", "Value");

            List<SelectListItem> statusList = new List<SelectListItem>();
            //foreach (var enumItem in Enum.GetValues(typeof(InspectionStatus)).Cast<InspectionStatus>())
            //    statusList.Add(new SelectListItem() { Value = ((int)enumItem).ToString(), Text = enumItem.DisplayName() });
            //ViewData["StatusList"] = new SelectList(statusList,"Value","Text",null);

            return View(model);
        }
        // GET: Unassigned Investments
        [HttpGet]
        public async Task<IActionResult> ListUnassigned(
                                             int? page,
                                             int? pageSize,
                                             int? communeId,
                                             InvestmentStatus? status,
                                             string searchPhrase = null,
                                             string sortBy = null)
        {

            InvestmentListViewModel model = new InvestmentListViewModel();
            model.Filtering.FilterByInspector = true;
            model.Filtering.UserId = Guid.Empty;
            //this is the only flag indicating whether assigned or not investments are displayed
            model.Paging.CurrentNumber = 1;
            model.Paging.PageSize = PageSize.ps_25;
            model.Sorting.sortBy = nameof(Investment.Address) + InvestmentListSorting.postfixUp;

            //needed to initiate table display, see view implementation
            model.List = await _context.InvestmentOwners
                .Where(i => i.Investment.InspectorId == null)
                .Select(i => new InvestmentOwner { InvestmentId = i.InvestmentId })
                .Take(1)
                .ToListAsync();
            //needed to build filtering dropdowns
            List<Address> list = await _context.Investments
                .Where(i => i.InspectorId == null)
                .Include(i => i.Address).ThenInclude(a => a.Commune)
                .Include(i => i.InvestmentOwners).ThenInclude(io => io.Owner)
                .Select(l => new Address
                {
                    Commune = l.Address.Commune,
                    City = l.Address.City,
                    CommuneId = l.Address.CommuneId,
                    CommuneType = l.Address.CommuneType,
                    DistrictId = l.Address.DistrictId,
                    StateId = l.Address.StateId
                })
                .Distinct()
                .ToListAsync();
            //filtering drop downs
            ViewData["CommuneList"] = new SelectList(
                list.Select(l => new {
                    Key = l.StateId.ToString() + "_" +
                          l.DistrictId.ToString() + "_" +
                          l.CommuneId.ToString() + "_" + l.CommuneType.ToString(),
                    Value = l.Commune.FullName
                })
                    .Distinct().ToList(), "Key", "Value");

            ViewData["CityList"] = new SelectList(
                list.Select(l => new {
                    Key = l.City,
                    Value = l.City
                })
                    .Distinct().ToList(), "Key", "Value");

            return View(model);
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
                .Include( i => i.Surveys)
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
      
            CreateViewModel ViewModel = ContractId.HasValue ? new CreateViewModel(_context.Contracts.SingleOrDefault(c => c.ContractId == ContractId),
                                                                _context.Countries.SingleOrDefault(c => c.Key == "PL").CountryId) 
                                                            : new CreateViewModel(_context.Countries.SingleOrDefault(c => c.Key == "PL").CountryId);
            return View(ViewModel);
        }

        // POST: Investments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel InvestmentVM, string ReturnUrl = null)
        {
            _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

            Investment investment = InvestmentVM.AsBase();

            //These fields are required, but filled only On survey
            ModelState.Remove("CompletionYear");
            ModelState.Remove("HeatedArea");
            ModelState.Remove("UsableArea");
            ModelState.Remove("TotalArea");

            if (ModelState.IsValid)
            {
                investment.InvestmentId = Guid.NewGuid();

                investment.Status = InvestmentStatus.Initial;
                investment.InspectionStatus = InspectionStatus.NotExists;


                _context.Add(investment.Address);
                _context.Add(investment);

                if (InvestmentVM.SurveyEE)
                {
                    SurveyEnergy Survey = new SurveyEnergy
                    {
                        SurveyId = Guid.NewGuid(),
                        InvestmentId = investment.InvestmentId,
                        Type = SurveyType.Energy,
                        RSEType = InvestmentVM.RSETypeEE,
                        Status = SurveyStatus.New
                    };
                    _context.Add(Survey);
                }

                if (InvestmentVM.SurveyCH)
                {
                    SurveyCentralHeating Survey = new SurveyCentralHeating
                    {
                        SurveyId = Guid.NewGuid(),
                        InvestmentId = investment.InvestmentId,
                        Type = SurveyType.CentralHeating,
                        RSEType = InvestmentVM.RSETypeCH,
                        Status = SurveyStatus.New
                    };
                    _context.Add(Survey);
                }

                if (InvestmentVM.SurveyHW)
                {
                    SurveyHotWater Survey = new SurveyHotWater
                    {
                        SurveyId = Guid.NewGuid(),
                        InvestmentId = investment.InvestmentId,
                        Type = SurveyType.HotWater,
                        RSEType = InvestmentVM.RSETypeHW,
                        Status = SurveyStatus.New
                    };
                    _context.Add(Survey);
                }

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Create", "InvestmentOwners", new { InvestmentId = investment.InvestmentId, ReturnUrl = ReturnUrl });
                }
                catch (Exception)
                {
                    throw;
                }
            }
            ViewData["CountryId"] = AddressesController.GetCountries(_context, InvestmentVM.Address.CountryId);
            ViewData["StateId"] = AddressesController.GetStates(_context, InvestmentVM.Address.StateId);
            ViewData["DistrictId"] = AddressesController.GetDistricts(_context, InvestmentVM.Address.StateId, InvestmentVM.Address.DistrictId);
            ViewData["CommuneId"] = AddressesController.GetCommunes(_context, InvestmentVM.Address.StateId, InvestmentVM.Address.DistrictId, InvestmentVM.Address.CommuneId, InvestmentVM.Address.CommuneType);

            return View(InvestmentVM);

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

            ModelState.Remove("CompletionYear");
            ModelState.Remove("HeatedArea");
            ModelState.Remove("UsableArea");
            ModelState.Remove("TotalArea");

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
        public async Task<IActionResult> AssignInspector(Guid? InspectorId, Guid[] InvestmentId, string ReturnUrl = null)
        {
            //IDictionary<string,string> result = new Dictionary<string, string>();

            _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

            try
            {
            //update
            var investmentsToAssign = _context.Investments.Where(i => InvestmentId.Any(inv => inv == i.InvestmentId)==true);

            await investmentsToAssign.LoadAsync();
            if (!InspectorId.HasValue)
            {
                await investmentsToAssign.ForEachAsync(i => i.InspectorId = _context.CurrentUserId);
            }
            else
            {
                await investmentsToAssign.ForEachAsync(i => i.InspectorId = InspectorId.Value);
            }

                //foreach (Guid item in InvestmentId)
                //{
                //    Investment singleInvestment = await _context.Investments.SingleAsync(m => m.InvestmentId == item);
                //    if (singleInvestment != null)
                //    {
                //        if (singleInvestment.InspectorId == null)
                //        {
                //            singleInvestment.InspectorId = InspectorId;
                //            _context.Update(singleInvestment);
                //        }
                //        else
                //            result.Add("InvestmentAlreadyAssigned", singleInvestment.InvestmentId.ToString());
                //    }
                //}
                //Save changes
                _context.Investments.UpdateRange(investmentsToAssign);
                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(ReturnUrl))
                    return Redirect(ReturnUrl);
                else
                    return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }

            //catch (DbUpdateException)
            //{
            //    if (Request.IsAjaxRequest())
            //        throw;
            //    else
            //        return View();
            //}

            //if (Request.IsAjaxRequest())
            //{
            //    if (result.Count != 0)
            //    {
            //        return Json("Error");
            //    }
            //    return Json("ok");
            //}
            //else
            //{
            //    if (ReturnUrl != null)
            //        return Redirect(ReturnUrl);
            //    else
            //        return RedirectToAction("Index");
            //}
        }

        // GET: Investments/Delete/5
        [Authorize(Roles = Roles.Admin + "," + Roles.User)]
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
        [Authorize(Roles = Roles.Admin + "," + Roles.User)]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var investment = await _context.Investments.SingleOrDefaultAsync(m => m.InvestmentId == id);

            _context.Investments.Remove(investment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult UploadDataFromFile(IFormCollection Form, int ContractId)
        {
            IList<string> errMessage = new List<string>();
            int success = 0;

            var file = Request.Form.Files.SingleOrDefault(f => f.FileName.ToLower().Contains(".csv"));
            if (file == null)
            {
                errMessage.Add("Błąd odczytu pliku");
                TempData["FileUploadResult"] = 8;
                TempData["FileUploadType"] = "Import Inwestycji";
                TempData["FileUploadError"] = errMessage;
                return RedirectToAction("Details", "Contracts", new { Id = ContractId });
            }

            //_azure.UploadAsync(file, enuAzureStorageContainerType.Contract, ContractId.ToString());
            //_fileStorage.Upload(file, enuAzureStorageContainerType.Contract, ContractId.ToString());
            //
            StreamReader sr = new StreamReader(file.OpenReadStream(),true);
            //Encoding.GetEncoding(1252)
            // StreamReader(stream, Encoding.UTF8))  
            //Encoding.Default
            InvestmentUploadHelper uploadhelper = new InvestmentUploadHelper(_context)
            {
                ContractId = ContractId
            };
            //
            ICollection<Survey> Surveys;
            Address InvestmentAddress;
            BusinessPartnerPerson Owner;
            InvestmentOwner InvestmentOwner;
            Investment Investment;
            Address OwnerAddress;
            string CsvRecord;
            int i = 0;

            //
            while ((CsvRecord = sr.ReadLine()) != null)
            {
                //if (errMessage.Count > 5) break;

                i += 1;
                if (!(i>1))
                {
                    continue;
                }
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        //set line to parse
                        uploadhelper.Record = CsvRecord;

                        //Parse Investment Address - also check if address has already been registered in the db
                        InvestmentAddress = uploadhelper.ParseInvestmentAddress();

                        Investment = uploadhelper.ParseInvestment();
                        Investment.Address = InvestmentAddress;

                        OwnerAddress = uploadhelper.ParseOwnerAddress();
                        Owner = (BusinessPartnerPerson)uploadhelper.ParseInvestmentOwner();

                        if (OwnerAddress.SingleLine == InvestmentAddress.SingleLine)
                        {
                            Owner.Address = InvestmentAddress;
                            OwnerAddress = null;
                        }
                        else
                        {
                            Owner.Address = OwnerAddress;
                        }

                        InvestmentOwner = new InvestmentOwner
                        {
                            Investment = Investment,
                            InvestmentId = Investment.InvestmentId,
                            Owner = Owner,
                            OwnerId = Owner.BusinessPartnerId
                        };

                        Surveys = uploadhelper.ParseSurveys();
                        foreach (var Survey in Surveys)
                        {
                            Survey.Investment = Investment;
                            Survey.InvestmentId = Investment.InvestmentId;
                        };
                        if (InvestmentAddress.AddressId == 0)
                        {
                            _context.Add(InvestmentAddress);
                        }
                        //int x = _context.SaveChanges();
                        _context.Add(Investment);
                        //x = _context.SaveChanges();
                        if (OwnerAddress != null && OwnerAddress.AddressId == 0)
                        {
                            _context.Add(OwnerAddress);
                            //x = _context.SaveChanges();
                        }
                        _context.Add(Owner);
                        //x = _context.SaveChanges();
                        _context.Add(InvestmentOwner);
                        //x = _context.SaveChanges();
                        _context.AddRange(Surveys);
                        //x = _context.SaveChanges();

                        int x = _context.SaveChanges();

                        transaction.Commit();
                        //transaction.Dispose();
                        success += 1;
                    }
                    catch (Exception exc)
                    {
                        transaction.Rollback();
                        if (exc.Message.Contains("See the inner exception for details"))
                            errMessage.Add("B³¹d w wierszu nr " + i.ToString() + ": " + exc.InnerException.Message.ToString());
                        else
                            errMessage.Add("B³¹d w wierszu nr " + i.ToString() + ": " + exc.Message.ToString());
                    }
                }
            }
            //catch (ArgumentOutOfRangeException)
            //catch (NullReferenceException)
            //catch (ArgumentNullException)
            //catch (InvalidOperationException)
            //catch (OutOfMemoryException)
            //catch (FormatException)

            sr.Close();

            if (errMessage.Count != 0)
            {
                TempData["FileUploadType"]  = "Import Inwestycji";
                TempData["FileUploadResult"] = 8;
                TempData["FileUploadError"] = errMessage;
                if (success != 0)
                {
                    TempData["FileUploadResult"] = 4;
                    TempData["FileUploadSuccess"] = "Pomyœlnie wczytano inwestycje dla " + success.ToString() + " wierszy.";
                }
            }
            else
            {
                TempData["FileUploadType"]  = "Import Inwestycji";
                TempData["FileUploadResult"]  = 0;
                TempData["FileUploadSuccess"] = "Pomyœlnie wczytano inwestycje dla " + success.ToString() + " wierszy.";
            }

            return RedirectToAction("Details", "Contracts", new { Id = ContractId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditGeneralInfoAjax( Investment investment)
        {
            try
            {   
                //Set current UserId - it will be reflected in the database tables
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));
                //
                investment.Address.CommuneType = (CommuneType)Enum.ToObject(typeof(CommuneType), investment.Address.CommuneId % 10);
                investment.Address.CommuneId /= 10;

                Investment inv = _context.Investments
                    .Include(i => i.Address)
                    .Single(i => i.InvestmentId == investment.InvestmentId);

                inv.Address.ApartmentNo = investment.Address.ApartmentNo;
                inv.Address.BuildingNo = investment.Address.BuildingNo;
                inv.Address.City = investment.Address.City;
                inv.Address.CommuneId = investment.Address.CommuneId;
                inv.Address.CommuneType = investment.Address.CommuneType;
                inv.Address.CountryId = investment.Address.CountryId;
                inv.Address.DistrictId = investment.Address.DistrictId;
                inv.Address.PostalCode = investment.Address.PostalCode;
                inv.Address.PostOfficeLocation = investment.Address.PostOfficeLocation;
                inv.Address.StateId = investment.Address.StateId;
                inv.Address.Street = investment.Address.Street;

                _context.Update(inv.Address);
                int Result = _context.SaveChanges();
                return Ok(inv.Address.AddressId);
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
    
        private bool InvestmentExists(Guid id)
        {
            return _context.Investments.Any(e => e.InvestmentId == id);
        }
    }

}
