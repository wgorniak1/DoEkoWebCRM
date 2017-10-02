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

        [HttpPost]
        [Route("Start")]
        public async Task<IActionResult> Start(DateTime Period)
        {
            Period = new DateTime(2017, 09, 01);

            var periodFrom = new DateTime(Period.Year, Period.Month, 1);
            var periodTo = new DateTime(Period.Year, Period.Month, 1).AddMonths(1).AddDays(-1);

            var payrollResults = new Collection<PayrollCluster>();

            var stats = _context.SurveyStatusHistory
                .Where(s => s.Status == Models.DoEko.Survey.SurveyStatus.Approval &&
                          s.Start >= periodFrom &&
                          s.Start <= periodTo);
            foreach (var item in stats)
            {
                //get contractId
                var InvestmentId = _context
                    .Surveys.Where(s => s.SurveyId == item.SurveyId)
                    .Select(s => s.InvestmentId).FirstOrDefault();
                var contractId = _context.Investments
                    .Where(i => i.InvestmentId == InvestmentId)
                    .Select(i => i.ContractId).FirstOrDefault();

                //get employee for the user
                var EmployeeUser = await _context.EmployeesUsers
                    .Include(eu=>eu.Employee).ThenInclude(e=>e.BasicPay)
                    .Where(eu => eu.UserId == item.UserId &&
                                 eu.Start <= item.Start &&
                                 eu.End >= item.Start).FirstOrDefaultAsync();
                if (EmployeeUser == null)
                {
                    continue;
                }
                //get rate for the employee
                var basicRate = EmployeeUser.Employee.BasicPay.Where(bp => bp.Code == "BASE" &&
                                                                           bp.Start <= periodFrom &&
                                                                           bp.End >= periodFrom &&
                                                                           bp.ContractId.HasValue && 
                                                                           bp.ContractId.Value == contractId).FirstOrDefault();
                if (basicRate == null)
                {
                    basicRate = EmployeeUser.Employee.BasicPay.Where(bp => bp.Code == "BASE" &&
                                                                           bp.Start <= periodFrom &&
                                                                           bp.End >= periodFrom &&
                                                                           !bp.ContractId.HasValue ).FirstOrDefault();
                }

                var employeeResult = payrollResults.FirstOrDefault(w => w.EmployeeId == EmployeeUser.Employee.EmployeeId);
                if (employeeResult == null)
                {
                    employeeResult = new PayrollCluster()
                    {
                        EmployeeId = EmployeeUser.EmployeeId,
                        PeriodFor = periodFrom,
                        PeriodIn = periodFrom,
                        SequenceNo = 1,
                        Results = new Collection<PayrollResult>()
                    };
                    payrollResults.Add(employeeResult);
                }

                employeeResult.Results.Add(new PayrollResult
                {
                    Code = basicRate.Code,
                    ShortDescription = basicRate.ShortDescription,
                    Rate = basicRate.Rate,
                    Number = 1,
                    Unit = basicRate.Unit,
                    Amount = basicRate.Rate * 1,
                    Currency = basicRate.Currency,
                    SurveyId = item.SurveyId
                });
            }

            //save results
            try
            {
                _context.PayrollCluster.AddRange(payrollResults);
                int result = await _context.SaveChangesAsync();


            }
            catch (Exception exc)
            {
                return Ok(exc);
            }

            return Ok();
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