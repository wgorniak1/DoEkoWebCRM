using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.DoEko;
using Microsoft.AspNetCore.Authorization;
using DoEko.Controllers.Extensions;

namespace DoEko.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/v1/Investments")]
    [Authorize()]
    public class InvestmentsController : Controller
    {
        private readonly DoEkoContext _context;

        public InvestmentsController(DoEkoContext context)
        {
            _context = context;
        }

        // GET: api/ApiInvestments
        [HttpGet]
        public IEnumerable<Investment> GetInvestments([FromQuery] int contractId)
        {
            return _context.Investments.Where(i => i.ContractId == contractId);
        }

        // GET: api/ApiInvestments
        [HttpGet]
        [Route("Neo")]
        public IEnumerable<object> GetInvestmentsNeo([FromQuery] int contractId)
        {

            var Surveys = _context.Surveys
                .Include(s => s.ResultCalculation)
                .Include(s => s.Investment).ThenInclude(i => i.Address)
                .Where(s => s.Investment.ContractId == contractId &&
                            (s.Investment.Status == InvestmentStatus.Initial ||
                             s.Investment.Status == InvestmentStatus.InReview) &&
                            (s.Investment.InspectionStatus == InspectionStatus.Submitted ||
                             s.Investment.InspectionStatus == InspectionStatus.Approved)).ToList();

            var Investments = Surveys.Select(s => s.Investment).Distinct().ToList();

            var model = new List<object>();

            foreach (var i in Investments)
            {

                var srvs = i.Surveys.Select(s => new {
                    SurveyId    = s.SurveyId,
                    Type        = s.Type.DisplayShortName(),
                    RSEType     = s.GetRSETypeName(),
                    Status      = s.Status.DisplayName(),
                    FinalPower  = s.ResultCalculation == null ? 0 : s.ResultCalculation.FinalRSEPower,
                    isCompleted = s.ResultCalculation == null ? false : s.ResultCalculation.Completed
                }).ToArray();

                //
                model.Add(new {
                    InvestmentId = i.InvestmentId,
                    Address  = new string[] { i.Address.FirstLine, i.Address.SecondLine },
                    PlotNumber = i.PlotNumber,
                    Status = i.Status.DisplayName(),
                    InspectionStatus = i.InspectionStatus.DisplayName(),
                    Calculate = i.Calculate.AsYesNo(),
                    Surveys = srvs
                });
            }

            return model;
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
            try
            {
                _context.Investments.Add(investment);
                await _context.SaveChangesAsync();

            }
            catch (Exception exc)
            {
                ModelState.AddModelError("SAVE", exc.InnerException != null ? exc.InnerException.Message : exc.Message);
                return BadRequest(ModelState);
            }

            return CreatedAtAction("PostInvestment", new { id = investment.InvestmentId }, investment);
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