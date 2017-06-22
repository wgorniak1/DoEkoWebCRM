using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.DoEko;
using DoEko.Controllers.Extensions;

namespace DoEko.Controllers
{
    public class BusinessPartnerPersonsController : Controller
    {
        private readonly DoEkoContext _context;

        public BusinessPartnerPersonsController(DoEkoContext context)
        {
            _context = context;    
        }

        public async Task<IActionResult> List()
        {
            if (HttpContext.Request.IsAjaxRequest())
            {
                //disable change tracking
                //_context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                var persons = await _context.BPPersons
                    .Include(bp => bp.Address)//.ThenInclude(a => a.State)
                    //.Include(bp => bp.Address).ThenInclude(a => a.District)
                    //.Include(bp => bp.Address).ThenInclude(a => a.Commune)
                    .Select(bp => new
                    {
                        bp.BusinessPartnerId,
                        //bp.FirstName,
                        //bp.LastName,
                        bp.FullName,
                        bp.Pesel,
                        bp.IdNumber,
                        bp.TaxId,
                        bp.BirthDate,
                        bp.Email,
                        bp.PhoneNumber,
                        Address = new
                        {
                            FirstLine = bp.Address.FirstLine,
                            SecondLine = bp.Address.SecondLine,
                            SingleLine = bp.Address.SingleLine
                        },
                        bp.DataProcessingConfirmation
                    })
                    .AsNoTracking()
                    .ToListAsync();

                 var invowners = _context.InvestmentOwners    
                    .Include(io => io.Investment).ThenInclude(i => i.Address)//.ThenInclude(a => a.State)
                                                                             //.Include(bp => bp.InvestmentOwners).ThenInclude(io => io.Investment).ThenInclude(i => i.Address).ThenInclude(a => a.District)
                                                                             //.Include(bp => bp.InvestmentOwners).ThenInclude(io => io.Investment).ThenInclude(i => i.Address).ThenInclude(a => a.Commune)

                    //.Include(bp => bp.InvestmentOwners).ThenInclude(io => io.Investment).ThenInclude(i=>i.Contract).ThenInclude(c=>c.Project)
                    //.ToList()
                    .Select(io => new
                    {
                        io.InvestmentId,
                        io.OwnerId,
                        io.OwnershipType,
                        io.Sponsor,
                        Address = new
                        {
                            FirstLine = io.Investment.Address.FirstLine,
                            SecondLine = io.Investment.Address.SecondLine,
                            SingleLine = io.Investment.Address.SingleLine
                        },
                        //Contract = new
                        //{
                        //    io.Investment.Contract.ContractId,
                        //    io.Investment.Contract.ShortDescription,
                        //    io.Investment.Contract.Number
                        //},
                        //Project = new
                        //{
                        //    io.Investment.Contract.Project.ShortDescription,
                        //    io.Investment.Contract.ProjectId
                        //}
                    }).ToList();

                IList<object> model = new List<object>();

                foreach (var person in persons)
                {
                    model.Add(new
                    {
                        person.BusinessPartnerId,
                        //bp.FirstName,
                        //bp.LastName,
                        person.FullName,
                        person.Pesel,
                        person.IdNumber,
                        person.TaxId,
                        person.BirthDate,
                        person.Email,
                        person.PhoneNumber,
                        Address = new
                        {
                            FirstLine = person.Address.FirstLine,
                            SecondLine = person.Address.SecondLine,
                            SingleLine = person.Address.SingleLine
                        },
                        person.DataProcessingConfirmation,
                        Investments = invowners.Where(io => io.OwnerId == person.BusinessPartnerId).ToList()
                    });
                }
                //var model1 = await _context.BPPersons.Select(bp => new
                //{
                //    bp.BusinessPartnerId,
                //    //bp.FirstName,
                //    //bp.LastName,
                //    bp.FullName,
                //    bp.Pesel,
                //    bp.IdNumber,
                //    bp.TaxId,
                //    bp.BirthDate,
                //    bp.Email,
                //    bp.PhoneNumber,
                //    Address = new
                //    {
                //        FirstLine = bp.Address.FirstLine,
                //        SecondLine = bp.Address.SecondLine,
                //        SingleLine = bp.Address.SingleLine
                //    },
                //    bp.DataProcessingConfirmation,
                //    Investments = bp.InvestmentOwners.Select(io => new
                //    {
                //        io.InvestmentId,
                //        io.OwnershipType,
                //        io.Sponsor,
                //        Address = new
                //        {
                //            FirstLine = io.Investment.Address.FirstLine,
                //            SecondLine = io.Investment.Address.SecondLine,
                //            SingleLine = io.Investment.Address.SingleLine
                //        },
                //        Contract = new
                //        {
                //            io.Investment.Contract.ContractId,
                //            io.Investment.Contract.ShortDescription,
                //            io.Investment.Contract.Number
                //        },
                //        Project = new
                //        {
                //            io.Investment.Contract.Project.ShortDescription,
                //            io.Investment.Contract.ProjectId
                //        }
                //    })
                //}).ToListAsync();

                //var model = await _context.BPPersons
                //    .Include(bp => bp.Address).ThenInclude(a => a.Commune)
                //    .Include(bp => bp.InvestmentOwners).ThenInclude(io => io.Investment).ThenInclude(i => i.Address)//.ThenInclude(a => a.Commune)
                //    .Include(bp => bp.InvestmentOwners).ThenInclude(io => io.Investment).ThenInclude(i => i.Contract).ThenInclude(c => c.Project)
                //    .Select(bp => new {
                //        bp.BusinessPartnerId,
                //        //bp.FirstName,
                //        //bp.LastName,
                //        bp.FullName,
                //        bp.Pesel,
                //        bp.IdNumber,
                //        bp.TaxId,
                //        bp.BirthDate,
                //        bp.Email,
                //        bp.PhoneNumber,
                //        Address = new {
                //            FirstLine = bp.Address.FirstLine,
                //            SecondLine = bp.Address.SecondLine,
                //            SingleLine = bp.Address.SingleLine },
                //        bp.DataProcessingConfirmation,
                //        Investments = bp.InvestmentOwners.Select(io=> new {
                //            io.InvestmentId,
                //            io.OwnershipType,
                //            io.Sponsor,
                //            Address = new {
                //                FirstLine = io.Investment.Address.FirstLine,
                //                SecondLine = io.Investment.Address.SecondLine,
                //                SingleLine = io.Investment.Address.SingleLine },
                //            Contract = new { io.Investment.Contract.ContractId,
                //                             io.Investment.Contract.ShortDescription,
                //                             io.Investment.Contract.Number },
                //            Project = new {  io.Investment.Contract.Project.ShortDescription,
                //                             io.Investment.Contract.ProjectId }
                //        })
                //    }).ToListAsync();

                return Json(new { data = model });
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> DataProcessingAllowed(Guid? id, bool allowed )
        {
            if (HttpContext.Request.IsAjaxRequest())
            {
                try
                {
                    var person = _context.BPPersons.Single(bp => bp.BusinessPartnerId == id);
                    person.DataProcessingConfirmation = allowed;
                    _context.BPPersons.Update(person);
                    var result = await _context.SaveChangesAsync();

                    return Ok();
                }
                catch (Exception exc)
                {
                    return BadRequest(exc);
                }
            }
            else return BadRequest();
        }

        // GET: BusinessPartnerPersons
        public async Task<IActionResult> Index()
        {
            var doEkoContext = _context.BPPersons.Include(b => b.Address);
            return View(await doEkoContext.ToListAsync());
        }

        // GET: BusinessPartnerPersons/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var businessPartnerPerson = await _context.BPPersons.SingleOrDefaultAsync(m => m.BusinessPartnerId == id);
            if (businessPartnerPerson == null)
            {
                return NotFound();
            }

            return View(businessPartnerPerson);
        }

        // GET: BusinessPartnerPersons/Create
        public IActionResult Create(Investment NewInvestment = null, string ReturnUrl = null )
        {
            BusinessPartnerPerson BpPerson = new BusinessPartnerPerson();

            if (NewInvestment != null)
            {
                BpPerson.Address = NewInvestment.Address;
                ViewData["CountryId"] = AddressesController.GetCountries(_context, BpPerson.Address.CountryId);
                ViewData["StateId"] = AddressesController.GetStates(_context, BpPerson.Address.StateId);
                ViewData["DistrictId"] = AddressesController.GetDistricts(_context, BpPerson.Address.StateId, BpPerson.Address.DistrictId);
                ViewData["CommuneId"] = AddressesController.GetCommunes(_context, BpPerson.Address.StateId, BpPerson.Address.DistrictId, BpPerson.Address.CommuneId, BpPerson.Address.CommuneType);
            }
            else
            {
                ViewData["CountryId"] = AddressesController.GetCountries(_context, 0);
                ViewData["StateId"] = AddressesController.GetStates(_context, 0);
                ViewData["DistrictId"] = AddressesController.GetDistricts(_context, 0, 0);
                ViewData["CommuneId"] = AddressesController.GetCommunes(_context, 0, 0, 0, 0);
            }

            ViewData["ReturnUrl"] = ReturnUrl;

//            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "BuildingNo");
            return View(BpPerson);
        }

        // POST: BusinessPartnerPersons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BusinessPartnerPerson businessPartnerPerson, string ReturnUrl = null)
        {
            if (ModelState.IsValid)
            {
                businessPartnerPerson.BusinessPartnerId = Guid.NewGuid();
                _context.Add(businessPartnerPerson);
                await _context.SaveChangesAsync();
                if (!string.IsNullOrEmpty(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                else return RedirectToAction("Index");
            }
//            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "BuildingNo", businessPartnerPerson.AddressId);
            return View(businessPartnerPerson);
        }

        // GET: BusinessPartnerPersons/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var businessPartnerPerson = await _context.BPPersons.SingleOrDefaultAsync(m => m.BusinessPartnerId == id);
            if (businessPartnerPerson == null)
            {
                return NotFound();
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "BuildingNo", businessPartnerPerson.AddressId);
            return View(businessPartnerPerson);
        }

        // POST: BusinessPartnerPersons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("BusinessPartnerId,AddressId,Email,PhoneNumber,TaxId,BirthDate,FirstName,IdNumber,LastName,Pesel")] BusinessPartnerPerson businessPartnerPerson)
        {
            if (id != businessPartnerPerson.BusinessPartnerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(businessPartnerPerson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusinessPartnerPersonExists(businessPartnerPerson.BusinessPartnerId))
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
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "BuildingNo", businessPartnerPerson.AddressId);
            return View(businessPartnerPerson);
        }

        // GET: BusinessPartnerPersons/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var businessPartnerPerson = await _context.BPPersons.SingleOrDefaultAsync(m => m.BusinessPartnerId == id);
            if (businessPartnerPerson == null)
            {
                return NotFound();
            }

            return View(businessPartnerPerson);
        }

        // POST: BusinessPartnerPersons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                if (HttpContext.Request.IsAjaxRequest())
                {

                    var businessPartnerPerson = await _context.BPPersons.Include(p => p.Address).SingleOrDefaultAsync(m => m.BusinessPartnerId == id);

                    if (!_context.Investments.Any(i => i.AddressId == businessPartnerPerson.AddressId))
                    {
                        _context.Addresses.Remove(businessPartnerPerson.Address);
                    }

                    _context.BPPersons.Remove(businessPartnerPerson);

                    var result = await _context.SaveChangesAsync();
                    if (result != 0)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Błąd podczas usuwania danych");
                        return BadRequest(ModelState);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Błędny typ żądania");
                    return BadRequest(ModelState);
                }
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.InnerException == null ? exc.Message : exc.InnerException.Message);
                return BadRequest(ModelState);

            }
        }

        private bool BusinessPartnerPersonExists(Guid id)
        {
            return _context.BPPersons.Any(e => e.BusinessPartnerId == id);
        }
        #region Helpers
        #endregion

    }
}
