using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.DoEko;
using DoEko.Models.DoEko.Survey;
using Microsoft.AspNetCore.Authorization;
using DoEko.Models.Identity;
using DoEko.Controllers.Helpers;
using DoEko.Services;
using System.IO;
using DoEko.ViewModels.API.SurveyViewModels;
using DoEko.Controllers.Extensions;

namespace DoEko.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/v1/Survey")]
    [Authorize]
    public class ApiSurveyController : Controller
    {
        private readonly DoEkoContext _context;
        private readonly IFileStorage _fileStorage;

        public ApiSurveyController(DoEkoContext context, IFileStorage fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        // GET: api/ApiSurvey
        [HttpGet]
        public IEnumerable<Survey> GetSurveys()
        {
            return _context.Surveys;
        }

        [HttpGet]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [Route("Neo")]
        [Authorize(Roles = Roles.Neo)]
        public async Task<IActionResult> GetSurveysNeo(int contractId, Guid? investmentId)
        {
            //number of investments to download per day
            int maxHits = 50;
            //
            var sl = _context.Surveys.AsQueryable();

            sl = sl.Where(s => s.Investment.ContractId == contractId && 
                               s.Investment.Status == InvestmentStatus.Initial &&
                               s.Investment.Calculate == true );

            sl = investmentId.HasValue ? sl.Where(s => s.InvestmentId == investmentId) : sl;

            sl = sl.OrderBy(s => s.InvestmentId);
            
            sl = sl.Include(s => s.Investment).ThenInclude(i => i.InvestmentOwners).ThenInclude(io => io.Owner).ThenInclude(o => o.Address)
                   .Include(s => s.Investment).ThenInclude(i => i.Address).ThenInclude(a=>a.State)
                   .Include(s => s.Investment).ThenInclude(i => i.Address).ThenInclude(a=>a.District)
                   .Include(s => s.Investment).ThenInclude(i => i.Address).ThenInclude(a=>a.Commune)
                   .Include(s => s.AirCondition)
                   .Include(s => s.Audit)
                   .Include(s => s.BathRoom)
                   .Include(s => s.BoilerRoom)
                   .Include(s => s.Building)
                   .Include(s => s.Ground)
                   .Include(s => s.PlannedInstall)
                   .Include(s => s.RoofPlanes)
                   .Include(s => s.Wall);

            var result = await sl.ToListAsync();
            var investments = result
                .Select(s => s.Investment)
                .Distinct()
                .Take(maxHits);

            MemoryStream report = investments 
                .SelectMany(i => i.Surveys)
                .Select(s => new SurveyNeoVM(s, _fileStorage))
                .ToList()
                .AsDataTable()
                .AsExcel();
            
            string fileName = "OZEDoAnalizy_" + DateTime.Now.ToLongDateString() + ".xlsx";

            return File(report.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

        }

        // GET: api/ApiSurvey/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSurvey([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var survey = await _context.Surveys.SingleOrDefaultAsync(m => m.SurveyId == id);

            if (survey == null)
            {
                return NotFound();
            }

            return Ok(survey);
        }

        // PUT: api/ApiSurvey/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSurvey([FromRoute] Guid id, [FromBody] Survey survey)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != survey.SurveyId)
            {
                return BadRequest();
            }

            _context.Entry(survey).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SurveyExists(id))
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

        // POST: api/ApiSurvey
        [HttpPost]
        public async Task<IActionResult> PostSurvey([FromBody] Survey survey)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Surveys.Add(survey);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSurvey", new { id = survey.SurveyId }, survey);
        }


        // DELETE: api/ApiSurvey/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSurvey([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var survey = await _context.Surveys.SingleOrDefaultAsync(m => m.SurveyId == id);
            if (survey == null)
            {
                return NotFound();
            }

            _context.Surveys.Remove(survey);
            await _context.SaveChangesAsync();

            return Ok(survey);
        }

        private bool SurveyExists(Guid id)
        {
            return _context.Surveys.Any(e => e.SurveyId == id);
        }
    }
}