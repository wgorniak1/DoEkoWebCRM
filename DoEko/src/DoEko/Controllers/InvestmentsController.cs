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

namespace DoEko.Controllers
{
    [Authorize()]
    public class InvestmentsController : Controller
    {
        private readonly DoEkoContext _context;
        private readonly AzureStorage _azure;
        private readonly UserManager<ApplicationUser> _userManager;

        public InvestmentsController(DoEkoContext context, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _azure = new AzureStorage(configuration.GetConnectionString("doekostorage_AzureStorageConnectionString"));
            _userManager = userManager;    
        }

        // GET: Investments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Investments.ToListAsync());
        }

        // GET: My Investments
        [HttpGet]
        public async Task<IActionResult> List()
        {
            Guid UserId;
            Guid.TryParse( _userManager.GetUserId(User), out UserId);

            var model = await _context.Investments
                .Where(i => i.InspectorId == UserId)
                .Include(i => i.Address)
                .Include(i => i.InvestmentOwners).ThenInclude(io => io.Owner)
                .ToListAsync();

            return View(model);
        }
        // GET: Unassigned Investments
        [HttpGet]
        public async Task<IActionResult> ListUnassigned()
        {
            //Guid UserId;
            //Guid.TryParse(_userManager.GetUserId(User), out UserId);
            ViewData["UserId"] = _userManager.GetUserId(User);

            var model = await _context.Investments.Where(i => i.InspectorId == null)
                .Include(i=>i.Address)
                .Include(i=>i.InvestmentOwners).ThenInclude(io=>io.Owner)
                .ToListAsync();


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
            Investment investment = InvestmentVM.AsBase();

            if (ModelState.IsValid)
            {
                investment.InvestmentId = Guid.NewGuid();

                _context.Add(investment.Address);
                _context.Add(investment);

                if (InvestmentVM.RseFotovoltaic)
                {
                    SurveyEnergy Survey = new SurveyEnergy
                    {
                        SurveyId = Guid.NewGuid(),
                        InvestmentId = investment.InvestmentId,
                        Type = SurveyType.Energy,
                        Status = SurveyStatus.New
                    };
                    _context.Add(Survey);
                }

                if (InvestmentVM.RseHeatPump)
                {
                    SurveyCentralHeating Survey = new SurveyCentralHeating
                    {
                        SurveyId = Guid.NewGuid(),
                        InvestmentId = investment.InvestmentId,
                        Type = SurveyType.CentralHeating,
                        Status = SurveyStatus.New
                    };
                    _context.Add(Survey);
                }

                if (InvestmentVM.RseSolar)
                {
                    SurveyHotWater Survey = new SurveyHotWater
                    {
                        SurveyId = Guid.NewGuid(),
                        InvestmentId = investment.InvestmentId,
                        Type = SurveyType.SolarHotWater,
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
        public async Task<IActionResult> AssignInspector(Guid InspectorId, Guid[] InvestmentId, string ReturnUrl)
        {
            IDictionary<string,string> result = new Dictionary<string, string>();
            
            //update
            foreach (Guid item in InvestmentId)
            {
                Investment singleInvestment = await _context.Investments.SingleOrDefaultAsync(m => m.InvestmentId == item);
                if (singleInvestment != null)
                {
                    if (singleInvestment.InspectorId == null)
                    {
                        singleInvestment.InspectorId = InspectorId;
                        _context.Update(singleInvestment);
                    }
                    else
                        result.Add("InvestmentAlreadyAssigned", singleInvestment.InvestmentId.ToString());
                }
            }
            //Save changes
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (Request.IsAjaxRequest())
                    throw;
                else
                    return View();
            }

            if (Request.IsAjaxRequest())
            {
                if (result.Count != 0)
                {
                    return Json("Error");
                }
                return Json("ok");
            }
            else
            {
                if (ReturnUrl != null)
                    return Redirect(ReturnUrl);
                else
                    return RedirectToAction("Index");
            }
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
        public IActionResult UploadDataFromFile(FormCollection Form, int ContractId)
        {
            IList<string> errMessage = new List<string>();
            int success = 0;

            var file = Request.Form.Files.SingleOrDefault(f => f.FileName.ToLower().Contains(".csv"));
            if (file == null)
            {
                errMessage.Add("B³¹d odczytu pliku");
                TempData["FileUploadResult"] = 8;
                TempData["FileUploadType"] = "Import Inwestycji";
                TempData["FileUploadError"] = errMessage;
                return RedirectToAction("Details", "Contracts", new { Id = ContractId });
            }

            //_azure.UploadAsync(file, enuAzureStorageContainerType.Contract, ContractId.ToString());

            //
            StreamReader sr = new StreamReader(file.OpenReadStream());
            
            //
            string CsvRecord;
            Address InvAddress;
            Investment Investment;
            Address OwnerAddress;
            BusinessPartnerPerson OwnerPerson;
            InvestmentOwner InvOwner;
            //
            int ctryPoland = _context.Countries.Single(c => c.Key == "PL").CountryId;
            int i = 0;
            //
            while ((CsvRecord = sr.ReadLine()) != null)
            {
                if (errMessage.Count > 5)
                {
                    break;
                }
                i += 1;
                if (i<3) continue;
                    
                var LineFields = CsvRecord.Split(';');
                if (LineFields.Length < 29)
                {
                    errMessage.Add("B³¹d struktury danych w wierszu " + i.ToString());
                    continue;
                }
                for (int k = 0; k < LineFields.Length; k++)
                {
                    LineFields[k] = LineFields[k].TrimStart().TrimEnd().Trim();
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        InvAddress = new Address();

                        InvAddress.CountryId = ctryPoland;
                        InvAddress.StateId = _context.States.Single(s => s.Key == LineFields[17].ToUpper()).StateId;
                        InvAddress.DistrictId = _context.Districts.Single(s => s.StateId == InvAddress.StateId && s.Text.ToUpper() == LineFields[18].ToUpper()).DistrictId;
                        InvAddress.CommuneType = (CommuneType)int.Parse(LineFields[20]);//(Models.DoEko.Addresses.CommuneType)Enum.Parse(typeof(Models.DoEko.Addresses.CommuneType), LineFields[19].ToString().ToUpper()),
                        InvAddress.CommuneId = _context.Communes.Single(s => s.StateId == InvAddress.StateId &&
                                                                                s.DistrictId == InvAddress.DistrictId &&
                                                                                s.Type == InvAddress.CommuneType &&
                                                                                s.Text.ToUpper() == LineFields[19].ToUpper()).CommuneId;
                        InvAddress.PostalCode = LineFields[21].ToUpper().Substring(0, LineFields[21].Length > 6 ? 6 : LineFields[21].Length);
                        InvAddress.City = LineFields[22].Substring(0, LineFields[22].Length > 50 ? 50 : LineFields[22].Length);
                        InvAddress.Street = LineFields[23].Substring(0, LineFields[23].Length > 50 ? 50 : LineFields[23].Length);
                        InvAddress.BuildingNo = LineFields[24].ToUpper().Substring(0, LineFields[24].Length > 10 ? 10 : LineFields[24].Length); ;
                        InvAddress.ApartmentNo = LineFields[25].ToUpper().Substring(0, LineFields[25].Length > 11 ? 11 : LineFields[25].Length); ;

                        Investment = new Investment();
                        Investment.InvestmentId = Guid.NewGuid();
                        Investment.Address = InvAddress;
                        Investment.ContractId = ContractId;
                        Investment.PlotNumber = LineFields[26].ToUpper();
                        Investment.LandRegisterNo = LineFields[27].ToUpper();
                        Investment.InspectionStatus = InspectionStatus.NotExists;
                        Investment.Status = InvestmentStatus.Initial;

                        OwnerAddress = new Address();

                        OwnerAddress.CountryId = ctryPoland;
                        OwnerAddress.StateId = _context.States.Single(s => s.Key == LineFields[6].ToUpper()).StateId;
                        OwnerAddress.DistrictId = _context.Districts.Single(s => s.StateId == OwnerAddress.StateId && s.Text.ToUpper() == LineFields[7].ToString().ToUpper()).DistrictId;
                        OwnerAddress.CommuneType = (Models.DoEko.Addresses.CommuneType)int.Parse(LineFields[9]);
                        OwnerAddress.CommuneId = _context.Communes.Single(s => s.StateId == OwnerAddress.StateId &&
                                                                                s.DistrictId == OwnerAddress.DistrictId &&
                                                                                s.Type == OwnerAddress.CommuneType &&
                                                                                s.Text.ToUpper() == LineFields[8].ToUpper()).CommuneId;

                        OwnerAddress.PostalCode = LineFields[10].ToUpper().Substring(0, LineFields[10].Length > 6 ? 6 : LineFields[10].Length);
                        OwnerAddress.City = LineFields[11].Substring(0, LineFields[11].Length > 50 ? 50 : LineFields[11].Length);
                        OwnerAddress.Street = LineFields[12].Substring(0, LineFields[12].Length > 50 ? 50 : LineFields[12].Length);
                        OwnerAddress.BuildingNo = LineFields[13].ToUpper().Substring(0, LineFields[13].Length > 10 ? 10 : LineFields[13].Length);
                        OwnerAddress.ApartmentNo = LineFields[14].ToUpper().Substring(0, LineFields[14].Length > 10 ? 10 : LineFields[14].Length);

                        OwnerPerson = new BusinessPartnerPerson();

                        OwnerPerson.BusinessPartnerId = Guid.NewGuid();
                        OwnerPerson.Address = OwnerAddress;
                        OwnerPerson.FirstName = LineFields[15].Substring(0, LineFields[15].Length > 30 ? 30 : LineFields[15].Length);
                        OwnerPerson.LastName = LineFields[16].Substring(0, LineFields[16].Length > 30 ? 30 : LineFields[16].Length);
                        OwnerPerson.PhoneNumber = LineFields[28].Substring(0, LineFields[28].Length > 16 ? 16 : LineFields[28].Length);
                        OwnerPerson.Email = "Nie ustawiony";
                        InvOwner = new InvestmentOwner();
                        InvOwner.InvestmentId = Investment.InvestmentId;
                        InvOwner.OwnerId = OwnerPerson.BusinessPartnerId;

                        _context.Add(InvAddress);
                        int x = _context.SaveChanges();
                        _context.Add(Investment);
                        x = _context.SaveChanges();
                        _context.Add(OwnerAddress);
                        x = _context.SaveChanges();
                        _context.Add(OwnerPerson);
                        x = _context.SaveChanges();
                        _context.Add(InvOwner);
                        x = _context.SaveChanges();

                        if (LineFields[5].Contains("TAK"))
                        {
                            SurveyEnergy srvEN = new SurveyEnergy { SurveyId = Guid.NewGuid(), InvestmentId = Investment.InvestmentId, Type = SurveyType.Energy };
                            _context.Add(srvEN);
                            x = _context.SaveChanges();
                        }
                        if (LineFields[4].Contains("TAK"))
                        {
                            SurveyHotWater srvHW = new SurveyHotWater { SurveyId = Guid.NewGuid(), InvestmentId = Investment.InvestmentId, Type = SurveyType.SolarHotWater };
                            _context.Add(srvHW);
                            x = _context.SaveChanges();
                        }
                        if (LineFields[3].Contains("TAK"))
                        {
                            SurveyCentralHeating srvCH = new SurveyCentralHeating { SurveyId = Guid.NewGuid(), InvestmentId = Investment.InvestmentId, Type = SurveyType.CentralHeating };
                            _context.Add(srvCH);
                            x = _context.SaveChanges();
                        }

                        //int x = _context.SaveChanges();

                        transaction.Commit();
                        transaction.Dispose();
                        success += 1;
                    }
                    catch (Exception exc)
                    {
                        transaction.Rollback();

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
    
        private bool InvestmentExists(Guid id)
        {
            return _context.Investments.Any(e => e.InvestmentId == id);
        }
    }
}
