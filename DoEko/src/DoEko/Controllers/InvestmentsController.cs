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

namespace DoEko.Controllers
{
    public class InvestmentsController : Controller
    {
        private readonly DoEkoContext _context;
        private readonly AzureStorage _azure;
        
        public InvestmentsController(DoEkoContext context, IConfiguration configuration)
        {
            _context = context;
            _azure = new AzureStorage(configuration.GetConnectionString("doekostorage_AzureStorageConnectionString"));
        }

        // GET: Investments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Investments.ToListAsync());
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
                    Survey Survey = new Survey
                    {
                        SurveyId = Guid.NewGuid(),
                        InvestmentId = investment.InvestmentId,
                        Type = SurveyType.Fotovoltaic,
                        Status = SurveyStatus.New
                    };
                    _context.Add(Survey);
                }

                if (InvestmentVM.RseHeatPump)
                {
                    Survey Survey = new Survey
                    {
                        SurveyId = Guid.NewGuid(),
                        InvestmentId = investment.InvestmentId,
                        Type = SurveyType.HeatPump,
                        Status = SurveyStatus.New
                    };
                    _context.Add(Survey);
                }

                if (InvestmentVM.RseSolar)
                {
                    Survey Survey = new Survey
                    {
                        SurveyId = Guid.NewGuid(),
                        InvestmentId = investment.InvestmentId,
                        Type = SurveyType.Solar,
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
            //update
            foreach (Guid item in InvestmentId)
            {
                Investment singleInvestment = await _context.Investments.SingleOrDefaultAsync(m => m.InvestmentId == item);
                if (singleInvestment != null)
                {
                    singleInvestment.InspectorId = InspectorId;

                    _context.Update(singleInvestment);
                }
            }
            //Save changes
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {

                throw;
            }

            return Redirect(ReturnUrl);
        }

        // GET: Investments/Delete/5
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
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var investment = await _context.Investments.SingleOrDefaultAsync(m => m.InvestmentId == id);

            _context.Investments.Remove(investment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDataFromFile(FormCollection Form, int ContractId)
        {
            bool error = false;
            IList<string> errMessage = new List<string>();

            var file = Request.Form.Files.SingleOrDefault(f => f.FileName.ToLower().Contains(".csv"));
            if (file == null)
            {
                //return Red;
            }

            _azure.UploadAsync(file, enuAzureStorageContainerType.Contract, ContractId.ToString());

            StreamReader sr = new StreamReader(file.OpenReadStream());
            string CsvRecord;

            //IList<string> CsvTable = new List<String>();

            //
            Address InvAddress;
            Investment Investment;
            BusinessPartnerPerson OwnerPerson;
            Address OwnerAddress;
            InvestmentOwner InvOwner;
            //
            int ctryPoland = _context.Countries.Single(c => c.Key == "PL").CountryId;
            int i = 0;
            //
            try
            {
                while ((CsvRecord = sr.ReadLine()) != null)
                {
                    i = i + 1;
                    if (i<3)
                    {
                        continue;
                    }
                    //CsvTable.Add(CsvRecord.Replace("\"", ""));
                    var LineFields = CsvRecord.Split(';');
                    if (LineFields.Length == 29)
                    {
                        InvAddress = new Address();

                        foreach (var field in LineFields)
                        {
                            field.Trim(' ');
                        }

                        InvAddress.CountryId = ctryPoland;
                        InvAddress.StateId = _context.States.Single(s => s.Key == LineFields[16].ToUpper()).StateId;
                        InvAddress.DistrictId = _context.Districts.Single(s => s.StateId == InvAddress.StateId && s.Text.ToUpper() == LineFields[17].ToUpper()).DistrictId;
                        InvAddress.CommuneType = (CommuneType)int.Parse(LineFields[19]);//(Models.DoEko.Addresses.CommuneType)Enum.Parse(typeof(Models.DoEko.Addresses.CommuneType), LineFields[19].ToString().ToUpper()),
                        InvAddress.CommuneId = _context.Communes.Single(s => s.StateId == InvAddress.StateId &&
                                                                                s.DistrictId == InvAddress.DistrictId &&
                                                                                s.Type == InvAddress.CommuneType &&
                                                                                s.Text.ToUpper() == LineFields[18].ToUpper()).CommuneId;
                        InvAddress.PostalCode = LineFields[20].ToUpper().Substring(0, LineFields[20].Length > 5 ? 5 : LineFields[20].Length);
                        InvAddress.City = LineFields[21].Substring(0, LineFields[21].Length>50 ? 50: LineFields[21].Length);
                        InvAddress.Street = LineFields[22].Substring(0, LineFields[22].Length > 50 ? 50 : LineFields[22].Length);
                        InvAddress.BuildingNo = LineFields[23].ToUpper().Substring(0, LineFields[23].Length > 10 ? 10 : LineFields[23].Length); ;
                        InvAddress.ApartmentNo = LineFields[24].ToUpper().Substring(0, LineFields[24].Length > 11 ? 11 : LineFields[24].Length); ;

                        Investment = new Investment();
                        Investment.InvestmentId = Guid.NewGuid();
                        Investment.Address = InvAddress;
                        Investment.ContractId = ContractId;
                        Investment.PlotNumber = LineFields[25].ToUpper();
                        Investment.LandRegisterNo = LineFields[26].ToUpper();
                        Investment.InspectionStatus = InspectionStatus.NotExists;
                        Investment.Status = InvestmentStatus.Initial;
                        
                        OwnerAddress = new Address();

                        OwnerAddress.CountryId = ctryPoland;
                        OwnerAddress.StateId = _context.States.Single(s => s.Key == LineFields[5].ToUpper()).StateId;
                        OwnerAddress.DistrictId = _context.Districts.Single(s => s.StateId == OwnerAddress.StateId && s.Text.ToUpper() == LineFields[6].ToString().ToUpper()).DistrictId;
                        OwnerAddress.CommuneType = (Models.DoEko.Addresses.CommuneType)int.Parse(LineFields[8]);
                        OwnerAddress.CommuneId = _context.Communes.Single(s => s.StateId == OwnerAddress.StateId     &&
                                                                               s.DistrictId == OwnerAddress.DistrictId &&
                                                                               s.Type == OwnerAddress.CommuneType      &&
                                                                               s.Text.ToUpper() == LineFields[7].ToUpper()).CommuneId;


                        OwnerAddress.PostalCode = LineFields[9].ToUpper().Substring(0, LineFields[9].Length > 5 ? 5 : LineFields[9].Length);
                        OwnerAddress.City = LineFields[10].Substring(0, LineFields[10].Length > 50 ? 50 : LineFields[10].Length);
                        OwnerAddress.Street = LineFields[11].Substring(0, LineFields[21].Length > 50 ? 50 : LineFields[11].Length); ;
                        OwnerAddress.BuildingNo = LineFields[12].ToUpper().Substring(0, LineFields[12].Length > 10 ? 10 : LineFields[12].Length); ;
                        OwnerAddress.ApartmentNo = LineFields[13].ToUpper().Substring(0, LineFields[13].Length > 10 ? 10 : LineFields[13].Length);

                        OwnerPerson = new BusinessPartnerPerson();

                        OwnerPerson.BusinessPartnerId = Guid.NewGuid();
                        OwnerPerson.Address = OwnerAddress;
                        OwnerPerson.FirstName = LineFields[14].Substring(0, LineFields[14].Length > 30 ? 30 : LineFields[14].Length);
                        OwnerPerson.LastName = LineFields[15].Substring(0, LineFields[15].Length > 30 ? 30 : LineFields[15].Length);
                        OwnerPerson.PhoneNumber = LineFields[27].Substring(0, LineFields[27].Length > 16 ? 16 : LineFields[27].Length);
                        OwnerPerson.Email = "Nie ustawiony";
                        InvOwner = new InvestmentOwner();
                        InvOwner.InvestmentId = Investment.InvestmentId;
                        InvOwner.OwnerId = OwnerPerson.BusinessPartnerId;

                        _context.Add(InvAddress);
                        int x = await _context.SaveChangesAsync();
                        _context.Add(Investment);
                        x = await _context.SaveChangesAsync();
                        _context.Add(OwnerAddress);
                        x = await _context.SaveChangesAsync();
                        _context.Add(OwnerPerson);
                        x = await _context.SaveChangesAsync();
                        _context.Add(InvOwner);
                        x = await _context.SaveChangesAsync();


                    };
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                error = true;
                errMessage.Add("B³¹d w linii" + i.ToString());
            }
            catch (NullReferenceException)
            {
                error = true;
                errMessage.Add("B³¹d w linii" + i.ToString());
            }
            catch (ArgumentNullException)
            {

                error = true;
                errMessage.Add("B³¹d w linii " + i.ToString());
            }
            catch (InvalidOperationException)
            {
                error = true;
                errMessage.Add("B³¹d w linii " + i.ToString());
            }
            catch (OutOfMemoryException)
            {
                error = true;
                errMessage.Add("B³¹d w linii " + i.ToString());
            }
            catch (FormatException)
            {
                error = true;
                errMessage.Add("B³¹d w linii " + i.ToString());
            }

            sr.Close();

            if (error)
            {
                TempData["FileUploadResult"] = false;
                TempData["FileUploadType"]  = "Import Inwestycji";
                TempData["FileUploadError"] = errMessage;
                return RedirectToAction("Details", "Contracts", new { Id = ContractId });
            }
            else
            {
                TempData["FileUploadResult"]  = true;
                TempData["FileUploadType"]  = "Import Inwestycji";

                TempData["FileUploadSuccess"] = "Pomyœlnie wczytano listê inwestycji" ;

                return RedirectToAction("Details", "Contracts", new { Id = ContractId });
            }
        }
    
    private bool InvestmentExists(Guid id)
        {
            return _context.Investments.Any(e => e.InvestmentId == id);
        }
    }
}
