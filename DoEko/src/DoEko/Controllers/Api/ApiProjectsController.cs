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

        public ApiProjectsController(DoEkoContext doEkoContext, ILoggerFactory loggerFactory)
        {
            _context = doEkoContext;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        [HttpGet()]
        public async Task<IActionResult> Get([FromQuery] ProjectStatusQuery? projectStatusQuery)
        {
            var result = _context.Projects.AsQueryable();

            if (projectStatusQuery.HasValue)
            {
                var statuses = ParseQueryString(projectStatusQuery);

                result = result.Where(p => ParseQueryString(projectStatusQuery).Any(s => s == p.Status));
            }

            return Ok(await result.ToListAsync());
        }


        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var project = await _context.Projects.SingleAsync(p => p.ProjectId == id);

                return Ok(project);
            }
            catch (Exception)
            {
                return NotFound(id);
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
            try
            {
                _context.Projects.Add(project);

                int result = await _context.SaveChangesAsync();

            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
                return BadRequest(ModelState);
            }

            return CreatedAtAction(this.Request.Path, new { id = project.ProjectId }, project);
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

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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
            return NoContent();
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public void Delete(int id)
        {

        }

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
