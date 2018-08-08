using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DoEko.Models.DoEko;
using DoEko.Models.DoEko.Survey;
using DoEko.Controllers.Helpers;
using Microsoft.EntityFrameworkCore;

namespace DoEko.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/v1/RSERules")]
    [Authorize()]
    public class ApiRSEPriceController : Controller
    {
        private readonly DoEkoContext _context;

        public ApiRSEPriceController(DoEkoContext context)
        {
            _context = context;
        }

        [HttpDelete]
        [Route("Price")]
        public IActionResult DeletePriceRules(int projectId)
        {
            if (projectId < 1)
                return BadRequest("Nieprawid³owy Id projektu");

            foreach (var r in _context.RSEPriceRules.Where(r => r.ProjectId == projectId).ToList())
            {
                _context.Entry(r).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            }
            
            _context.SaveChanges();
            
            return Ok();
        }

        [HttpGet]
        [Route("Price")]
        public IEnumerable<RSEPriceRule> GetPriceRule([FromQuery] int projectId = 1)
        {
            var qry = _context.RSEPriceRules.Where(r => r.ProjectId == projectId);

            ICollection<RSEPriceRule> rules = qry.ToList();
            if (rules.Count == 0 && projectId == 1)
            {
                RSEPriceRuleHelper.CreateDefaultConfiguration(_context);

                rules = qry.ToList();
            }

            return rules;
        }

        [HttpDelete]
        [Route("Tax")]
        public IActionResult DeleteTaxRules([FromQuery] int projectId, RSEPriceTaxRule rSEPriceTaxRule = null)
        {
            if (projectId <= 1)
            {
                return BadRequest("Nieprawid³owy Id projektu");
            }

            if (rSEPriceTaxRule == null)
            {
                foreach (var r in _context.RSEPriceTaxRules.Where(r => r.ProjectId == projectId).ToList())
                {
                    _context.Entry(r).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                }
                
            }
            else
            {
                _context.Entry(rSEPriceTaxRule).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            }
            

            _context.SaveChanges();

            return Ok();
        }


        [HttpGet]
        [Route("Tax")]
        public async Task<IEnumerable<RSEPriceTaxRule>> GetTaxRulesAsync([FromQuery] int projectId = 1,[FromQuery] bool getDefaults = true)
        {
            //Create default configuration if doesn't exists
            if (!(await _context.RSEPriceTaxRules.AnyAsync(t => t.ProjectId == 1)))
            {
                RSEPriceTaxRuleHelper.CreateDefaultConfiguration(_context);
            }

            //Return default configuration if specific doesn't exists
            if (await _context.RSEPriceTaxRules.AnyAsync(t => t.ProjectId == projectId))
            {
                return await _context.RSEPriceTaxRules.Where(t => t.ProjectId == projectId).ToListAsync();
            }            
            else
            {
                return await _context.RSEPriceTaxRules.Where(t => t.ProjectId == 1).ToListAsync();
            }
        }

    }
}