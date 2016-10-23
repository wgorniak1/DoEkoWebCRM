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

namespace DoEko.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PaymentsController : Controller
    {
        private readonly DoEkoContext _context;
        private readonly AzureStorage _azure;

        public PaymentsController(DoEkoContext context, IConfiguration configuration)
        {
            _context = context;
            _azure = new AzureStorage(configuration.GetConnectionString("doekostorage_AzureStorageConnectionString"));
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
            var investments = _context.Contracts
                .Where(c => c.ContractId == ContractId)
                .Include(c => c.Investments)
                .ThenInclude(i => i.Address)
                .ThenInclude(a=>a.Commune)
                .Single().Investments;
            ViewData["DLInvestments"] = new SelectList(investments, "InvestmentId", "Address.SingleLine", null);

            var contract = _context.Contracts.Include(c=>c.Project).SingleOrDefault(c => c.ContractId == ContractId);
            ViewData["ProjectId"] = contract.ProjectId;
            ViewData["ShortDescription"] = contract.Project.ShortDescription;
            ViewData["ContractId"] = contract.ContractId;
            ViewData["ContractNumber"] = contract.Number;

            return View(payments.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> AssignInvestment(IList<Payment> Payments)
       {
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

                    try
                    {
                        _context.UpdateRange(Payments);
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
                var investments = _context.Contracts
                                .Where(c => c.ContractId == ContractId)
                                .Include(c => c.Investments)
                                .ThenInclude(i => i.Address)
                                .ThenInclude(a => a.Commune)
                                .Single().Investments;
                ViewData["DLInvestments"] = new SelectList(investments, "InvestmentId", "Address.SingleLine", null);

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
            bool error = false;

            var file = Request.Form.Files.SingleOrDefault(f => f.FileName.ToLower().Contains(".csv"));
            if (file == null)
            {
                //return Red;
            }

            _azure.UploadAsync(file, enuAzureStorageContainerType.Contract, ContractId.ToString());

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
            try
            {
                foreach (string CsvLine in CsvTable)
                {
                    var LineFields = CsvLine.Split(';');
                    if (LineFields.Length == 12)
                    {
                        payment = new Payment {
                            ContractId = ContractId,
                            SourceRow = CsvLine
                            };
                        if (DateTime.TryParse(LineFields[0].ToString(), out date))
                        {
                            payment.PaymentDate = date;
                        }
                        if (DateTime.TryParse(LineFields[1].ToString(), out date))
                        {
                            payment.PostingDate = date;
                        }
                        if (decimal.TryParse(LineFields[3].Replace('.',',').ToString(), out amount))
                        {
                            payment.Amount = amount;
                        }

                        if (payment.Amount != 0)
                        {
                            _context.Add(payment);
                            await _context.SaveChangesAsync();
                        }
                    }
                }

            }
            catch (Exception)
            {
                error = true;

            }

            if (error)
            {
                TempData["FileUploadResult"]  = false;
                TempData["FileUploadType"]    = "Import Wpłat";

                IList<string> msg = new List<string>();
                msg.Add("Błąd podczas przetwarzania pliku wpłat");

                TempData["FileUploadError"] = msg;
                return RedirectToAction("Details", "Contracts", new { Id = ContractId });
            }
            else
            {
                TempData["FileUploadResult"] = true;
                TempData["FileUploadType"] = "Import Wpłat";
                TempData["FileUploadSuccess"] = "Pomyślnie wczytano listę wpłat";

                return RedirectToAction("AssignInvestment", new { ContractId = ContractId });
            }
        }
        
    }
}