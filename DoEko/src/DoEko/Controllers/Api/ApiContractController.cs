using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.DoEko;
using Microsoft.AspNetCore.Authorization;
using DoEko.Models.Identity;

namespace DoEko.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/v1/Contract")]
    [Authorize]
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
        public async Task<IActionResult> GetClusterContracts([FromQuery] string cluster)
        {
            return Ok(await _context.Contracts
                .Where(c => c.Type == ContractType.Cluster && c.Project.ShortDescription == cluster)
                .Select(c => new { id = c.ContractId, text = c.ClusterDetails.Commune.Text })
                .OrderBy(c => c.text)
                .ToListAsync());
        }

        // GET: api/ApiContract
        [HttpGet]
        public IEnumerable<Contract> GetContracts()
        {
            return _context.Contracts;
        }

        [HttpGet]
        [Route("Neo")]
        public IEnumerable<object> GetContractsNeo()
        {
            var model = new List<object>();

            var contracts = _context.Contracts
                .Include(c => c.Project)
                .Include(c => c.Investments)
                .Where(c => c.Project.Status != ProjectStatus.Closed && 
                            c.Type != ContractType.Cluster );

            foreach (var c in contracts)
            {
                model.Add(new
                {
                    Project = c.Project.ShortDescription,
                    Number = c.Number,
                    ContractId = c.ContractId,
                    InvestmentNo = c.Investments.Count()
                });
            }

            return model;
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

        [HttpPut("{id}")]
        public async Task<IActionResult> SetStatus([FromRoute] int id, [FromBody] Contract contract)
        {
            if (!ContractExists(id))
            {
                return BadRequest();
            }
            if (id != contract.ContractId)
            {
                return BadRequest();
            }

            Contract c = _context.Contracts.Single(co => co.ContractId == id);

            if (contract.Status != c.Status)
            {
                if (contract.Status == ContractStatus.Completed)
                {
                    //cascade udpate of status of all investments and surveys
                    var surveys = _context.Surveys.Where(s => s.Investment.ContractId == c.ContractId);

                    foreach (var item in await surveys.ToListAsync())
                    {
                        //
                    }
                }
            }

            return NoContent();

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