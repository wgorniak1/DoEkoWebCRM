using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DoEko.Models.DoEko;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using DoEko.Models.Identity;
using DoEko.Services;
using DoEko.Controllers.Attributes;

namespace DoEko.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class PaymentsController : Controller
    {
        private readonly DoEkoContext _context;
        private readonly IFileStorage _fileStorage;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly AzureStorage _azure;

        public PaymentsController(DoEkoContext context, IFileStorage fileStorage, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _fileStorage = fileStorage;
            _userManager = userManager;
            //_azure = new AzureStorage(configuration.GetConnectionString("doekostorage_AzureStorageConnectionString"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AssignInvestment(int ContractId)
        {
            IList<Payment> payments = _context.Payments.Where(p => p.ContractId == ContractId && p.InvestmentId == null && p.NotNeeded == false ).ToList();
            if (!payments.Any())
            {
                return RedirectToAction("Details", "Contracts", new { Id = ContractId });
            }
            //var investments = _context.Contracts
            //    .Where(c => c.ContractId == ContractId)
            //    .Include(c => c.Investments)
            //    .ThenInclude(i => i.Address)
            //    .ThenInclude(a=>a.Commune)
            //    .Single().Investments;
            var investments = _context.Investments
                .Where(i => i.ContractId == ContractId)
                .Include(i => i.Address).ThenInclude(a => a.Commune)
                .Include(i => i.InvestmentOwners).ThenInclude(io=>io.Owner)
                .OrderBy(i => i.Address.SingleLine)
                .ToList();
            var ls = investments
                .Select(i => new SelectListItem
                {
                    Text = i.InvestmentOwners.Count > 0 ? i.Address.SingleLine + ", " +
                            i.InvestmentOwners.FirstOrDefault().Owner.PartnerName1 + ' ' +
                            i.InvestmentOwners.FirstOrDefault().Owner.PartnerName2 :
                            i.Address.SingleLine,
                    Value = i.InvestmentId.ToString()
                }).ToList();
            ViewData["DLInvestments"] = new SelectList(ls, "Value", "Text", null);

            var contract = _context.Contracts.Include(c=>c.Project).SingleOrDefault(c => c.ContractId == ContractId);
            ViewData["ProjectId"] = contract.ProjectId;
            ViewData["ShortDescription"] = contract.Project.ShortDescription;
            ViewData["ContractId"] = contract.ContractId;
            ViewData["ContractNumber"] = contract.Number;

            return View(payments.ToList());
        }

        [HttpPost]
        [RequestFormSizeLimitAttribute(5000, Order = 1)]
        [ValidateAntiForgeryToken(Order = 2)]
        public async Task<IActionResult> AssignInvestment(IList<Payment> Payments)
       {
            _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));
            var ContractId = Payments.ElementAt(0).ContractId;
            bool error = true;

            if (ModelState.IsValid)
            {
                if(Payments.Where( p => p.InvestmentId   != null  && 
                                        p.NotNeeded      == false && 
                                        p.RseFotovoltaic == false &&
                                        p.RseHeatPump    == false && 
                                        p.RseSolar       == false ).Any())
                {
                    ModelState.AddModelError(string.Empty, $"Proszę wybrać przynajmniej jedno źródło, jeśli przypisano inwestycję");
                }
                else
                {
                    //
                    Payments.Where(p => p.NotNeeded == true)
                        .ToList().ForEach(p => { p.InvestmentId = null; p.RseFotovoltaic = false; p.RseHeatPump = false; p.RseSolar = false; });

                    //
                    foreach (var item in Payments)
                    {
                        Payment dbp = _context.Payments.Single(p => p.PaymentId == item.PaymentId);

                        dbp.InvestmentId = item.InvestmentId;
                        dbp.NotNeeded = item.NotNeeded;
                        dbp.RseFotovoltaic = item.RseFotovoltaic;
                        dbp.RseHeatPump = item.RseHeatPump;
                        dbp.RseSolar = item.RseSolar;
                        _context.Payments.Update(dbp);
                    }

                    try
                    {
                        //_context.UpdateRange(Payments);
                        int x = await _context.SaveChangesAsync();
                        error = false;
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        throw;
                    }
                }
            }

            if (!error)
            {
                return RedirectToAction("Details", "Contracts", new { Id = ContractId });
            }
            else
            {
                var investments = _context.Investments
                                .Where(i => i.ContractId == ContractId)
                                .Include(i => i.Address).ThenInclude(a => a.Commune)
                                .Include(i => i.InvestmentOwners).ThenInclude(io => io.Owner)
                                .OrderBy(i => i.Address.SingleLine)
                                .ToList();
                var ls = investments
                    .Select(i => new SelectListItem
                    {
                        Text = i.InvestmentOwners.Count > 0 ? i.Address.SingleLine + ", " +
                                i.InvestmentOwners.FirstOrDefault().Owner.PartnerName1 + ' ' +
                                i.InvestmentOwners.FirstOrDefault().Owner.PartnerName2 :
                                i.Address.SingleLine,
                        Value = i.InvestmentId.ToString()
                    }).ToList();
                ViewData["DLInvestments"] = new SelectList(ls, "Value", "Text", null);

                var contract = _context.Contracts.Include(c => c.Project).SingleOrDefault(c => c.ContractId == ContractId);
                ViewData["ProjectId"] = contract.ProjectId;
                ViewData["ShortDescription"] = contract.Project.ShortDescription;
                ViewData["ContractId"] = contract.ContractId;
                ViewData["ContractNumber"] = contract.Number;

                return View(Payments);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadPaymentFile(FormCollection Form, int ContractId)
        {
            _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));
            bool error = false;
            int index;
            IList<string> msg = new List<string>();


            var file = Request.Form.Files.SingleOrDefault(f => f.FileName.ToLower().Contains(".csv"));
            if (file == null)
            {
                //return Red;
            }

            //_azure.Upload(file, enuAzureStorageContainerType.Contract, ContractId.ToString());
            
            _fileStorage.Upload(file, enuAzureStorageContainerType.Contract, ContractId.ToString());

            StreamReader sr = new StreamReader(file.OpenReadStream());
            string CsvRecord;
            IList<string> CsvTable = new List<String>();
            while ((CsvRecord = sr.ReadLine()) != null)
            {
                CsvTable.Add(CsvRecord.Replace("\"", ""));
            }

            sr.Close();

            Payment payment;
            DateTime date;
            Decimal amount;
            index = 0;
            //
            char separator = ',';
            if (CsvTable.First().Split(separator).Length < 12)
                separator = ';';
            if (CsvTable.First().Split(separator).Length < 12)
            {
                msg.Add("Błąd struktury pliku");
                msg.Add("Nie można rozpoznać separatora (; lub ,)");
                TempData["FileUploadResult"] = 8;
                TempData["FileUploadType"] = "Import Wpłat";
                TempData["FileUploadError"] = msg;
                return RedirectToAction("Details", "Contracts", new { Id = ContractId });
            }

            try
            {
                //Remove header
                try
                {
                    CsvTable.RemoveAt(0);
                }
                catch (Exception)
                { }
                foreach (string CsvLine in CsvTable)
                {
                    index += 1;
                    var LineFields = CsvLine.Split(separator);
                    if (LineFields.Length < 12)
                    {
                        continue;
                    }
                    payment = new Payment {
                        ContractId = ContractId,
                        SourceRow = CsvLine,
                        PaymentDate = DateTime.Parse(LineFields[0].ToString()),
                        PostingDate = DateTime.Parse(LineFields[1].ToString()),
                        Amount = decimal.Parse(LineFields[3].Replace(',','.').ToString())
                    };
                    //if (DateTime.TryParse(LineFields[0].ToString(), out date))
                    //{
                    //    payment.PaymentDate = date;
                    //}
                    //if (DateTime.TryParse(LineFields[1].ToString(), out date))
                    //{
                    //    payment.PostingDate = date;
                    //}
                    //if (decimal.TryParse(LineFields[3].Replace('.',',').ToString(), out amount))
                    //{
                    //    payment.Amount = amount;
                    //}

                    if (payment.Amount != 0)
                    {
                        _context.Add(payment);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception exc)
            {
                error = true;
                msg.Add("Błąd podczas przetwarzania w wierszu " + index.ToString());
                if (exc.InnerException != null)
                    msg.Add(exc.InnerException.Message);
                else
                    msg.Add(exc.Message);
            }

            if (error)
            {
                TempData["FileUploadResult"]  = 8;
                TempData["FileUploadType"]    = "Import Wpłat";
                TempData["FileUploadError"] = msg;
                return RedirectToAction("Details", "Contracts", new { Id = ContractId });
            }
            else
            {
                TempData["FileUploadResult"] = 0;
                TempData["FileUploadType"] = "Import Wpłat";
                TempData["FileUploadSuccess"] = "Pomyślnie wczytano listę wpłat";

                return RedirectToAction("AssignInvestment", new { ContractId = ContractId });
            }
        }
        
    }
}