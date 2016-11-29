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
using DoEko.ViewComponents.ViewModels;
using Microsoft.AspNetCore.Identity;
using DoEko.Models.Identity;

namespace DoEko.Controllers
{
    [Authorize()]
    public class InvestmentOwnersController : Controller
    {
        private readonly DoEkoContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public InvestmentOwnersController(DoEkoContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OwnerPersonSaveAjax(InvestmentOwnerPersonViewModel model)
        {
            string text = model.Owner.FirstName;
            if (text == "Wojciech")
            {
                return Json(new { text = "uda³o siê" });
            }
            else
            //    Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            ////Log your exception
            //return Json(new { Message = e.Message });
                return NotFound();

            //400 - BadRequest brak wymaganych parametrów 
            //422 - parametry podane sa wszystkie ale w zlym formacie
            //404 - NotFoundResult parametry s¹ podane i s¹ w prawid³owym formacie ale po prostu nie ma danych
            //200 - przy odczycie, ok zwracam wynik
            //201 - przy zapisie, ok zapisalem 

        }

        [HttpGet]
        public ActionResult CreatePersonAjax(Guid investmentId)
        {
            return ViewComponent("InvestmentOwnerData", new { investmentId = investmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePersonAjax(InvestmentOwnerPersonViewModel io)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

                if (ModelState.IsValid)
                {
                    // Owner Id
                    io.Owner.BusinessPartnerId = Guid.NewGuid();
                    // Owner Address
                    if (io.SameAddress)
                    {
                        io.Owner.Address = _context.Investments.Include(i => i.Address).Single(i => i.InvestmentId == io.InvestmentId).Address;
                        io.Owner.AddressId = io.Owner.Address.AddressId;
                    }
                    else
                    {
                        io.Owner.Address.AddressId = 0;
                        io.Owner.AddressId = 0;
                        //Adres gmina i typ sklejone w jednym polu        
                        io.Owner.Address.CommuneType = (CommuneType)Enum.ToObject(typeof(CommuneType), io.Owner.Address.CommuneId % 10);
                        io.Owner.Address.CommuneId /= 10;

                        _context.Addresses.Add(io.Owner.Address);
                    }

                    InvestmentOwner iowner = new InvestmentOwner()
                    {
                        InvestmentId = io.InvestmentId,
                        Owner = io.Owner,
                        OwnerId = io.Owner.BusinessPartnerId,
                        OwnershipType = io.OwnershipType,
                        Sponsor = io.Sponsor
                    };

                    _context.BPPersons.Add(io.Owner);
                    _context.InvestmentOwners.Add(iowner);
                    int Result = await _context.SaveChangesAsync();

                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult EditPersonAjax(InvestmentOwnerPersonViewModel model)
        {
            try
            {
                int Result = 0;
                //
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));
                //
                model.Owner.Address.CommuneType = (CommuneType)Enum.ToObject(typeof(CommuneType), model.Owner.Address.CommuneId % 10);
                model.Owner.Address.CommuneId /= 10;


                //Owner Data
                BusinessPartnerPerson owner = _context.BPPersons.Where(p => p.BusinessPartnerId == model.Owner.BusinessPartnerId).Single();

                int OldAddressId = owner.AddressId;

                owner.FirstName = model.Owner.FirstName;
                owner.Email = model.Owner.Email;
                owner.LastName = model.Owner.LastName;
                owner.PhoneNumber = model.Owner.PhoneNumber;
                if (User.IsInRole(Roles.Admin))
                {
                    //These fields are visible only for administrators
                    owner.IdNumber = model.Owner.IdNumber;
                    owner.Pesel = model.Owner.Pesel;
                    owner.TaxId = model.Owner.TaxId;
                }

                //Owner Address
                if (model.SameAddress)
                {
                    //Update owner address = Set address Id to investment address
                    owner.AddressId = _context.Investments.Where(i => i.InvestmentId == model.InvestmentId).Select(i => i.AddressId).Single();

                    _context.BPPersons.Update(owner);
                    Result = _context.SaveChanges();

                    //Delete old address if set
                    if (owner.AddressId != OldAddressId)
                        try
                        {
                            Address DeleteAddress = _context.Addresses.Where(a => a.AddressId == OldAddressId).Single();
                            _context.Addresses.Remove(DeleteAddress);
                            //
                            Result = _context.SaveChanges();
                        }
                        catch (Exception exc) { }
                }
                else
                {
                    //Owner address different than investment
                    if (model.Owner.AddressId != 0)
                    {
                        //Existing address to update
                        Address ownerAddress = _context.Addresses.Where(a => a.AddressId == model.Owner.AddressId).Single();

                        ownerAddress.ApartmentNo = model.Owner.Address.ApartmentNo;
                        ownerAddress.BuildingNo = model.Owner.Address.BuildingNo;
                        ownerAddress.City = model.Owner.Address.City;
                        ownerAddress.CommuneId = model.Owner.Address.CommuneId;
                        ownerAddress.CommuneType = model.Owner.Address.CommuneType;
                        ownerAddress.CountryId = model.Owner.Address.CountryId;
                        ownerAddress.DistrictId = model.Owner.Address.DistrictId;
                        ownerAddress.PostalCode = model.Owner.Address.PostalCode;
                        ownerAddress.PostOfficeLocation = model.Owner.Address.PostOfficeLocation;
                        ownerAddress.StateId = model.Owner.Address.StateId;
                        ownerAddress.Street = model.Owner.Address.Street;

                        owner.AddressId = model.Owner.AddressId;

                        _context.BPPersons.Update(owner);
                        _context.Addresses.Update(ownerAddress);
                        Result = _context.SaveChanges();

                    }
                    else
                    {
                        //new address to create
                        owner.Address = new Address
                        {
                            ApartmentNo = model.Owner.Address.ApartmentNo,
                            BuildingNo = model.Owner.Address.BuildingNo,
                            City = model.Owner.Address.City,
                            CommuneId = model.Owner.Address.CommuneId,
                            CommuneType = model.Owner.Address.CommuneType,
                            CountryId = model.Owner.Address.CountryId,
                            DistrictId = model.Owner.Address.DistrictId,
                            PostalCode = model.Owner.Address.PostalCode,
                            PostOfficeLocation = model.Owner.Address.PostOfficeLocation,
                            StateId = model.Owner.Address.StateId,
                            Street = model.Owner.Address.Street
                        };

                        _context.Addresses.Add(owner.Address);
                        _context.BPPersons.Update(owner);

                        Result = _context.SaveChanges();
                        
                    }
                }

                //Update Investment Owner
                InvestmentOwner io = _context.InvestmentOwners
                    .Single(i => i.InvestmentId == model.InvestmentId && i.OwnerId == model.Owner.BusinessPartnerId);

                io.OwnershipType = model.OwnershipType;
                io.Sponsor = model.Sponsor;

                _context.InvestmentOwners.Update(io);
                Result = _context.SaveChanges();

                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePersonAjax(Guid InvestmentId, Guid OwnerId)
        {
            try
            {
                InvestmentOwner io = _context.InvestmentOwners.Where(i => i.InvestmentId == InvestmentId && i.OwnerId == OwnerId).Single();
                _context.InvestmentOwners.Remove(io);
                int Result = _context.SaveChanges();

                BusinessPartnerPerson owner = _context.BPPersons.Where(p => p.BusinessPartnerId == OwnerId).Single();
                Address ownerAddress = _context.Addresses.Where(a => a.AddressId == owner.AddressId).Single();

                _context.BPPersons.Remove(owner);
                Result = _context.SaveChanges();

                try
                {
                    _context.Addresses.Remove(ownerAddress);
                    Result = _context.SaveChanges();
                }
                catch (Exception exc) { //if address is the same  
                                      };
                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }

        private bool InvestmentOwnerExists(Guid id)
        {
            return _context.InvestmentOwners.Any(e => e.InvestmentId == id);
        }
    }
}
