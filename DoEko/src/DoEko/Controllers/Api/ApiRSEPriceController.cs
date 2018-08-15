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
using DoEko.ViewModels.RSEPriceSettingsViewModels;

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
        public IActionResult DeletePriceRules([FromQuery] int projectId, [FromBody] RSEPriceRule priceRule = null)
        {
            if (projectId < 1)
            {
                return BadRequest("Nieprawid³owy Id projektu");
            }

            if (priceRule == null)
            {
                foreach (var r in _context.RSEPriceRules.Where(r => r.ProjectId == projectId).ToList())
                {
                    _context.Entry(r).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                }

            }
            else
            {
                _context.Entry(priceRule).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            }


            _context.SaveChanges();

            return Ok();
        }


        [HttpGet]
        [Route("Price")]
        public async Task<IEnumerable<RSEPriceRule>> GetPriceRule([FromQuery] int projectId = 1, [FromQuery] bool getDefaults = true)
        {
            //Create default configuration if doesn't exists
            if (!(await _context.RSEPriceRules.AnyAsync(t => t.ProjectId == 1)))
            {
                RSEPriceRuleHelper.CreateDefaultConfiguration(_context);
            }

            //Return default configuration if specific doesn't exists
            if (await _context.RSEPriceRules.AnyAsync(t => t.ProjectId == projectId))
            {
                return await _context.RSEPriceRules.Where(t => t.ProjectId == projectId).ToListAsync();
            }
            else
            {
                var result = await _context.RSEPriceRules
                    .Where(t => t.ProjectId == 1)
                    .ToListAsync();

                //result.ForEach(t => t.ProjectId = projectId);

                return result;
            }

        }


        [HttpPost]
        [Route("Price")]
        public async Task<IActionResult> AddPriceRules([FromBody] RSEPriceRulesViewModel rSEPriceRulesViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.RSEPriceRules.AddRange(rSEPriceRulesViewModel.PriceRules);
                int result = await _context.SaveChangesAsync();

                return CreatedAtAction("/api/v1/RSERules/Price", new { projectId = rSEPriceRulesViewModel.ProjectId });

            }
            catch (Exception exc)
            {
                ModelState.Clear();
                ModelState.AddModelError("", exc.Message);
                return BadRequest(ModelState);
            }

        }

        [HttpPut]
        [Route("Price")]
        public async Task<IActionResult> UpdatePriceRules([FromBody] RSEPriceRulesViewModel rSEPriceRulesViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {

                    foreach (var group in rSEPriceRulesViewModel.PriceRules.GroupBy(
                        t => new
                        {
                            t.ProjectId,
                            t.SurveyType,
                            t.RSEType,
                            t.Unit
                        }))
                    {

                        var Old = _context.RSEPriceRules.Where(t =>
                        t.ProjectId == group.Key.ProjectId &&
                                 t.SurveyType == group.Key.SurveyType &&
                                 t.RSEType == group.Key.RSEType &&
                                 t.Unit == group.Key.Unit).ToList();

                        _context.RSEPriceRules.RemoveRange(Old);
                        int result = _context.SaveChanges();

                        _context.RSEPriceRules.AddRange(group.ToList());
                        result = _context.SaveChanges();
                    }


                    transaction.Commit();
                    return CreatedAtAction("/api/v1/RSERules/Price", new { projectId = rSEPriceRulesViewModel.ProjectId });
                }
                catch (Exception exc)
                {
                    transaction.Rollback();
                    //!!!!!!!!!!!!!!!!!!!!! this is to set fake "ok" status of entities that failed during db update
                    //!!!!!!!!!!!!!!!!!!!!! and let the loop continue creation of other data
                    _context.ChangeTracker.AcceptAllChanges();

                    ModelState.Clear();
                    ModelState.AddModelError("", exc.Message);
                    return BadRequest(ModelState);

                }
            }
        }




        [HttpDelete]
        [Route("Tax")]
        public IActionResult DeleteTaxRules([FromQuery] int projectId, [FromBody] RSEPriceTaxRule taxRule = null)
        {
            if (projectId <= 1)
            {
                return BadRequest("Nieprawid³owy Id projektu");
            }

            if (taxRule == null)
            {
                foreach (var r in _context.RSEPriceTaxRules.Where(r => r.ProjectId == projectId).ToList())
                {
                    _context.Entry(r).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                }
                
            }
            else
            {
                _context.Entry(taxRule).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
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
                var result = await _context.RSEPriceTaxRules
                    .Where(t => t.ProjectId == 1)
                    .ToListAsync();

                //result.ForEach(t => t.ProjectId = projectId);

                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rSEPriceTaxRule"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Tax")]
        public async Task<IActionResult> AddTaxRules([FromBody] RSEPriceTaxRulesViewModel rSEPriceTaxRulesViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.RSEPriceTaxRules.AddRange(rSEPriceTaxRulesViewModel.TaxRules);
                int result = await _context.SaveChangesAsync();

                return CreatedAtAction("/api/v1/RSERules/Tax", new { projectId = rSEPriceTaxRulesViewModel.ProjectId });
                
            }
            catch (Exception exc)
            {
                ModelState.Clear();
                ModelState.AddModelError("", exc.Message);
                return BadRequest(ModelState);
            }

        }

        [HttpPut]
        [Route("Tax")]
        public async Task<IActionResult> UpdateTaxRules([FromBody] RSEPriceTaxRulesViewModel rSEPriceTaxRulesViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {

                    foreach (var group in rSEPriceTaxRulesViewModel.TaxRules.GroupBy(
                        t => new
                        {
                            t.ProjectId,
                            t.SurveyType,
                            t.RSEType,
                            t.InstallationLocalization,
                            t.BuildingPurpose
                        }))
                    {

                        var Old = _context.RSEPriceTaxRules.Where(t =>
                        t.ProjectId == group.Key.ProjectId &&
                                 t.SurveyType == group.Key.SurveyType &&
                                 t.RSEType == group.Key.RSEType &&
                                 t.InstallationLocalization == group.Key.InstallationLocalization &&
                                 t.BuildingPurpose == group.Key.BuildingPurpose).ToList();

                        _context.RSEPriceTaxRules.RemoveRange(Old);
                        int result = _context.SaveChanges();

                        _context.RSEPriceTaxRules.AddRange(group.ToList());
                        result = _context.SaveChanges();
                    }


                    transaction.Commit();
                    return CreatedAtAction("/api/v1/RSERules/Tax", new { projectId = rSEPriceTaxRulesViewModel.ProjectId });
                }
                catch (Exception exc)
                {
                    transaction.Rollback();
                    //!!!!!!!!!!!!!!!!!!!!! this is to set fake "ok" status of entities that failed during db update
                    //!!!!!!!!!!!!!!!!!!!!! and let the loop continue creation of other data
                    _context.ChangeTracker.AcceptAllChanges();

                    ModelState.Clear();
                    ModelState.AddModelError("", exc.Message);
                    return BadRequest(ModelState);

                }
            }
        }

    }
}