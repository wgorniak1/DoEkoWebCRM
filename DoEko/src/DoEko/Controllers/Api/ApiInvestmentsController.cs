using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.DoEko;

namespace DoEko.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/ApiInvestments")]
    public class ApiInvestmentsController : Controller
    {
        private readonly DoEkoContext _context;

        public ApiInvestmentsController(DoEkoContext context)
        {
            _context = context;
        }

        // GET: api/ApiInvestments
        [HttpGet]
        public IEnumerable<Investment> GetInvestments()
        {
            return _context.Investments;
        }

        // GET: api/ApiInvestments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvestment([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var investment = await _context.Investments.SingleOrDefaultAsync(m => m.InvestmentId == id);

            if (investment == null)
            {
                return NotFound();
            }

            return Ok(investment);
        }

        // PUT: api/ApiInvestments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvestment([FromRoute] Guid id, [FromBody] Investment investment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != investment.InvestmentId)
            {
                return BadRequest();
            }

            _context.Entry(investment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvestmentExists(id))
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

        // POST: api/ApiInvestments
        [HttpPost]
        public async Task<IActionResult> PostInvestment([FromBody] Investment investment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Investments.Add(investment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInvestment", new { id = investment.InvestmentId }, investment);
        }

        // DELETE: api/ApiInvestments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvestment([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var investment = await _context.Investments.SingleOrDefaultAsync(m => m.InvestmentId == id);
            if (investment == null)
            {
                return NotFound();
            }

            _context.Investments.Remove(investment);
            await _context.SaveChangesAsync();

            return Ok(investment);
        }

        private bool InvestmentExists(Guid id)
        {
            return _context.Investments.Any(e => e.InvestmentId == id);
        }
    }
}