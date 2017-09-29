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

namespace DoEko.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/WageTypeDefinitions")]
    [Authorize]
    public class WageTypeDefinitionsController : Controller
    {
        private readonly DoEkoContext _context;

        public WageTypeDefinitionsController(DoEkoContext context)
        {
            _context = context;
        }

        // GET: api/WageTypeDefinitions
        [HttpGet]
        public async Task<IActionResult> GetWageTypeCatalog()
        {
            return Ok(await _context.WageTypeCatalog.ToListAsync());
        }

        // GET: api/WageTypeDefinitions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWageTypeDefinition([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var wageTypeDefinition = await _context.WageTypeCatalog.SingleOrDefaultAsync(m => m.WageTypeDefinitionId == id);

            if (wageTypeDefinition == null)
            {
                return NotFound();
            }

            return Ok(wageTypeDefinition);
        }

        // PUT: api/WageTypeDefinitions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWageTypeDefinition([FromRoute] int id, [FromBody] WageTypeDefinition wageTypeDefinition)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != wageTypeDefinition.WageTypeDefinitionId)
            {
                return BadRequest();
            }

            _context.Entry(wageTypeDefinition).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WageTypeDefinitionExists(id))
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

        // POST: api/WageTypeDefinitions
        [HttpPost]
        public async Task<IActionResult> PostWageTypeDefinition([FromBody] WageTypeDefinition wageTypeDefinition)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.WageTypeCatalog.Add(wageTypeDefinition);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWageTypeDefinition", new { id = wageTypeDefinition.WageTypeDefinitionId }, wageTypeDefinition);
        }

        // DELETE: api/WageTypeDefinitions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWageTypeDefinition([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var wageTypeDefinition = await _context.WageTypeCatalog.SingleOrDefaultAsync(m => m.WageTypeDefinitionId == id);
            if (wageTypeDefinition == null)
            {
                return NotFound();
            }

            _context.WageTypeCatalog.Remove(wageTypeDefinition);
            await _context.SaveChangesAsync();

            return Ok(wageTypeDefinition);
        }

        private bool WageTypeDefinitionExists(int id)
        {
            return _context.WageTypeCatalog.Any(e => e.WageTypeDefinitionId == id);
        }
    }
}