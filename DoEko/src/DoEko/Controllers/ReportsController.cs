using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DoEko.Models.DoEko;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DoEko.Models.Identity;
using DoEko.Models.DoEko.Addresses;
using DoEko.Controllers.Helpers;
using DoEko.Models.DoEko.Survey;
using DoEko.ViewModels.SurveyViewModels;
using DoEko.ViewModels.TestViewModels;
using DoEko.Services;
using Microsoft.WindowsAzure.Storage.Blob;
using DoEko.Controllers.Extensions;
using DoEko.ViewModels.ReportsViewModels;
using Microsoft.AspNetCore.Authorization;

namespace DoEko.Controllers
{
    [Authorize(Roles = Roles.Admin + "," + Roles.User)]
    public class ReportsController : Controller
    {
        private DoEkoContext _context;
        private UserManager<ApplicationUser> _userManager;
        private IFileStorage _fileStorage;
        public ReportsController(DoEkoContext context, UserManager<ApplicationUser> userManager, IFileStorage fileStorage)
        {
            _context = context;
            _userManager = userManager;
            _fileStorage = fileStorage;
        }

        [HttpGet]
        public async Task<IActionResult> SurveyExtract()
        {
            //ViewData['Contracts'] = lc;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ProgressSummary()
        {
            //disable change tracking
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            ProgressSummaryViewModel model = new ProgressSummaryViewModel();

            var contracts = await _context.Contracts
                //.Include(c=>c.Investments)
                //.ThenInclude(i=>i.Surveys)
                .Select(c => new Contract() {
                    ContractDate = c.ContractDate,
                    ContractId = c.ContractId,
                    FullfilmentDate = c.FullfilmentDate,
                    Number = c.Number,
                    ShortDescription = c.ShortDescription,
                    Status = c.Status,
                    Type = c.Type,
                    Project = new Models.DoEko.Project {ShortDescription = c.Project.ShortDescription }
                })
                .ToListAsync();

            foreach (var contract in contracts)
            {
                //load survey data for stats
                List<SurveyCentralHeating> lsch = _context.SurveysCH.Where(s => s.Investment.Contract.ContractId == contract.ContractId).Select(s=> new SurveyCentralHeating {SurveyId = s.SurveyId, Status = s.Status, RSEType = s.RSEType, InvestmentId = s.InvestmentId, Type = s.Type, Investment = new Investment { InvestmentId = s.Investment.InvestmentId, InspectorId = s.Investment.InspectorId } }).ToList();
                List<SurveyHotWater> lshw = _context.SurveysHW.Where(s => s.Investment.Contract.ContractId == contract.ContractId).Select(s=> new SurveyHotWater {SurveyId = s.SurveyId, Status = s.Status, RSEType = s.RSEType, InvestmentId = s.InvestmentId, Type = s.Type, Investment = new Investment { InvestmentId = s.Investment.InvestmentId, InspectorId = s.Investment.InspectorId } }).ToList();
                List<SurveyEnergy> lsen = _context.SurveysEN.Where(s => s.Investment.Contract.ContractId == contract.ContractId).Select(s=> new SurveyEnergy {SurveyId = s.SurveyId, Status = s.Status, RSEType = s.RSEType, InvestmentId = s.InvestmentId, Type = s.Type, Investment = new Investment { InvestmentId = s.Investment.InvestmentId, InspectorId = s.Investment.InspectorId } }).ToList();

                //new contract statistic
                ContractStatistic cs = new ContractStatistic();

                cs.Contract = contract;
                cs.InvestmentCount = _context.Investments.Count(i=>i.ContractId == contract.ContractId);

                //start survey statistics
                cs.Surveys.Add(this.SurveyStatsFor(lsch, SurveyType.CentralHeating, (int)SurveyRSETypeCentralHeating.HeatPump));
                cs.Surveys.Add(this.SurveyStatsFor(lsch, SurveyType.CentralHeating, (int)SurveyRSETypeCentralHeating.HeatPumpAir));
                cs.Surveys.Add(this.SurveyStatsFor(lsch, SurveyType.CentralHeating, (int)SurveyRSETypeCentralHeating.PelletBoiler));
                cs.Surveys.Add(this.SurveyStatsFor(lshw, SurveyType.HotWater, (int)SurveyRSETypeHotWater.Solar));
                cs.Surveys.Add(this.SurveyStatsFor(lshw, SurveyType.HotWater, (int)SurveyRSETypeHotWater.HeatPump));
                cs.Surveys.Add(this.SurveyStatsFor(lsen, SurveyType.Energy, (int)SurveyRSETypeEnergy.PhotoVoltaic));
                
                model.Contracts.Add(cs);
            }

            return View(model);
        }

        private SurveyStatistic SurveyStatsFor( IEnumerable<Survey> srvList, SurveyType type, int rseType)
        {
            SurveyStatistic ss = new SurveyStatistic();

            ss.Type = type;
            ss.RSEType = rseType;

            switch (type)
            {
                case SurveyType.CentralHeating:
                    IEnumerable<SurveyCentralHeating> lsch = srvList.Cast<SurveyCentralHeating>();

                    ss.Cancelled = lsch.Count(s => s.RSEType == (SurveyRSETypeCentralHeating)rseType && s.Status == SurveyStatus.Cancelled);
                    ss.Completed = lsch.Count(s => s.RSEType == (SurveyRSETypeCentralHeating)rseType && s.Status == SurveyStatus.Approved);
                    ss.Draft = lsch.Count(s => s.RSEType == (SurveyRSETypeCentralHeating)rseType && s.Status == SurveyStatus.Draft);
                    ss.InApproval = lsch.Count(s => s.RSEType == (SurveyRSETypeCentralHeating)rseType && s.Status == SurveyStatus.Approval);
                    ss.NotStarted = lsch.Count(s => s.RSEType == (SurveyRSETypeCentralHeating)rseType && s.Status == SurveyStatus.New);
                    ss.Rejected = lsch.Count(s => s.RSEType == (SurveyRSETypeCentralHeating)rseType && s.Status == SurveyStatus.Rejected);
                    ss.NotStartedNotassigned = lsch.Count(s => s.RSEType == (SurveyRSETypeCentralHeating)rseType && s.Status == SurveyStatus.New && ( s.Investment.InspectorId == Guid.Empty || s.Investment.InspectorId == null));
                    ss.Total = lsch.Count(s => s.RSEType == (SurveyRSETypeCentralHeating)rseType);

                    break;
                case SurveyType.HotWater:
                    IEnumerable<SurveyHotWater> lshw = srvList.Cast<SurveyHotWater>();
                    ss.Cancelled = lshw.Count(s => s.RSEType == (SurveyRSETypeHotWater)rseType && s.Status == SurveyStatus.Cancelled);
                    ss.Completed = lshw.Count(s => s.RSEType == (SurveyRSETypeHotWater)rseType && s.Status == SurveyStatus.Approved);
                    ss.Draft = lshw.Count(s => s.RSEType == (SurveyRSETypeHotWater)rseType && s.Status == SurveyStatus.Draft);
                    ss.InApproval = lshw.Count(s => s.RSEType == (SurveyRSETypeHotWater)rseType && s.Status == SurveyStatus.Approval);
                    ss.NotStarted = lshw.Count(s => s.RSEType == (SurveyRSETypeHotWater)rseType && s.Status == SurveyStatus.New);
                    ss.Rejected = lshw.Count(s => s.RSEType == (SurveyRSETypeHotWater)rseType && s.Status == SurveyStatus.Rejected);
                    ss.NotStartedNotassigned = lshw.Count(s => s.RSEType == (SurveyRSETypeHotWater)rseType && s.Status == SurveyStatus.New && (s.Investment.InspectorId == Guid.Empty || s.Investment.InspectorId == null));
                    ss.Total = lshw.Count(s => s.RSEType == (SurveyRSETypeHotWater)rseType);

                    break;
                case SurveyType.Energy:
                    IEnumerable<SurveyEnergy> lsen = srvList.Cast<SurveyEnergy>();
                    ss.Cancelled = lsen.Count(s => s.RSEType == (SurveyRSETypeEnergy)rseType && s.Status == SurveyStatus.Cancelled);
                    ss.Completed = lsen.Count(s => s.RSEType == (SurveyRSETypeEnergy)rseType && s.Status == SurveyStatus.Approved);
                    ss.Draft = lsen.Count(s => s.RSEType == (SurveyRSETypeEnergy)rseType && s.Status == SurveyStatus.Draft);
                    ss.InApproval = lsen.Count(s => s.RSEType == (SurveyRSETypeEnergy)rseType && s.Status == SurveyStatus.Approval);
                    ss.NotStarted = lsen.Count(s => s.RSEType == (SurveyRSETypeEnergy)rseType && s.Status == SurveyStatus.New);
                    ss.Rejected = lsen.Count(s => s.RSEType == (SurveyRSETypeEnergy)rseType && s.Status == SurveyStatus.Rejected);
                    ss.NotStartedNotassigned = lsen.Count(s => s.RSEType == (SurveyRSETypeEnergy)rseType && s.Status == SurveyStatus.New && (s.Investment.InspectorId == Guid.Empty || s.Investment.InspectorId == null));
                    ss.Total = lsen.Count(s => s.RSEType == (SurveyRSETypeEnergy)rseType);

                    break;
                default:
                    break;
            }

            return ss;
        }
    }
}