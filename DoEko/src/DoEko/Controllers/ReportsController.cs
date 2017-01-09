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
using Microsoft.AspNetCore.Mvc.Rendering;

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
            ViewData["Projects"] = new SelectList(await _context.Projects.Select(p => new SelectListItem() {
                Value = p.ProjectId.ToString(),
                Text = p.ShortDescription + " (" +
                       p.StartDate.ToShortDateString() + " - " +
                       p.EndDate.ToShortDateString() + ")"
            }).ToListAsync(),"Value","Item",null);

            ViewData["Contracts"] = new SelectList(await _context.Contracts.Select(c => new SelectListItem() {
                 Value = c.ContractId.ToString(),
                 Text = c.FullfilmentDate.HasValue ?  
                        c.Number + 
                        c.ContractDate.ToShortDateString() + 
                        c.FullfilmentDate.Value.ToShortDateString() +
                        c.ShortDescription :

                        c.Number +
                        c.ContractDate.ToShortDateString() +
                        c.FullfilmentDate.Value.ToShortDateString() +
                        c.ShortDescription
            }).ToListAsync(), "Value","Text",null);

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SurveyToCSV(int? projectId, int? contractId, Guid? investmentId)
        {
            var sl = _context.Surveys.Where(s => s.Status != SurveyStatus.Cancelled);

            if (projectId.HasValue)
                sl = sl.Where(s => s.Investment.Contract.ProjectId == projectId);
            if (contractId.HasValue)
                sl = sl.Where(s => s.Investment.ContractId == contractId);
            if (investmentId.HasValue)
                sl = sl.Where(s => s.InvestmentId == investmentId);

            sl = sl.OrderBy(s => s.Investment.Contract.ProjectId)
                .ThenBy(s => s.Investment.ContractId)
                .ThenBy(s => s.InvestmentId);

            sl.Include(s => s.Investment).ThenInclude(i => i.InvestmentOwners).ThenInclude(io => io.Owner).ThenInclude(o => o.Address)
                .Include(s => s.Investment).ThenInclude(i => i.Address);

            var result = await sl.ToListAsync();

            CsvExport list = await SurveyListAsCSV(result);
            return File(list.ExportToBytes(), "text/csv", "DaneZAnkiet.csv");

        }

        private async Task<CsvExport> SurveyListAsCSV(List<Survey> data)
        {
            CsvExport myExport = new CsvExport(columnSeparator: ";");
            

            //foreach (var srv in data)
            //{
            //    //HEADER
            //    myExport.Addrow();

            //    //DATA

            //    //SURVEY GENERAL
            //    myExport["TYP OZE"] = srv.TypeFullDescription();
            //    myExport["STATUS ANKIETY"] = srv.Status.DisplayName();

            //    //INWESTYCJA
            //    myExport["INWESTYCJA - ADRES"] = srv.Investment.Address.SingleLine();


            //    //WŁAŚCICIELE
            //    for (int i = 0; i < 3; i++)
            //    {
            //        if (srv.Investment.InvestmentOwners.Count == (i + 1))
            //        {

            //            myExport["WŁAŚCICIEL " + i.ToString()] = srv.Investment.InvestmentOwners.ElementAt(i).Owner.PartnerName2 + " " + srv.Investment.InvestmentOwners.ElementAt(i).Owner.PartnerName1;
            //            myExport["WŁAŚCICIEL ADRES " + i.ToString()] = srv.Investment.InvestmentOwners.ElementAt(i).Owner.Address.SingleLine;
            //        }
            //        else
            //        {
            //            myExport["WŁAŚCICIEL " + i.ToString()] = "";
            //            myExport["WŁAŚCICIEL ADRES " + i.ToString()] = "";
            //        }
            //    }

            //    //INWESTYCJA OGOLNE

            //    myExport["RODZ. DZIAŁALN."] = srv.Investment.BusinessActivity.DisplayName();
            //    myExport["PALIWO GŁ.CO"] = srv.Investment.CentralHeatingFuel.DisplayName();
            //    myExport["RODZAJ GŁ.CO"] = srv.Investment.CentralHeatingType == CentralHeatingType.Other ?
            //                   srv.Investment.CentralHeatingType.DisplayName() : 
            //                   srv.Investment.CentralHeatingTypeOther;
            //    myExport["ROK BUDOWY"] = srv.Investment.CompletionYear.ToString();
            //    myExport["POW. OGRZEWANA"] = srv.Investment.HeatedArea.ToString();
            //    myExport["PALIWO GŁ. CW"] = srv.Investment.HotWaterFuel.DisplayName();
            //    myExport["RODZAJ GŁ. CW"] = srv.Investment.HotWaterType.DisplayName();
            //    myExport["INTERNET W M.INW."] = srv.Investment.InternetAvailable.ToString();
            //    myExport["NR KS. WIECZ."] = srv.Investment.LandRegisterNo;
            //    myExport["L.MIESZKAŃCÓW"] = srv.Investment.NumberOfOccupants.ToString();
            //    //myExport[""] = srv.Investment.PlotAreaNumber;
            //    myExport["NR DZIAŁKI"] = srv.Investment.PlotNumber;
            //    myExport["STAN BUD."] = srv.Investment.Stage.DisplayName();
            //    myExport["POW. CAŁK."] = srv.Investment.TotalArea.ToString();
            //    myExport["RODZ.BUD."] = srv.Investment.Type.DisplayName();
            //    myExport["POW. UŻYTK."] = srv.Investment.UsableArea.ToString();

            //    //AIRCOND
            //    myExport["ISTN. KLIMAT."] = srv.AirCondition.Exists.ToString();
            //    myExport["KLIMAT.PLANOWANA"] = srv.AirCondition.isPlanned.ToString();
            //    myExport["MECH.WENT.ISTN."] = srv.AirCondition.MechVentilationExists.ToString();
            //    myExport["RODZAJ WENT."] = srv.AirCondition.Type.DisplayName();
            //    //ENERGY AUDIT
            //    myExport["PAR.DOD.ŹR.CIEPŁA"] = srv.Audit.AdditionalHeatParams;
            //    myExport["DOD.ŹR.CIEPŁA"] = srv.Audit.AdditionalHeatSource.ToString();
            //    myExport["ŚR.ROCZNE ZUŻYCIE CO"] = srv.Audit.AverageYearlyFuelConsumption.ToString();
            //    myExport["ŚR.ROCZNE KOSZTY CO"] = srv.Audit.AverageYearlyHeatingCosts.ToString();
            //    myExport["MAX TEMP. PIECA"] = srv.Audit.BoilerMaxTemp.ToString();
            //    myExport["PIEC - MOC"] = srv.Audit.BoilerNominalPower.ToString();
            //    myExport["PIEC - PLAN. WYM."] = srv.Audit.BoilerPlannedReplacement.ToString();
            //    myExport["PIEC - ROK PROD."] = srv.Audit.BoilerProductionYear.ToString();
            //    myExport["CO PODLOG."] = srv.Audit.CHFRadiantFloorInstalled.ToString();
            //    myExport["PLAN.POMPA BĘDZIE JEDYNYM ŹR."] = srv.Audit.CHIsHPOnlySource.ToString();
            //    myExport["CO PODLOG. - PROCENT POW."] = srv.Audit.CHRadiantFloorAreaPerc.ToString();
            //    myExport["CO GRZEJNIKI"] = srv.Audit.CHRadiatorsInstalled.ToString();
            //    myExport["CO GRZEJNIKI - TYP"] = srv.Audit.CHRadiatorType.DisplayName();
            //    myExport["UMOWA KOMPLEKS."] = srv.Audit.ComplexAgreement.ToString();
            //    myExport["EE - ŚR. KOSZT/MC."] = srv.Audit.ElectricityAvgMonthlyCost.ToString();
            //    myExport["EE - MOC PRZYŁ."] = srv.Audit.ElectricityPower.ToString();
            //    myExport["EE - DOD. LICZNIK"] = srv.Audit.ENAdditionalConsMeter.ToString();
            //    myExport["EE - UZIEMIENIE"] = srv.Audit.ENIsGround.ToString();
            //    myExport["EE - PLANOWANA MOC"] = srv.Audit.ENPowerLevel.ToString();
            //    myExport["CW - MOC"] = srv.Audit.HWSourcePower.ToString();
            //    myExport["EE - L. FAZ"] = srv.Audit.PhaseCount.DisplayName();
            //    myExport["EE - ROCZNE ZUŻYCIE"] = srv.Audit.PowerAvgYearlyConsumption.ToString();
            //    myExport["EE - DYSTRYBUTOR"] = srv.Audit.PowerCompanyName.DisplayName();
            //    myExport["EE - UMIEJSC. LICZNIKA"] = srv.Audit.PowerConsMeterLocation.DisplayName();
            //    myExport["EE - RODZ. PRZYŁ."] = srv.Audit.PowerSupplyType.DisplayName();
            //    myExport["CW - POW. WĘŻ."] = srv.Audit.TankCoilSize.ToString();
            //    myExport["CW - ISTN. ZASOBNIK"] = srv.Audit.TankExists.ToString();
            //    myExport["CW - OBJ. ZASOBNIKA"] = srv.Audit.TankVolume.ToString();

            //    //BATHROOM
            //    myExport["ISTN. ŁAŹ."] = srv.BathRoom.BathExsists.ToString();
            //    myExport["OBJ. WANNY"] = srv.BathRoom.BathVolume.ToString();
            //    myExport["L.ŁAZIENEK"] = srv.BathRoom.NumberOfBathrooms.ToString();
            //    myExport["ISTN.PRYSZNIC"] = srv.BathRoom.ShowerExists.ToString();
            //    //BOILERROOM
            //    myExport[""] = srv.BoilerRoom.AirVentilationExists
            //    myExport[""] = srv.BoilerRoom.DoorHeight
            //    myExport[""] = srv.BoilerRoom.Height
            //    myExport[""] = srv.BoilerRoom.HighVoltagePowerSupply
            //    myExport[""] = srv.BoilerRoom.HWCirculationInstalled
            //    myExport[""] = srv.BoilerRoom.HWInstalled
            //    myExport[""] = srv.BoilerRoom.HWPressureReductorExists
            //    myExport[""] = srv.BoilerRoom.IsDryAndWarm
            //    myExport[""] = srv.BoilerRoom.Length
            //    myExport[""] = srv.BoilerRoom.RoomExists
            //    myExport[""] = srv.BoilerRoom.ThreePowerSuppliesExists
            //    myExport[""] = srv.BoilerRoom.Volume
            //    myExport[""] = srv.BoilerRoom.Width
            //    //BUILDING
            //    myExport[""] = srv.Building
            //    myExport[""] = srv.Building
            //    myExport[""] = srv.Building
            //    myExport[""] = srv.Building
            //    myExport[""] = srv.Building
            //    myExport[""] = srv.Building
            //    //
            //    myExport[""] = srv.CancelComments
            //    myExport[""] = srv.CancelType
            //    myExport[""] = srv.ChangedAt
            //    myExport[""] = srv.ChangedBy
            //    myExport[""] = srv.FreeCommments
            //    myExport[""] = srv.Ground
            //    myExport[""] = srv.IsPaid
            //    myExport[""] = srv.PlannedInstall
            //    myExport[""] = srv.RejectComments
            //    srv.RoofPlanes
            //    //srv.Status
            //    myExport[""] = srv.Wall

            //}

            return myExport;
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