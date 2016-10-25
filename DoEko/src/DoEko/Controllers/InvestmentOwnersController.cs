using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.DoEko;
using DoEko.ViewModels.InvestmentOwnerViewModels;
using DoEko.Models.DoEko.Addresses;
using Microsoft.AspNetCore.Authorization;

namespace DoEko.Controllers
{
    [Authorize()]
    public class InvestmentOwnersController : Controller
    {
        private readonly DoEkoContext _context;

        public InvestmentOwnersController(DoEkoContext context)
        {
            _context = context;    
        }

        // GET: InvestmentOwners
        public async Task<IActionResult> Index()
        {
            var doEkoContext = _context.InvestmentOwners.Include(i => i.Investment).Include(i => i.Owner);
            return View(await doEkoContext.ToListAsync());
        }

        // GET: InvestmentOwners/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var investmentOwner = await _context.InvestmentOwners.SingleOrDefaultAsync(m => m.InvestmentId == id);
            if (investmentOwner == null)
            {
                return NotFound();
            }

            return View(investmentOwner);
        }

        // GET: InvestmentOwners/Create
        public async Task<IActionResult> Create(Guid InvestmentId, string ReturnUrl = null)
        {
            CreateViewModel model = new CreateViewModel();
            
            Investment Investment  = await _context.Investments.Include(i=>i.Address).SingleOrDefaultAsync(i => i.InvestmentId == InvestmentId);
            //
            model.InvestmentId = Investment.InvestmentId;
            model.ContractId = Investment.ContractId;
            //
            model.OwnerPerson = new BusinessPartnerPerson();
            model.OwnerEntity = new BusinessPartnerEntity();
            //Default Address
            model.Address = Investment.Address;
            model.AddressId = Investment.AddressId;
            model.SameLocation = true;
            //owner = sponsor
            model.Sponsor = true;

            ViewData["CountryId"] = AddressesController.GetCountries(_context, model.Address.CountryId);
            ViewData["StateId"] = AddressesController.GetStates(_context, model.Address.StateId);
            ViewData["DistrictId"] = AddressesController.GetDistricts(_context, model.Address.StateId, model.Address.DistrictId);
            ViewData["CommuneId"] = AddressesController.GetCommunes(_context, model.Address.StateId, model.Address.DistrictId, model.Address.CommuneId, model.Address.CommuneType);
            model.Address.CommuneId *= 10;
            model.Address.CommuneId += (int)model.Address.CommuneType;

            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                ViewData["ReturnUrl"] = ReturnUrl;
            }
            return View(model);
        }

        // POST: InvestmentOwners/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel createViewModel, string returnUrl = null)
        {

            createViewModel.Address.CommuneType = (CommuneType)Enum.ToObject(typeof(CommuneType), createViewModel.Address.CommuneId % 10);
            createViewModel.Address.CommuneId /= 10;

            if (ModelState.IsValid)
            {
                InvestmentOwner model = new InvestmentOwner();
                model.InvestmentId = createViewModel.InvestmentId;

                //adres do utworzenia 
                if (!createViewModel.SameLocation)
                {
                    createViewModel.Address.AddressId = 0;
                    _context.Add(createViewModel.Address);
                }

                if (!string.IsNullOrEmpty(createViewModel.OwnerPerson.LastName))
                {
                    createViewModel.OwnerPerson.BusinessPartnerId = Guid.NewGuid();
                    createViewModel.OwnerPerson.AddressId = createViewModel.Address.AddressId;

                    _context.Add(createViewModel.OwnerPerson);

                    model.OwnerId = createViewModel.OwnerPerson.BusinessPartnerId;

                }
                else
                {
                    createViewModel.OwnerEntity.BusinessPartnerId = Guid.NewGuid();
                    createViewModel.OwnerEntity.AddressId = createViewModel.Address.AddressId;

                    _context.Add(createViewModel.OwnerEntity);

                    model.OwnerId = createViewModel.OwnerEntity.BusinessPartnerId;
                }

                model.Sponsor = createViewModel.Sponsor;

                _context.Add(model);
                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                return RedirectToAction("Index");
            }

            ViewData["CountryId"] = AddressesController.GetCountries(_context, createViewModel.Address.CountryId);
            ViewData["StateId"] = AddressesController.GetStates(_context, createViewModel.Address.StateId);
            ViewData["DistrictId"] = AddressesController.GetDistricts(_context, createViewModel.Address.StateId, createViewModel.Address.DistrictId);
            ViewData["CommuneId"] = AddressesController.GetCommunes(_context, createViewModel.Address.StateId, createViewModel.Address.DistrictId, createViewModel.Address.CommuneId, createViewModel.Address.CommuneType);

            return View(createViewModel);
        }

        // GET: InvestmentOwners/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var investmentOwner = await _context.InvestmentOwners.SingleOrDefaultAsync(m => m.InvestmentId == id);
            if (investmentOwner == null)
            {
                return NotFound();
            }
            ViewData["InvestmentId"] = new SelectList(_context.Investments, "InvestmentId", "InvestmentId", investmentOwner.InvestmentId);
            ViewData["OwnerId"] = new SelectList(_context.BusinessPartners, "BusinessPartnerId", "Discriminator", investmentOwner.OwnerId);
            return View(investmentOwner);
        }

        // POST: InvestmentOwners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("InvestmentId,OwnerId,Sponsor")] InvestmentOwner investmentOwner)
        {
            if (id != investmentOwner.InvestmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(investmentOwner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvestmentOwnerExists(investmentOwner.InvestmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["InvestmentId"] = new SelectList(_context.Investments, "InvestmentId", "InvestmentId", investmentOwner.InvestmentId);
            ViewData["OwnerId"] = new SelectList(_context.BusinessPartners, "BusinessPartnerId", "Discriminator", investmentOwner.OwnerId);
            return View(investmentOwner);
        }

        // GET: InvestmentOwners/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var investmentOwner = await _context.InvestmentOwners.SingleOrDefaultAsync(m => m.InvestmentId == id);
            if (investmentOwner == null)
            {
                return NotFound();
            }

            return View(investmentOwner);
        }

        // POST: InvestmentOwners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var investmentOwner = await _context.InvestmentOwners.SingleOrDefaultAsync(m => m.InvestmentId == id);
            _context.InvestmentOwners.Remove(investmentOwner);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool InvestmentOwnerExists(Guid id)
        {
            return _context.InvestmentOwners.Any(e => e.InvestmentId == id);
        }
    }
}
