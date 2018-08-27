using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DoEko.Models.DoEko;
using Microsoft.AspNetCore.Authorization;
using DoEko.Models.Identity;
using DoEko.Controllers.Extensions;

using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DoEko.Controllers.Helpers;
using DoEko.Models.DoEko.Survey;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DoEko.Controllers.Api
{
    public enum ProjectStatusQuery
    {
        New = ProjectStatus.New,
        InProgress = ProjectStatus.InProgress,
        Verified = ProjectStatus.Verified,
        Completed = ProjectStatus.Completed,
        Closed = ProjectStatus.Closed,
        All,
        NotClosed,
        NotCompleted,
    }

    [Authorize()]
    [Route("api/v1/Projects")]
    public class ApiProjectsController : Controller
    {
        private readonly ILogger _logger;
        private readonly DoEkoContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ResourceAuthorizationHelper _authHelper;


        public ApiProjectsController(DoEkoContext doEkoContext, ILoggerFactory loggerFactory, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = doEkoContext;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _authHelper = new ResourceAuthorizationHelper(_context);
        }

        [HttpGet()]
        [Route("{id}/totalrse/{surveyType?}/{rseType?}")]
        public async Task<IActionResult> GetTotalSurveys([FromRoute] int id, [FromRoute] SurveyType? surveyType, [FromRoute] int? rseType, [FromQuery] IQueryable<int> surveyStatus, [FromQuery] bool withChildProjects = false)
        {
            int totals = 0;

            var query = _context.Surveys.Where(s => s.Investment.Contract.ProjectId == id);

            if (surveyType.HasValue)
            {
                query = query.Where(s => s.Type == surveyType);

                if (rseType.HasValue)
                {
                    switch (surveyType.Value)
                    {
                        case SurveyType.CentralHeating:
                            query = query.Where(s => (int)((SurveyCentralHeating)s).RSEType == rseType.Value);
                            break;
                        case SurveyType.HotWater:
                            query = query.Where(s => (int)((SurveyHotWater)s).RSEType == rseType.Value);
                            break;
                        case SurveyType.Energy:
                            query = query.Where(s => (int)((SurveyEnergy)s).RSEType == rseType.Value);
                            break;
                        default:
                            break;
                    }
                }
            }

            if (surveyStatus.Count() != 0)
            {
                query = query.Where(s => surveyStatus.Any(sts => sts == (int)(s.Status)));
            }

            totals = await query.CountAsync();

            return Ok(totals);
        }
        
        [HttpGet()]
        public async Task<IActionResult> Get([FromQuery] ProjectStatusQuery? projectStatusQuery)
        {
            var result = _context.Projects.AsQueryable();

            result = result.Include(p => p.ParentProject);
            result = result.Where(p => p.ProjectId != 1); //skip technical project

            if (projectStatusQuery.HasValue)
            {
                var statuses = ParseQueryString(projectStatusQuery);

                result = result.Where(p => ParseQueryString(projectStatusQuery).Any(s => s == p.Status));

                //AUTHORIZATION FILTER!!!
            }


            return Ok(await result.ToListAsync());
        }


        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {


            if (!this.ProjectExists(id))
            {
                return NotFound(id);
            }

            ApplicationUser applicationUser = await _userManager.GetUserAsync(this.User);
            if (!await _authHelper.CheckAuthAsync(applicationUser, AccessType.Read, ResourceType.Project, id))
            {
                return NotFound(id);
            }
            
            try
            {
                var project = await _context.Projects.SingleAsync(p => p.ProjectId == id);

                return Ok(project);
            }
            catch (Exception exception)
            {
                ModelState.Clear();
                ModelState.AddModelError("", exception, this.MetadataProvider.GetMetadataForType(typeof(Project)));
                return BadRequest(ModelState);

            }

            
        }
    
        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Project project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser applicationUser = await _userManager.GetUserAsync(this.User);
            if (!await _authHelper.CheckAuthAsync(applicationUser, AccessType.Create, ResourceType.Project))
            {
                return Unauthorized();
            }

            try
            {
                _context.Projects.Add(project);

                int result = await _context.SaveChangesAsync();

                return CreatedAtAction(this.Request.Path, new { id = project.ProjectId }, project);
            }
            catch (Exception exception)
            {
                ModelState.Clear();
                ModelState.AddModelError("", exception, this.MetadataProvider.GetMetadataForType(typeof(Project)));
                return BadRequest(ModelState);
            }

        }
    

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Project project)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != project.ProjectId)
            {
                return BadRequest();
            }

            ApplicationUser applicationUser = await _userManager.GetUserAsync(this.User);
            if (!await _authHelper.CheckAuthAsync(applicationUser, AccessType.Update, ResourceType.Project,id))
            {
                return Unauthorized();
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                {
                    return NotFound(id);
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                if (this.ProjectExists(id))
                {
                    ApplicationUser applicationUser = await _userManager.GetUserAsync(this.User);

                    if (!await _authHelper.CheckAuthAsync(applicationUser, AccessType.Delete, ResourceType.Project,id))
                    {
                        return Unauthorized();
                    }

                    _context.Projects.Remove(_context.Projects.Single(p => p.ProjectId == id));
                    int result = await _context.SaveChangesAsync();

                    return NoContent();
                }
                else
                {
                    return NotFound(id);
                }
            }
            catch (Exception exception)
            {
                ModelState.Clear();
                ModelState.AddModelError("", exception, this.MetadataProvider.GetMetadataForType(typeof(Project)));
                return BadRequest(ModelState);
            }
        }
        #region Dictionary
        
        [HttpGet]
        [Route("Dictionary/Status")]
        public IActionResult GetProjectStatuses()
        {
            return Ok(EnumHelper.GetKeyValuePairs(typeof(ProjectStatus)).ToList());
        }

        #endregion
        #region helpers
        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(p => p.ProjectId == id);
        }

        private IEnumerable<ProjectStatus> ParseQueryString(ProjectStatusQuery? projectStatusQuery)
        {
            IList<ProjectStatus> statuses = new List<ProjectStatus>();

            if (projectStatusQuery.HasValue)
            {
                switch (projectStatusQuery.Value)
                {
                    case ProjectStatusQuery.All:
                        foreach (var item in Enum.GetValues(typeof(ProjectStatus)))
                        {
                            statuses.Add((ProjectStatus)item);
                        }
                        break;
                        
                    case ProjectStatusQuery.NotClosed:
                        foreach (var item in Enum.GetValues(typeof(ProjectStatus)))
                        {
                            statuses.Add((ProjectStatus)item);
                        }
                        statuses.Remove(ProjectStatus.Closed);
                        break;
                    case ProjectStatusQuery.NotCompleted:
                        foreach (var item in Enum.GetValues(typeof(ProjectStatus)))
                        {
                            statuses.Add((ProjectStatus)item);
                        }
                        
                        statuses.Remove(ProjectStatus.Closed);
                        statuses.Remove(ProjectStatus.Completed);
                        break;
                    default:
                        statuses.Add((ProjectStatus)projectStatusQuery.Value);
                        break;
                }
            }
            else
            {
                foreach (var item in Enum.GetValues(typeof(ProjectStatus)))
                {
                    statuses.Add((ProjectStatus)item);
                }
            }

            return statuses;
        }
        #endregion helpers
    }
}
