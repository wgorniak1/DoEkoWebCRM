using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DoEko.Models.DoEko;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DoEko.Models.Identity;
using DoEko.Models.DoEko.Addresses;
using DoEko.Controllers.Helpers;
using DoEko.Models.DoEko.Survey;
using DoEko.ViewModels.SurveyViewModels;

namespace DoEko.Controllers
{
    public class TestController : Controller
    {
        private DoEkoContext _context;
        private UserManager<ApplicationUser> _userManager;
        public TestController(DoEkoContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }

        // GET: Test/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Test/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Test/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));
            try
            {
                // TODO: Add insert logic here
                test model = new Models.DoEko.test { checkme = true, PaymentId = Guid.NewGuid() };
                _context.Add(model);
                int result = await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch(Exception exc)
            {
                //exc.Message
                return Json(exc.Message);
            }
        }

        // GET: Test/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Test/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Test/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Test/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult csvTest()
        {
            _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));
            string line = "1; 98,40 z³ ; 80,00 z³ ;Kocio³ na Pellet;;;Lubelskie;Janowski;Chrzanów (Gmina W.);37-500; Tuczempy ;Mickiewicza;44;;Ryszard;Trelka;Podlaskie;Bia³ostocki;Choroszcz (Gmina M-W);37-500;Tuczempy;Mickiewicza;3A;1;266;30621;661 111 760;;;";

            InvestmentUploadHelper uploadhelper = new InvestmentUploadHelper(_context);
            uploadhelper.ContractId = _context.Contracts.First().ContractId;

            try
            {   
                //set line to parse
                uploadhelper.Record = line;


                Address InvestmentAddress = uploadhelper.ParseInvestmentAddress();
                Investment Investment = uploadhelper.ParseInvestment();
                Investment.Address = InvestmentAddress;

                Address OwnerAddress = uploadhelper.ParseOwnerAddress();
                BusinessPartnerPerson Owner = (BusinessPartnerPerson) uploadhelper.ParseInvestmentOwner();

                if (OwnerAddress.SingleLine == InvestmentAddress.SingleLine)
                {
                    Owner.Address = InvestmentAddress;
                    OwnerAddress = null;
                }
                else
                {
                    Owner.Address = OwnerAddress;
                }

                InvestmentOwner InvestmentOwner = new InvestmentOwner
                {
                    Investment = Investment,
                    InvestmentId = Investment.InvestmentId,
                    Owner = Owner,
                    OwnerId = Owner.BusinessPartnerId
                };

                ICollection<Survey> Surveys = uploadhelper.ParseSurveys();
                foreach (var Survey in Surveys)
                {
                    Survey.Investment = Investment;
                    Survey.InvestmentId = Investment.InvestmentId;
                };
                
            }
            catch (Exception exc)
            {
                return Json(exc.Message);
            }

            return Json("ok");
        }

        public async Task<ActionResult> TestOwner()
        {
            SurveyCentralHeating survey = await _context.SurveysCH
                .Include(s=>s.Investment).ThenInclude(i=>i.Address).ThenInclude(a=>a.Commune)
                .Include(s=>s.Investment).ThenInclude(i=>i.InvestmentOwners).ThenInclude(io=>io.Owner).ThenInclude(o=>o.Address)
                .FirstAsync();

            DetailsCHViewModel model = new DetailsCHViewModel(survey);

            return View(model);
        }
    }
}