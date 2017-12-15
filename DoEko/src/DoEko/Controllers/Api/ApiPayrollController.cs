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
using DoEko.ViewModels.PayrollViewModels;
using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;

namespace DoEko.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/v1/Payroll")]
    [Authorize(Roles = Roles.Admin)]
    public class ApiPayrollController : Controller
    {
        private readonly DoEkoContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApiPayrollController(DoEkoContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/v1/Payroll
        [HttpGet]
        [Route("Cluster")]
        public async Task<IEnumerable<PayrollClusterVM>> GetPayrollCluster()
        {
            return await _context.PayrollCluster.Cast<PayrollClusterVM>().ToListAsync();
        }


        // GET: api/ApiPayroll/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayrollCluster([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var payrollCluster = await _context.PayrollCluster.SingleOrDefaultAsync(m => m.PayrollClusterId == id);

            if (payrollCluster == null)
            {
                return NotFound();
            }

            return Ok(payrollCluster);
        }

        // PUT: api/ApiPayroll/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPayrollCluster([FromRoute] Guid id, [FromBody] PayrollCluster payrollCluster)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != payrollCluster.PayrollClusterId)
            {
                return BadRequest();
            }

            _context.Entry(payrollCluster).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PayrollClusterExists(id))
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

        // POST: api/ApiPayroll
        [HttpPost]
        public async Task<IActionResult> PostPayrollCluster([FromBody] PayrollCluster payrollCluster)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.PayrollCluster.Add(payrollCluster);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPayrollCluster", new { id = payrollCluster.PayrollClusterId }, payrollCluster);
        }

        // DELETE: api/ApiPayroll/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayrollCluster([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var payrollCluster = await _context.PayrollCluster.SingleOrDefaultAsync(m => m.PayrollClusterId == id);
            if (payrollCluster == null)
            {
                return NotFound();
            }

            _context.PayrollCluster.Remove(payrollCluster);
            await _context.SaveChangesAsync();

            return Ok(payrollCluster);
        }

        private bool PayrollClusterExists(Guid id)
        {
            return _context.PayrollCluster.Any(e => e.PayrollClusterId == id);
        }
    }
}