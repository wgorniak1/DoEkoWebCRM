using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.DoEko;
using DoEko.Models.Payroll;
using Microsoft.AspNetCore.Authorization;
using DoEko.Models.Identity;
using DoEko.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;
using DoEko.Controllers.Helpers;

namespace DoEko.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/v1/BusinessPartners")]
    [Authorize()]
    public class ApiBusinessPartnerController : Controller
    {
        private readonly DoEkoContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApiBusinessPartnerController(DoEkoContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // POST: api/v1/BusinessPartners
        [HttpPost]
        [Route("CreatePerson")]
        public async Task<IActionResult> CreatePerson([FromBody] BusinessPartnerPerson Person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _context.BPPersons.Add(Person);

                int result = await _context.SaveChangesAsync();

            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
                return BadRequest(ModelState);
            }

            return CreatedAtAction("CreatePerson", new { id = Person.BusinessPartnerId }, Person);
        }

        // GET: api/v1/BusinessPartners/1234
        [HttpGet("{id}")]
        public IActionResult GetPartner([FromRoute] Guid id)
        {
            if (this.EmployeeExists(id))
            {
                BusinessPartner bp = _context.BusinessPartners.Single(p => p.BusinessPartnerId == id);

                if (bp.Type == BusinessPartnerType.Person)
                {
                    BusinessPartnerPerson bpp = (BusinessPartnerPerson)bp;
                    return Ok(bpp);
                }
                else
                {
                    BusinessPartnerEntity bpe = (BusinessPartnerEntity)bp;
                    return Ok(bpe);
                }
            }
            else
                return NotFound(id);
        }

        // PUT: api/ApiEmployees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBusinessPartner([FromRoute] Guid id, [FromBody] BusinessPartner businessPartner)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != businessPartner.BusinessPartnerId)
            {
                return BadRequest();
            }

            _context.Entry(businessPartner).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException exc)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound(id);
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // DELETE: api/ApiEmployees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {

            var bp = await _context.BusinessPartners.SingleOrDefaultAsync(p => p.BusinessPartnerId == id);
            if (bp == null)
            {
                return NotFound(); //NoContent?
            }

            _context.BusinessPartners.Remove(bp);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        #region Dictionary
        // GET: api/v1/BusinessPartners/Types
        [HttpGet]
        [Route("Types")]
        public IActionResult GetPartnerTypes()
        {
            return Ok(EnumHelper.GetKeyValuePairs(typeof(BusinessPartnerType)));
        }

        #endregion

        #region Private
        private bool EmployeeExists(Guid id)
        {
            return _context.Employees.Any(e => e.BusinessPartnerId == id);
        }

        #endregion

    }
}