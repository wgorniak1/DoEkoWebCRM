using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.DoEko;
using Microsoft.AspNetCore.Authorization;

namespace DoEko.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/v1/Contract")]
    public class ApiContractController : Controller
    {
        private readonly DoEkoContext _context;

        public ApiContractController(DoEkoContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Clusters")]
        public async Task<IActionResult> GetClusterContracts()
        {
            return Ok(await _context.Contracts
                .Where(c => c.Type == ContractType.Cluster)
                .Select(c => new { id = c.ContractId, text = c.ClusterDetails.Commune.Text})
                .OrderBy(c => c.text)
                .ToListAsync());
        }

        // GET: api/ApiContract
        [HttpGet]
        public IEnumerable<Contract> GetContracts()
        {
            return _context.Contracts;
        }

        // GET: api/ApiContract/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContract([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contract = await _context.Contracts.SingleOrDefaultAsync(m => m.ContractId == id);

            if (contract == null)
            {
                return NotFound();
            }

            return Ok(contract);
        }

        // PUT: api/ApiContract/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContract([FromRoute] int id, [FromBody] Contract contract)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contract.ContractId)
            {
                return BadRequest();
            }

            _context.Entry(contract).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContractExists(id))
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

        // POST: api/ApiContract
        [HttpPost]
        public async Task<IActionResult> PostContract([FromBody] Contract contract)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //_context.
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContract", new { id = contract.ContractId }, contract);
        }

        // DELETE: api/ApiContract/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contract = await _context.Contracts.SingleOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();

            return Ok(contract);
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.ContractId == id);
        }
    }
}