using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DoEko.Models.DoEko;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.Identity;

namespace DoEko.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/v1/controlparameter")]
    [Authorize()]
    public class ApiControlParameterController : Controller
    {
        private readonly DoEkoContext _context;

        public ApiControlParameterController(DoEkoContext context)
        {
            _context = context;
        }

        // GET: api/ApiControlParameter
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ControlParameter>), 200)]
        public async Task<IActionResult> GetControlParametersAsync()
        {
            return Ok(await _context.Settings.ToListAsync());
        }

        // GET: api/ApiControlParameter/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ControlParameter), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetControlParameterAsync(int id)
        {
            var cp = await _context.Settings.FindAsync(id);

            if (cp == null)
            {
                return NotFound();
            }

            return Ok(cp);
        }
        
        // POST: api/ApiControlParameter
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(typeof(ControlParameter), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostcontrolParameterAsync([FromBody] ControlParameter controlParameter)
        {
            _context.Settings.Add(controlParameter);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetControlParameterAsync), 
                new { id = controlParameter.Id }, controlParameter);
        }
        
        // PUT: api/ApiControlParameter/5
        [HttpPut("{id}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> PutControlParameterAsync(int id, ControlParameter controlParameter)
        {
            if (id != controlParameter.Id)
            {
                return BadRequest();
            }

            _context.Entry(controlParameter).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteControlParameterAsync(int id)
        {
            var cp = await _context.Settings.FindAsync(id);
            if (cp == null)
            {
                return NotFound();
            }

            _context.Settings.Remove(cp);
            await _context.SaveChangesAsync();

            return  NoContent();
        }
    }
}
