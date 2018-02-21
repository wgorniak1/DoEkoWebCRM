using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.DoEko;
using DoEko.Models.DoEko.Addresses;
using Microsoft.AspNetCore.Authorization;

namespace DoEko.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/v1/Address")]
    [Authorize]
    public class ApiAddressController : Controller
    {
        private readonly DoEkoContext _context;

        public ApiAddressController(DoEkoContext context)
        {
            _context = context;
        }

        // GET: api/ApiAddress
        [HttpGet]
        public IEnumerable<Address> GetAddresses()
        {
            return _context.Addresses;
        }

        // GET: api/ApiAddress/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddress([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var address = await _context.Addresses.SingleOrDefaultAsync(m => m.AddressId == id);

            if (address == null)
            {
                return NotFound();
            }

            return Ok(address);
        }

        // PUT: api/ApiAddress/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddress([FromRoute] int id, [FromBody] Address address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != address.AddressId)
            {
                return BadRequest();
            }

            _context.Entry(address).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ApiAddress
        [HttpPost]
        public async Task<IActionResult> PostAddress([FromBody] Address address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //_context.Addresses.Where(a=>a.)

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAddress", new { id = address.AddressId }, address);
        }

        // DELETE: api/ApiAddress/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var address = await _context.Addresses.SingleOrDefaultAsync(m => m.AddressId == id);
            if (address == null)
            {
                return NotFound();
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return Ok(address);
        }
        #region StatesDictionary
        [HttpGet]
        [AllowAnonymous]
        [Route("States")]
        public async Task<IActionResult> States()
        {
            return Ok(await _context.States.Select(s => new { id = s.StateId, text = s.Text }).ToListAsync());
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("States/{stateId:int}/Districts")]
        public async Task<IActionResult> GetDistrictsAsync([FromRoute] int stateId)
        {
            return Ok(await _context
                .Districts.Where(d => d.StateId == stateId)
                .Select(d => new { id = d.DistrictId, text = d.Text }).ToListAsync());
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("States/{stateId:int}/Districts/{districtId:int}/Communes")]
        public async Task<IActionResult> GetCommunesAsync([FromRoute][FromQuery] int stateId,int districtId)
        {
            return Ok(await _context
                .Communes
                .Where(c => c.StateId == stateId && c.DistrictId == districtId)
                .Select(d => new { id = d.CommuneId * 10 + (int)d.Type, text = d.FullName }).ToListAsync());
        }
        #endregion

        private bool AddressExists(int id)
        {
            return _context.Addresses.Any(e => e.AddressId == id);
        }
    }
}