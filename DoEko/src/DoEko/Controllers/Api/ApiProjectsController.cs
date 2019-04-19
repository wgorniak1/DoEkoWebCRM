using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DoEko.Models.DoEko;
using Microsoft.AspNetCore.Authorization;
using DoEko.Models.Identity;
using DoEko.Controllers.Extensions;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DoEko.Controllers.Helpers;
using DoEko.Models.DoEko.Survey;
using DoEko.Controllers.Api;
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
        [Route("{id}/totalcost/{surveyType?}/{rseType?}")]
        public async Task<IActionResult> GetTotalCost(
            [FromRoute] int id,
            [FromRoute] SurveyType? surveyType,
            [FromRoute] int? rseType,
            [FromQuery] int[] surveyStatus,
            [FromQuery] bool gross = false,
            [FromQuery] bool withChildProjects = false)
        {

            var hw = _context.SurveysHW
                .AsNoTracking()
                .Where(s => s.Investment.Contract.ProjectId == id &&
                            s.Status != SurveyStatus.Cancelled &&
                            s.ResultCalculation != null)
                .Select(s => new { s.Type, s.RSEType, s.Status, s.ResultCalculation.RSENetPrice, s.ResultCalculation.RSEGrossPrice });

            var ch = _context.SurveysCH
                .AsNoTracking()
                .Where(s => s.Investment.Contract.ProjectId == id &&
                            s.Status != SurveyStatus.Cancelled &&
                            s.ResultCalculation != null)
                .Select(s => new { s.Type, s.RSEType, s.Status, s.ResultCalculation.RSENetPrice, s.ResultCalculation.RSEGrossPrice });

            var en = _context.SurveysEN
                .AsNoTracking()
                .Where(s => s.Investment.Contract.ProjectId == id &&
                            s.Status != SurveyStatus.Cancelled &&
                            s.ResultCalculation != null)
                .Select(s => new { s.Type, s.RSEType, s.Status, s.ResultCalculation.RSENetPrice, s.ResultCalculation.RSEGrossPrice });

            if (surveyType.HasValue)
            {
                switch (surveyType.Value)
                {
                    case SurveyType.CentralHeating:
                        hw = null;
                        en = null;
                        break;
                    case SurveyType.HotWater:
                        ch = null;
                        en = null;
                        break;
                    case SurveyType.Energy:
                        ch = null;
                        hw = null;
                        break;
                    default:
                        break;
                }

                if (rseType.HasValue)
                {
                    switch (surveyType.Value)
                    {
                        case SurveyType.CentralHeating:
                            ch = ch?.Where(s => (int)s.RSEType == rseType.Value);
                            break;
                        case SurveyType.HotWater:
                            hw = hw?.Where(s => (int)s.RSEType == rseType.Value);
                            break;
                        case SurveyType.Energy:
                            en = en?.Where(s => (int)s.RSEType == rseType.Value);
                            break;
                        default:
                            break;
                    }
                }
            }

            if (surveyStatus.Count() != 0)
            {
                hw = hw?.Where(s => surveyStatus.Any(sts => sts == (int)s.Status));
                en = en?.Where(s => surveyStatus.Any(sts => sts == (int)s.Status));
                ch = ch?.Where(s => surveyStatus.Any(sts => sts == (int)s.Status));
            }

            double result = 0.00;

            if (hw != null)
            {
                try
                {
                    result +=
                    //    .GroupBy(s => new { s.RSEType, s.Status })
                    //    .Select(g => new
                    //{
                    //    g.First().Type,
                    //    g.Key.RSEType,
                    //    g.Key.Status,
                    //    NetPrice = g.Sum(c => c.RSENetPrice),
                    //    GrossPrice = g.Sum(c => c.RSEGrossPrice), })
                        //.Sum(i => gross == true ? i.GrossPrice : i.NetPrice);
                    await hw.SumAsync(s => gross == true ? s.RSEGrossPrice : s.RSENetPrice);
                }
                catch (Exception exc)
                {
                    _logger.LogDebug(exc.Message);
                }
            };
            if (ch != null)
            {
                try
                {
                    result += ch.Sum(s => gross == true ? s.RSEGrossPrice : s.RSENetPrice);

                }
                catch (Exception exc)
                {
                    _logger.LogDebug(exc.Message);
                }
            }
            if (en != null)
            {
                try
                {
                    result += en.Sum(s => gross == true ? s.RSEGrossPrice : s.RSENetPrice);
                }
                catch (Exception exc)
                {
                    _logger.LogDebug(exc.Message);
                }
            }

            return Ok(result);
        }

        [HttpGet()]
        [Route("{id}/totalpower/{surveyType?}/{rseType?}")]
        public async Task<IActionResult> GetTotalPower(
            [FromRoute] int id,
            [FromRoute] SurveyType? surveyType,
            [FromRoute] int? rseType,
            [FromQuery] int[] surveyStatus,
            [FromQuery] bool withChildProjects = false)
        {

            var hw = _context.SurveysHW
                .AsNoTracking()
                .Where(s => s.Investment.Contract.ProjectId == id &&
                            s.Status != SurveyStatus.Cancelled &&
                            s.ResultCalculation != null)
                .Select(s => new { s.Type, s.RSEType, s.Status, s.ResultCalculation.FinalRSEPower });

            var ch = _context.SurveysCH
                .AsNoTracking()
                .Where(s => s.Investment.Contract.ProjectId == id &&
                            s.Status != SurveyStatus.Cancelled &&
                            s.ResultCalculation != null)
                .Select(s => new { s.Type, s.RSEType, s.Status, s.ResultCalculation.FinalRSEPower });

            var en = _context.SurveysEN
                .AsNoTracking()
                .Where(s => s.Investment.Contract.ProjectId == id &&
                            s.Status != SurveyStatus.Cancelled &&
                            s.ResultCalculation != null)
                .Select(s => new { s.Type, s.RSEType, s.Status, s.ResultCalculation.FinalRSEPower });

            if (surveyType.HasValue)
            {
                switch (surveyType.Value)
                {
                    case SurveyType.CentralHeating:
                        hw = null;
                        en = null;                        
                        break;
                    case SurveyType.HotWater:
                        ch = null;
                        en = null;
                        break;
                    case SurveyType.Energy:
                        ch = null;
                        hw = null;
                        break;
                    default:
                        break;
                }

                if (rseType.HasValue)
                {
                    switch (surveyType.Value)
                    {
                        case SurveyType.CentralHeating:
                            ch = ch?.Where(g => (int)g.RSEType == rseType.Value);
                            break;
                        case SurveyType.HotWater:
                            hw = hw?.Where(g => (int)g.RSEType == rseType.Value);
                            break;
                        case SurveyType.Energy:
                            en = en?.Where(g => (int)g.RSEType == rseType.Value);
                            break;
                        default:
                            break;
                    }
                }
            }

            if (surveyStatus.Count() != 0)
            {
                hw = hw?.Where(g => surveyStatus.Any(sts => sts == (int)g.Status));
                en = en?.Where(g => surveyStatus.Any(sts => sts == (int)g.Status));
                ch = ch?.Where(g => surveyStatus.Any(sts => sts == (int)g.Status));
            }

            double result = 0.00;

            if (hw != null)
            {
                try
                {
                    result += await hw.SumAsync(i => i.FinalRSEPower);

                }
                catch (Exception exc)
                {
                    _logger.LogDebug(exc.Message);
                }
            };
            if (ch != null)
            {
                try
                {
                    result += await ch.SumAsync(i => i.FinalRSEPower);
                }
                catch (Exception exc)
                {
                    _logger.LogDebug(exc.Message);
                }
            }
            if (en != null)
            {
                try
                {
                    result += await en.SumAsync(i => i.FinalRSEPower);
                }
                catch (Exception exc)
                {
                    _logger.LogDebug(exc.Message);
                }
            }

            return Ok(result);
        }

        [HttpGet()]
        [Route("{id}/totalrse/{surveyType?}/{rseType?}")]
        public async Task<IActionResult> GetTotalSurveys(
            [FromRoute] int id, 
            [FromRoute] SurveyType? surveyType, 
            [FromRoute] int? rseType, 
            [FromQuery] int[] surveyStatus, 
            [FromQuery] bool withChildProjects = false)
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
        public async Task<IActionResult> Get( GetAllProjectsInputDto input )
        {
            var result = _context.Projects.AsQueryable();

            result = result.Include(p => p.ParentProject);
            result = result.Where(p => p.ProjectId != 1); //skip technical project

            var totalCount = result.Count();

            if (input.Status.HasValue)
            {
                var statuses = ParseQueryString(input.Status.Value);

                result = result.Where(p => ParseQueryString(input.Status.Value).Any(s => s == p.Status));

                //AUTHORIZATION FILTER!!!
            }
            if (!string.IsNullOrEmpty(input.Search))
            {
                var searchString = input.Search.ToUpperInvariant();

                result = result.Where(p => p.ShortDescription.ToUpperInvariant().Contains(searchString));
            }

            if (!string.IsNullOrEmpty(input.Sorting))
            {
                if (input.Sorting.Contains("total"))
                {

                }
                else
                {
                    result = result.OrderBy(input.Sorting);
                }
            }


            var filteredCount = result.Count();
            
            //
            result = result.Skip(input.SkipCount);
            result = result.Take(input.TotalCount);
            //

            //


            var list = await result.ToListAsync();
            //
            var res = new PagedResultDto<Project>(filteredCount, totalCount, list);
            //
            var response = this.Ok(res);

            return response;
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

    public class GetAllProjectsInputDto{
        public int SkipCount { get; set; } = 0;
        public int TotalCount { get; set; } = int.MaxValue;
        public string Sorting { get; set; } = nameof(Project.ProjectId) + " ASC";
        public ProjectStatusQuery? Status { get; set; } = ProjectStatusQuery.All;
        public string Search { get; set; } = "";
    }

    /// <summary>
    /// Implements <see cref="IPagedResult{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the items in the <see cref="ListResultDto{T}.Items"/> list</typeparam>
    [Serializable]
    public class PagedResultDto<T> : ListResultDto<T>, IPagedResult<T>
    {
        /// <summary>
        /// Total count of Items.
        /// </summary>
        public int TotalCount { get; set; }

        public int FilteredCount { get; set; }
        /// <summary>
        /// Creates a new <see cref="PagedResultDto{T}"/> object.
        /// </summary>
        public PagedResultDto()
        {

        }

        /// <summary>
        /// Creates a new <see cref="PagedResultDto{T}"/> object.
        /// </summary>
        /// <param name="totalCount">Total count of Items</param>
        /// <param name="items">List of items in current page</param>
        public PagedResultDto(int filteredCount,int totalCount, IReadOnlyList<T> items)
            : base(items)
        {
            TotalCount = totalCount;
            FilteredCount = filteredCount;
        }
    }

    /// <summary>
    /// Implements <see cref="IListResult{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the items in the <see cref="Items"/> list</typeparam>
    [Serializable]
    public class ListResultDto<T> : IListResult<T>
    {
        /// <summary>
        /// List of items.
        /// </summary>
        public IReadOnlyList<T> Items
        {
            get { return _items ?? (_items = new List<T>()); }
            set { _items = value; }
        }
        private IReadOnlyList<T> _items;

        /// <summary>
        /// Creates a new <see cref="ListResultDto{T}"/> object.
        /// </summary>
        public ListResultDto()
        {

        }

        /// <summary>
        /// Creates a new <see cref="ListResultDto{T}"/> object.
        /// </summary>
        /// <param name="items">List of items</param>
        public ListResultDto(IReadOnlyList<T> items)
        {
            Items = items;
        }
    }

    /// <summary>
    /// This interface is defined to standardize to return a list of items to clients.
    /// </summary>
    /// <typeparam name="T">Type of the items in the <see cref="Items"/> list</typeparam>
    public interface IListResult<T>
    {
        /// <summary>
        /// List of items.
        /// </summary>
        IReadOnlyList<T> Items { get; set; }
    }

    /// <summary>
    /// This interface is defined to standardize to return a page of items to clients.
    /// </summary>
    /// <typeparam name="T">Type of the items in the <see cref="IListResult{T}.Items"/> list</typeparam>
    public interface IPagedResult<T> : IListResult<T>, IHasTotalCount
    {

    }
    /// <summary>
    /// This interface is defined to standardize to set "Total Count of Items" to a DTO.
    /// </summary>
    public interface IHasTotalCount
    {
        /// <summary>
        /// Total count of Items.
        /// </summary>
        int TotalCount { get; set; }
    }
}
