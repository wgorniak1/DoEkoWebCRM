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
using Microsoft.AspNetCore.Identity;

namespace DoEko.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/v1/Survey")]
    [Authorize]
    public class ApiSurveyController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DoEkoContext _context;
        private readonly IFileStorage _fileStorage;

        public ApiSurveyController(DoEkoContext context, IFileStorage fileStorage, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _fileStorage = fileStorage;
            _userManager = userManager;
        }

        // GET: api/v1/Survey
        [HttpGet]
        public IEnumerable<Survey> GetSurveys([FromQuery] Guid? investmentId)
        {

            var qry = investmentId.HasValue ? _context.Surveys.Where(s => s.InvestmentId == investmentId) : _context.Surveys.AsQueryable(); 

            qry = qry
                .Include(s => s.Investment).ThenInclude(i => i.Contract).ThenInclude(c => c.Project)
                .Include(s => s.PlannedInstall)
                .Include(s => s.ResultCalculation);

            var result = qry.ToList();
            //TO JEST DO ZMIANY!!!
            foreach (var s in result)
            {
                if (s.ResultCalculation == null)
                {
                    s.ResultCalculation = new SurveyResultCalculations();
                }
                if (s.PlannedInstall == null)
                {
                    s.PlannedInstall = new SurveyDetPlannedInstall();
                }

                RSEPriceHelper r = new RSEPriceHelper(_context, true,s.Investment.Contract.ProjectId);

                r.Survey = s;

                s.ResultCalculation.RSENetPrice = Convert.ToDouble(r.Net);
                s.ResultCalculation.RSETax = Convert.ToDouble(r.Tax);
                s.ResultCalculation.RSEGrossPrice = Convert.ToDouble(r.Gross);
                s.ResultCalculation.RSEOwnerContrib = Convert.ToDouble(r.OwnerContribution);
            }
            foreach (var s in result)
            {
                s.PlannedInstall.Survey = null;
                s.ResultCalculation.Survey = null;
                s.Investment.Surveys = null;
                s.Investment.Contract.Investments = null;
                s.Investment.Contract.Project.Contracts = null;
            }

            return result;
        }

        [HttpGet]
        //[Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [Route("Neo")]
        [Authorize(Roles = Roles.Neo)]
        public async Task<IActionResult> GetSurveysNeo(int contractId, Guid? investmentId, int maxHits = 50)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));
            }
            catch (Exception)
            {
            }

            //
            NeoExtractHelper extract = new NeoExtractHelper(_context,_fileStorage);
            //
            extract.PrepareQuery(contractId, investmentId, maxHits);
            //
            int number = await extract.UpdateStatus();

            if (number > 0)
                return Ok(new { url = await extract.CreateFile() });
            else
                return BadRequest(new { error = "Brak nowych inwestycji do przeliczenia." });            
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