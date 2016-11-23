using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DoEko.Models.DoEko;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DoEko.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.DoEko.Survey;
using DoEko.ViewModels.SurveyViewModels;
using DoEko.Models.DoEko.Addresses;
using DoEko.Controllers.Helpers;
using DoEko.ViewComponents.ViewModels;
using Microsoft.AspNetCore.Http;
using DoEko.Services;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

namespace DoEko.Controllers
{

    [Authorize()]
    public class SurveysController : Controller
    {
        private readonly DoEkoContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IFileStorage _fileStorage;

        public SurveysController(DoEkoContext context, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> RoleManager,
            IFileStorage filestorage)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = RoleManager;
            _fileStorage = filestorage;
        }
        
        public async Task<IActionResult> Maintain(Guid Id)
        {
            Survey srv = await _context.Surveys.SingleAsync(s => s.SurveyId == Id);
            switch (srv.Type)
            {
                case SurveyType.CentralHeating:
                    SurveyCentralHeating srvch = await _context.SurveysCH
                        .Include(s => s.Investment)
                        .ThenInclude(i => i.Address)
                        .Include(s=>s.Investment)
                        .ThenInclude(i => i.InvestmentOwners)
                        .SingleAsync(s => s.SurveyId == Id);
                    return View("MaintainCH", srvch);
                case SurveyType.HotWater:
                    SurveyHotWater srvhw = await _context.SurveysHW
                        .Include(s => s.Investment)
                        .ThenInclude(i => i.Address)
                        .Include(s => s.Investment)
                        .ThenInclude(i => i.InvestmentOwners)
                        .SingleAsync(s => s.SurveyId == Id);
                    return View("MaintainHW", srvhw);
                case SurveyType.Energy:
                    SurveyEnergy srven = await _context.SurveysEN
                        .Include(s => s.Investment)
                        .ThenInclude(i => i.Address)
                        .Include(s => s.Investment)
                        .ThenInclude(i => i.InvestmentOwners)
                        .SingleAsync(s => s.SurveyId == Id);
                    return View("MaintainEN", srven);
                default:
                    return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> MaintainNextStepAjax(string currentStep, Guid surveyId)
        {
            Survey Srv;                //Survey being currently edited
            Enum RSEType;              //RSE Type used to calculate next step
            SurveyFormStep CurrentStep;//Current Step
            SurveyFormStep NextStep;   //Next Step

            int CurrentStepNo = 1;     //Needed only for owner or roofplane section where same component is displayed several times for each item
            int NextStepNo = 1;        //Neede only for owner or roofplane sections 
            //InvestmentOwner IO;        //Repeating section Owners
            //SurveyDetRoof RoofPlane;   //Repeating section Roof Planes
            
            //1. Get Survey Type / RSE Type
            try
            {
                Srv = await _context.Surveys
                      .Include(s => s.Investment)
                      .ThenInclude(i => i.InvestmentOwners)
                      .Include(s=>s.RoofPlanes)
                      .SingleAsync(s => s.SurveyId == surveyId);

                switch (Srv.Type)
                {
                    case SurveyType.CentralHeating:
                        RSEType = _context.SurveysCH.Single(s => s.SurveyId == surveyId).RSEType;
                        break;
                    case SurveyType.HotWater:
                        RSEType = _context.SurveysHW.Single(s => s.SurveyId == surveyId).RSEType;
                        break;
                    case SurveyType.Energy:
                        RSEType = _context.SurveysEN.Single(s => s.SurveyId == surveyId).RSEType;
                        break;
                    default:
                        return BadRequest();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }

            //2. Calculate current & next step

            //currentStep is the Id of the form tag composed of "viewComponentName" and "number of item".
            //Number of item is only used for owners and roof planes
            CurrentStep = (SurveyFormStep)Enum.Parse(typeof(SurveyFormStep), currentStep.Split('_').ElementAt(0));
            switch (CurrentStep)
            {
                case SurveyFormStep.InvestmentOwnerData:
                    CurrentStepNo = int.Parse(currentStep.Split('_').ElementAt(1));
                    if (CurrentStepNo < Srv.Investment.InvestmentOwners.Count)
                    {
                        NextStep = CurrentStep;
                        NextStepNo = CurrentStepNo + 1;
                    }
                    else
                    {
                        NextStep = SurveyFormHelper.Navigation[Srv.Type][RSEType].Find(CurrentStep).Next.Value;
                    }
                    break;
                case SurveyFormStep.SurveyGround:
                    NextStep = SurveyFormStep.SurveyAuditCH;
                    break;
                case SurveyFormStep.SurveyWall:
                    NextStep = SurveyFormStep.SurveyAuditCH;
                    break;
                case SurveyFormStep.SurveyRoofPlane:
                    CurrentStepNo = int.Parse(currentStep.Split('_').ElementAt(1));
                    if (CurrentStepNo < Srv.RoofPlanes.Count)
                    {
                        NextStep = CurrentStep;
                        NextStepNo = CurrentStepNo + 1;
                    }
                    else
                    {
                        NextStep = SurveyFormStep.SurveyAuditCH;
                    }
                    break;
                case SurveyFormStep.SurveySummary:
                    //No next step
                    return BadRequest();
                default:
                    NextStep = SurveyFormHelper.Navigation[Srv.Type][RSEType].Find(CurrentStep).Next.Value;
                    break;
            }

            //Special logic for Ground/Wall/Roof - next section depends on value of Localization
            if (NextStep == SurveyFormStep.SurveyGround)
            {
                switch (Srv.PlannedInstall.Localization)
                {
                    case InstallationLocalization.Roof:
                        NextStep = SurveyFormStep.SurveyRoofPlane;
                        break;
                    case InstallationLocalization.Ground:
                        NextStep = SurveyFormStep.SurveyGround;
                        break;
                    case InstallationLocalization.Wall:
                        NextStep = SurveyFormStep.SurveyWall;
                        break;
                    default:
                        NextStep = SurveyFormStep.SurveyGround;
                        break;
                }
            }

            //3. Set Attributes passed to viewcomponent controller
            object ViewComponentAttributes;

            switch (NextStep)
            {
                case SurveyFormStep.InvestmentGeneralInfo:
                    ViewComponentAttributes = new { investmentId = Srv.InvestmentId, mode = "Edit" };
                    break;
                case SurveyFormStep.InvestmentOwnerData:
                    ViewComponentAttributes = new { investmentId = Srv.InvestmentId, ownerId = Srv.Investment.InvestmentOwners.ElementAt(NextStepNo - 1).OwnerId };
                    break;
                case SurveyFormStep.SurveyRoofPlane:
                    ViewComponentAttributes = new { surveyId = surveyId, roofId = Srv.RoofPlanes.ElementAt(NextStepNo - 1).RoofPlaneId };
                    break;
                default:
                    ViewComponentAttributes = new { surveyId = surveyId };
                    break;
            }
            //4. Return viewcomponent

            return ViewComponent(NextStep.ToString(), ViewComponentAttributes);
        }

        [HttpGet]
        public async Task<IActionResult> MaintainPreviousStepAjax(string currentStep, Guid surveyId)
        {
            Survey Srv;                //Survey being currently edited
            Enum RSEType;              //RSE Type used to calculate next step
            SurveyFormStep CurrentStep;//Current Step
            SurveyFormStep PrevStep;   //Previous Step

            int CurrentStepNo = 1;     //Needed only for owner or roofplane section where same component is displayed several times for each item
            int PreviousStepNo = 1;        //Neede only for owner or roofplane sections 
                                       //InvestmentOwner IO;        //Repeating section Owners
                                       //SurveyDetRoof RoofPlane;   //Repeating section Roof Planes

            //1. Get Survey Type / RSE Type
            try
            {
                Srv = await _context.Surveys
                      .Include(s => s.Investment)
                      .ThenInclude(i => i.InvestmentOwners)
                      .Include(s => s.RoofPlanes)
                      .SingleAsync(s => s.SurveyId == surveyId);

                switch (Srv.Type)
                {
                    case SurveyType.CentralHeating:
                        RSEType = _context.SurveysCH.Single(s => s.SurveyId == surveyId).RSEType;
                        break;
                    case SurveyType.HotWater:
                        RSEType = _context.SurveysHW.Single(s => s.SurveyId == surveyId).RSEType;
                        break;
                    case SurveyType.Energy:
                        RSEType = _context.SurveysEN.Single(s => s.SurveyId == surveyId).RSEType;
                        break;
                    default:
                        return BadRequest();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }

            //2. Calculate current & next step

            //currentStep is the Id of the form tag composed of "viewComponentName" and "number of item".
            //Number of item is only used for owners and roof planes
            CurrentStep = (SurveyFormStep)Enum.Parse(typeof(SurveyFormStep), currentStep.Split('_').ElementAt(0));
            switch (CurrentStep)
            {
                case SurveyFormStep.InvestmentGeneralInfo:
                    return BadRequest();
                case SurveyFormStep.InvestmentOwnerData:
                    CurrentStepNo = int.Parse(currentStep.Split('_').ElementAt(1));
                    if (CurrentStepNo > 1)
                    {
                        PrevStep = CurrentStep;
                        PreviousStepNo = CurrentStepNo - 1;
                    }
                    else
                    {
                        PrevStep = SurveyFormHelper.Navigation[Srv.Type][RSEType].Find(CurrentStep).Previous.Value;
                    }
                    break;
                case SurveyFormStep.SurveyGround:
                    PrevStep = SurveyFormHelper.Navigation[Srv.Type][RSEType].Find(CurrentStep).Previous.Value;
                    break;
                case SurveyFormStep.SurveyWall:
                    PrevStep = SurveyFormHelper.Navigation[Srv.Type][RSEType].Find(SurveyFormStep.SurveyGround).Previous.Value;
                    break;
                case SurveyFormStep.SurveyRoofPlane:
                    CurrentStepNo = int.Parse(currentStep.Split('_').ElementAt(1));
                    if (CurrentStepNo > 1)
                    {
                        PrevStep = CurrentStep;
                        PreviousStepNo = CurrentStepNo - 1;
                    }
                    else
                    {
                        PrevStep = SurveyFormHelper.Navigation[Srv.Type][RSEType].Find(SurveyFormStep.SurveyGround).Previous.Value;
                    }
                    break;
                default:
                    PrevStep = SurveyFormHelper.Navigation[Srv.Type][RSEType].Find(CurrentStep).Previous.Value;
                    break;
            }

            //Special logic for Ground/Wall/Roof - next section depends on value of Localization
            if (PrevStep == SurveyFormStep.SurveyRoofPlane)
            {
                switch (Srv.PlannedInstall.Localization)
                {
                    case InstallationLocalization.Roof:
                        PrevStep = SurveyFormStep.SurveyRoofPlane;
                        break;
                    case InstallationLocalization.Ground:
                        PrevStep = SurveyFormStep.SurveyGround;
                        break;
                    case InstallationLocalization.Wall:
                        PrevStep = SurveyFormStep.SurveyWall;
                        break;
                    default:
                        PrevStep = SurveyFormStep.SurveyGround;
                        break;
                }
            }

            //Special logic for Roofplane and owners
            if (PrevStep == SurveyFormStep.SurveyRoofPlane && PrevStep != CurrentStep)
            {
                PreviousStepNo = Srv.RoofPlanes.Count;
            }
            else if (PrevStep == SurveyFormStep.InvestmentOwnerData && PrevStep != CurrentStep)
            {
                PreviousStepNo = Srv.Investment.InvestmentOwners.Count;
            }

            //3. Set Attributes passed to viewcomponent controller
            object ViewComponentAttributes;

            switch (PrevStep)
            {
                case SurveyFormStep.InvestmentGeneralInfo:
                    ViewComponentAttributes = new { investmentId = Srv.InvestmentId, mode = "Edit" };
                    break;
                case SurveyFormStep.InvestmentOwnerData:
                    ViewComponentAttributes = new { investmentId = Srv.InvestmentId, ownerId = Srv.Investment.InvestmentOwners.ElementAt(PreviousStepNo - 1).OwnerId };
                    break;
                case SurveyFormStep.SurveyRoofPlane:
                    ViewComponentAttributes = new { surveyId = surveyId, roofId = Srv.RoofPlanes.ElementAt(PreviousStepNo - 1).RoofPlaneId };
                    break;
                default:
                    ViewComponentAttributes = new { surveyId = surveyId };
                    break;
            }
            //4. Return viewcomponent

            return ViewComponent(PrevStep.ToString(), ViewComponentAttributes);
        }

        [HttpPost]
        public async Task<IActionResult> EditBuildingGeneralInfoAjax(Survey survey)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));
                survey.Building.SurveyId = survey.SurveyId;

                Survey srv = await _context.Surveys
                    .Include(s=>s.Investment)
                    .Include(s=>s.Building)
                    .SingleAsync(s => s.SurveyId == survey.SurveyId);

                if (srv.Building == null)
                {
                    _context.Add(survey.Building);
                }
                else
                {
                    srv.Building.InsulationThickness = survey.Building.InsulationThickness;
                    srv.Building.InsulationType = survey.Building.InsulationType;
                    srv.Building.TechnologyType = survey.Building.TechnologyType;
                    srv.Building.Volume = survey.Building.Volume;
                    srv.Building.WallMaterial = survey.Building.WallMaterial;
                    srv.Building.WallThickness = survey.Building.WallThickness;
                    _context.Update(srv.Building);
                }

                srv.Investment.BusinessActivity = survey.Investment.BusinessActivity;
                srv.Investment.CompletionYear = survey.Investment.CompletionYear;
                srv.Investment.HeatedArea = survey.Investment.HeatedArea;
                srv.Investment.InternetAvailable = survey.Investment.InternetAvailable;
                srv.Investment.NumberOfOccupants = survey.Investment.NumberOfOccupants;
                srv.Investment.TotalArea = survey.Investment.TotalArea;
                srv.Investment.Type = survey.Investment.Type;
                srv.Investment.UsableArea = survey.Investment.UsableArea;


                _context.Update(srv.Investment);

                int Result = await _context.SaveChangesAsync();



                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditPlannedInstallationAjax(SurveyDetPlannedInstall plannedInstall)
        {
            _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

            if (ModelState.IsValid)
            {
                Survey srv = await _context.Surveys.SingleAsync(s => s.SurveyId == plannedInstall.SurveyId);
                switch (srv.Type)
                {
                    case SurveyType.CentralHeating:
                        SurveyCentralHeating srvCH = _context.SurveysCH.Single(s => s.SurveyId == plannedInstall.SurveyId);
                        srvCH.PlannedInstall = plannedInstall;
                        _context.Update(plannedInstall);
                        break;
                    case SurveyType.HotWater:
                        SurveyHotWater srvHW = _context.SurveysHW.Single(s => s.SurveyId == plannedInstall.SurveyId);
                        srvHW.PlannedInstall = plannedInstall;
                        _context.Update(plannedInstall);
                        break;
                    case SurveyType.Energy:
                        SurveyEnergy srvEN = _context.SurveysEN.Single(s => s.SurveyId == plannedInstall.SurveyId);
                        //srvEN.PlannedInstall = plannedInstall;
                        _context.Update(plannedInstall);
                        break;
                    default:
                        break;
                }

                int result = await _context.SaveChangesAsync();

                return Ok();
            }
            else
            {
                return BadRequest();
            }

        }
        [HttpPost]
        public async Task<IActionResult> EditBoilerRoomAjax(SurveyDetBoilerRoom boilerRoom)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

                Survey srv = await _context.Surveys
                    .Include(s => s.BoilerRoom)
                    .SingleAsync(s => s.SurveyId == boilerRoom.SurveyId);

                if (srv.BoilerRoom == null)
                {
                    _context.Add(boilerRoom);
                }
                else
                {
                    srv.BoilerRoom.AirVentilationExists = boilerRoom.AirVentilationExists;
                    srv.BoilerRoom.DoorHeight = boilerRoom.DoorHeight;
                    srv.BoilerRoom.Height = boilerRoom.Height;
                    srv.BoilerRoom.HighVoltagePowerSupply = boilerRoom.HighVoltagePowerSupply;
                    srv.BoilerRoom.HWCirculationInstalled = boilerRoom.HWCirculationInstalled;
                    srv.BoilerRoom.HWInstalled = boilerRoom.HWInstalled;
                    srv.BoilerRoom.HWPressureReductorExists = boilerRoom.HWPressureReductorExists;
                    srv.BoilerRoom.IsDryAndWarm = boilerRoom.IsDryAndWarm;
                    srv.BoilerRoom.Length = boilerRoom.Length;
                    srv.BoilerRoom.RoomExists = boilerRoom.RoomExists;
                    srv.BoilerRoom.Volume = boilerRoom.Volume;
                    srv.BoilerRoom.Width = boilerRoom.Width;
                    _context.Update(srv.BoilerRoom);
                }

                int Result = await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }
        [HttpPost]
        public async Task<IActionResult> EditBathroomAjax(SurveyDetBathroom bathroom)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

                Survey srv = await _context.Surveys
                    .Include(s => s.BathRoom)
                    .SingleAsync(s => s.SurveyId == bathroom.SurveyId);

                if (srv.BathRoom == null)
                {
                    _context.Add(bathroom);
                }
                else
                {
                    srv.BathRoom.BathExsists = bathroom.BathExsists;
                    srv.BathRoom.BathVolume = bathroom.BathVolume;
                    srv.BathRoom.NumberOfBathrooms = bathroom.NumberOfBathrooms;
                    srv.BathRoom.ShowerExists = bathroom.ShowerExists;
                    _context.Update(srv.BathRoom);
                }

                int Result = await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }

        }
        [HttpPost]
        public async Task<IActionResult> EditAirConditionAjax(SurveyDetAirCond aircond)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

                Survey srv = await _context.Surveys
                    .Include(s => s.AirCondition)
                    .SingleAsync(s => s.SurveyId == aircond.SurveyId);

                if (srv.AirCondition == null)
                {
                    _context.Add(aircond);
                }
                else
                {
                    srv.AirCondition.Exists = aircond.Exists;
                    srv.AirCondition.isPlanned = aircond.isPlanned;
                    srv.AirCondition.MechVentilationExists = aircond.MechVentilationExists;
                    srv.AirCondition.Type = aircond.Type;

                    _context.Update(srv.AirCondition);
                }

                int Result = await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }

        }
        [HttpPost]
        public async Task<IActionResult> EditGroundAjax(SurveyDetGround ground)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

                Survey srv = await _context.Surveys
                    .Include(s => s.Ground)
                    .SingleAsync(s => s.SurveyId == ground.SurveyId);

                if (srv.Ground == null)
                {
                    _context.Add(ground);
                }
                else
                {
                    srv.Ground.Area = ground.Area;
                    srv.Ground.FormerMilitary = ground.FormerMilitary;
                    srv.Ground.OtherInstallation = ground.OtherInstallation;
                    srv.Ground.OtherInstallationType = ground.OtherInstallationType;
                    srv.Ground.Rocks = ground.Rocks;
                    srv.Ground.SlopeTerrain = ground.SlopeTerrain;
                    srv.Ground.WetLand = ground.WetLand;
                    _context.Update(srv.Ground);
                }
                int Result = await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }

        }
        [HttpPost]
        public async Task<IActionResult> EditWallAjax(SurveyDetWall wall)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

                Survey srv = await _context.Surveys
                    .Include(s => s.Investment)
                    .Include(s => s.Wall)
                    .SingleAsync(s => s.SurveyId == wall.SurveyId);

                if (srv.Wall == null)
                {
                    _context.Add(wall);
                }
                else
                {
                    srv.Wall.Azimuth = wall.Azimuth;
                    srv.Wall.Height = wall.Height;
                    srv.Wall.Width = wall.Width;
                    srv.Wall.UsableArea = srv.Wall.Width * srv.Wall.Height;
                    _context.Update(srv.Wall);
                }

                int Result = await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }
        [HttpPost]
        public async Task<IActionResult> EditRoofPlaneAjax(SurveyRoofPlaneViewModel model)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

                Survey srv = await _context.Surveys
                    .Include(s => s.RoofPlanes)
                    .SingleAsync(s => s.SurveyId == model.SurveyId);

                if (model.Plane.RoofPlaneId == null)
                {
                    model.Plane.RoofPlaneId = Guid.NewGuid();
                    model.Plane.SurveyId = model.SurveyId;
                    _context.Add(model.Plane);
                }
                else
                {
                    SurveyDetRoof plane = srv.RoofPlanes.Single(r => r.RoofPlaneId == model.Plane.RoofPlaneId);

                    plane.BuildingHeight = model.Plane.BuildingHeight;
                    plane.Chimney = model.Plane.Chimney;
                    plane.EdgeLength = model.Plane.EdgeLength;
                    plane.InstallationUnderPlane = model.Plane.InstallationUnderPlane;
                    plane.Length = model.Plane.Length;
                    plane.LightingProtection = model.Plane.LightingProtection;
                    plane.OkapHeight = model.Plane.OkapHeight;
                    plane.RidgeWeight = model.Plane.RidgeWeight;
                    plane.RoofLength = model.Plane.RoofLength;
                    plane.RoofMaterial = model.Plane.RoofMaterial;    
                    plane.SkyLights = model.Plane.SkyLights;
                    plane.SlopeAngle = model.Plane.SlopeAngle;
                    plane.SurfaceArea = model.Plane.SurfaceArea;
                    plane.SurfaceAzimuth = model.Plane.SurfaceAzimuth;
                    plane.Type = model.Plane.Type;
                    plane.Width = model.Plane.Width;
                    plane.Windows = model.Plane.Windows;

                    _context.Update(plane);
                }
                int Result = await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }

        }
        [HttpPost]
        public async Task<IActionResult> EditAuditCHAjax(SurveyAuditViewModel model)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

                Survey srv = await _context.Surveys
                    .Include(s => s.Investment)
                    .Include(s => s.Audit)
                    .SingleAsync(s => s.SurveyId == model.SurveyId);

                if (srv.Audit== null)
                {
                    model.Audit.SurveyId = model.SurveyId;
                    _context.Add(model.Audit);
                }
                else
                {
                    srv.Audit.SurveyId = model.SurveyId;
                    srv.Audit.CHRadiatorsInstalled = model.Audit.CHRadiatorsInstalled;
                    srv.Audit.CHRadiatorType = model.Audit.CHRadiatorType;
                    srv.Audit.CHFRadiantFloorInstalled = model.Audit.CHFRadiantFloorInstalled;
                    srv.Audit.CHRadiantFloorAreaPerc = model.Audit.CHRadiantFloorAreaPerc;
                    srv.Audit.AverageYearlyFuelConsumption = model.Audit.AverageYearlyFuelConsumption;
                    srv.Audit.AverageYearlyHeatingCosts = model.Audit.AverageYearlyHeatingCosts;
                    srv.Audit.AdditionalHeatSource = model.Audit.AdditionalHeatSource;
                    srv.Audit.AdditionalHeatParams = model.Audit.AdditionalHeatParams;
                    srv.Audit.CHIsHPOnlySource = model.Audit.CHIsHPOnlySource;
                    _context.Update(srv.Audit);
                }

                srv.Investment.CentralHeatingType = model.Investment.CentralHeatingType;
                srv.Investment.CentralHeatingFuel = model.Investment.CentralHeatingFuel;
                srv.Investment.CentralHeatingTypeOther = model.Investment.CentralHeatingTypeOther;
                _context.Update(srv.Investment);

                int Result = await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }

        }
        [HttpPost]
        public async Task<IActionResult> EditAuditENAjax(SurveyAuditViewModel model)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

                Survey srv = await _context.Surveys
                    .Include(s => s.Investment)
                    .Include(s => s.Audit)
                    .SingleAsync(s => s.SurveyId == model.SurveyId);

                if (srv.Audit == null)
                {
                    model.Audit.SurveyId = model.SurveyId;
                    _context.Add(model.Audit);
                }
                else
                {
                    srv.Audit.SurveyId = model.SurveyId;
                    srv.Audit.ElectricityPower = model.Audit.ElectricityPower;
                    srv.Audit.ENAdditionalConsMeter = model.Audit.ENAdditionalConsMeter;
                    srv.Audit.PowerCompanyName = model.Audit.PowerCompanyName;
                    srv.Audit.PowerSupplyType = model.Audit.PowerSupplyType;
                    srv.Audit.PowerConsMeterLocation = model.Audit.PowerConsMeterLocation;
                    srv.Audit.ElectricityAvgMonthlyCost = model.Audit.ElectricityAvgMonthlyCost;
                    srv.Audit.PowerAvgYearlyConsumption = model.Audit.PowerAvgYearlyConsumption;
                    srv.Audit.ENPowerLevel = model.Audit.ENPowerLevel;
                    srv.Audit.ENIsGround = model.Audit.ENIsGround;
                    srv.Audit.PhaseCount = model.Audit.PhaseCount;
                    srv.Audit.ComplexAgreement = model.Audit.ComplexAgreement;
                    _context.Update(srv.Audit);
                }

                int Result = await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }

        }
        [HttpPost]
        public async Task<IActionResult> EditAuditHWAjax(SurveyAuditViewModel model)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

                Survey srv = await _context.Surveys
                    .Include(s => s.Investment)
                    .Include(s => s.Audit)
                    .SingleAsync(s => s.SurveyId == model.SurveyId);

                if (srv.Audit == null)
                {
                    model.Audit.SurveyId = model.SurveyId;
                    _context.Add(model.Audit);
                }
                else
                {
                    srv.Audit.SurveyId = model.SurveyId;
                    srv.Audit.HWSourcePower = model.Audit.HWSourcePower;
                    srv.Audit.BoilerNominalPower = model.Audit.BoilerNominalPower;
                    srv.Audit.BoilerMaxTemp = model.Audit.BoilerMaxTemp;
                    srv.Audit.BoilerProductionYear = model.Audit.BoilerProductionYear;
                    srv.Audit.BoilerPlannedReplacement = model.Audit.BoilerPlannedReplacement;
                    srv.Audit.TankExists = model.Audit.TankExists;
                    srv.Audit.TankVolume = model.Audit.TankVolume;
                    srv.Audit.TankCoilSize = model.Audit.TankCoilSize;
                    _context.Update(srv.Audit);
                }
                srv.Investment.HotWaterType = model.Investment.HotWaterType;
                srv.Investment.HotWaterFuel = model.Investment.HotWaterFuel;
                _context.Update(srv.Investment);

                int Result = await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }

        }
        [HttpPost]
        public async Task<IActionResult> EditSummaryAjax(Survey survey)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

                switch (survey.Type)
                {
                    case SurveyType.CentralHeating:
                        var SrvCH = _context.SurveysCH.Single(s => s.SurveyId == survey.SurveyId);
                        SrvCH.FreeCommments = survey.FreeCommments;
                        if (SrvCH.FirstEditAt == null)
                        {
                            SrvCH.FirstEditAt = DateTime.Now;
                            SrvCH.FirstEditBy = _context.CurrentUserId;
                        }
                        _context.SurveysCH.Update(SrvCH);
                        break;
                    case SurveyType.HotWater:
                        var SrvHW = _context.SurveysHW.Single(s => s.SurveyId == survey.SurveyId);
                        SrvHW.FreeCommments = survey.FreeCommments;
                        if (SrvHW.FirstEditAt == null)
                        {
                            SrvHW.FirstEditAt = DateTime.Now;
                            SrvHW.FirstEditBy = _context.CurrentUserId;
                        }
                        _context.SurveysHW.Update(SrvHW);
                        break;
                    case SurveyType.Energy:
                        var SrvEN = _context.SurveysEN.Single(s => s.SurveyId == survey.SurveyId);
                        SrvEN.FreeCommments = survey.FreeCommments;
                        if (SrvEN.FirstEditAt == null)
                        {
                            SrvEN.FirstEditAt = DateTime.Now;
                            SrvEN.FirstEditBy = _context.CurrentUserId;
                        }
                        _context.SurveysEN.Update(SrvEN);
                        break;
                    default:
                        return BadRequest();
                }

                int Result = await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }

        [HttpPost]
        public IActionResult EditPhotoAjax(FormCollection Form, Guid SurveyId)
        {
            if (SurveyId == null)
            {
                return BadRequest();
            }
            CloudBlobContainer Container = _fileStorage.GetBlobContainer(enuAzureStorageContainerType.Survey);

            string Key = SurveyId.ToString();

            foreach (var file in Request.Form.Files)
            {
                Stream stream = file.OpenReadStream();
                if (file.Length == 0)
                    continue;

                if (file.Length > 0)
                {
                    string name = Key + '/' + file.FileName;
                    CloudBlockBlob blob = Container.GetBlockBlobReference(name);
                    blob.UploadFromStream(file.OpenReadStream());
                }
            }
            Guid invid = _context.Surveys.Single(s => s.SurveyId == SurveyId).InvestmentId;

            return RedirectToAction("Details", "Investments", new { Id = invid });
        }


        [HttpGet]
        public async Task<IActionResult> Details(Guid Id, string ReturnUrl = null)
        {

            SurveyHotWater SurveyHW = await _context.SurveysHW
                .Include(s => s.Investment).ThenInclude(i => i.Address)
                .Include(s => s.Investment).ThenInclude(i => i.InvestmentOwners).ThenInclude(io => io.Owner)
                .SingleOrDefaultAsync(s => s.SurveyId == Id);
            if (SurveyHW != null)
            {
                return View("DetailsHW", SurveyHW);
            }

            SurveyEnergy SurveyEN = await _context.SurveysEN
                .Include(s => s.Investment).ThenInclude(i => i.Address)
                .Include(s => s.Investment).ThenInclude(i => i.InvestmentOwners).ThenInclude(io => io.Owner)
                .SingleOrDefaultAsync(s => s.SurveyId == Id);
            if (SurveyEN != null)
            {
                return View("DetailsEN", SurveyEN);
            }

            SurveyCentralHeating SurveyCH = await _context.SurveysCH
                .Include(s => s.Investment).ThenInclude(i => i.Address)
                .Include(s => s.Investment).ThenInclude(i => i.InvestmentOwners).ThenInclude(io => io.Owner).ThenInclude(o => o.Address)
                .SingleOrDefaultAsync(s => s.SurveyId == Id);
            if (SurveyCH != null)
            {
                DetailsCHViewModel model = new DetailsCHViewModel(SurveyCH);

                //ViewData["InvAddrCountryId"] = AddressesController.GetCountries(_context, InvestmentVM.Address.CountryId);
                ViewData["InvAddrStateId"] = AddressesController.GetStates(_context, model.InvestmentAddress.StateId);
                ViewData["InvAddrDistrictId"] = AddressesController.GetDistricts(_context, model.InvestmentAddress.StateId, model.InvestmentAddress.DistrictId);
                ViewData["InvAddrCommuneId"] = AddressesController.GetCommunes(_context, model.InvestmentAddress.StateId, model.InvestmentAddress.DistrictId, model.InvestmentAddress.CommuneId, model.InvestmentAddress.CommuneType);

                for (int i = 0; i < model.Owners.Count(); i++)
                {
                    Address _adr = model.Owners.ElementAt(i).Address;
                    ViewData["OwnerStateId_" + i.ToString()] = AddressesController.GetStates(_context, _adr.StateId);
                    ViewData["OwnerDistrictId_" + i.ToString()] = AddressesController.GetDistricts(_context, _adr.StateId, _adr.DistrictId);
                    ViewData["OwnerCommuneId_" + i.ToString()] = AddressesController.GetCommunes(_context, _adr.StateId, _adr.DistrictId, _adr.CommuneId, _adr.CommuneType);

                }

                return View("DetailsCH", model);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsCH(DetailsCHViewModel model, string ReturnUrl = null)
        {
            //Needed to automatically set ChangedBy / CreatedBy
            _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));
            //Adres gmina i typ sklejone w jednym polu        
            model.InvestmentAddress.CommuneType = (CommuneType)Enum.ToObject(typeof(CommuneType), model.InvestmentAddress.CommuneId % 10);
            model.InvestmentAddress.CommuneId /= 10;
            for (int i = 0; i < model.Owners.Count; i++)
            {
                model.Owners[i].Address.CommuneType = (CommuneType)Enum.ToObject(typeof(CommuneType), model.Owners[i].Address.CommuneId % 10);
                model.Owners[i].Address.CommuneId /= 10;
            }

            SurveyCentralHeating survey = _context.SurveysCH.SingleOrDefault(s => s.SurveyId == model.Survey.SurveyId);
            
            switch (survey.Status)
            {
                case SurveyStatus.New:
                    survey.Status = SurveyStatus.Draft;
                    break;
                case SurveyStatus.Draft:
                    break;
                case SurveyStatus.Approval:
                    break;
                case SurveyStatus.Rejected:
                    survey.Status = SurveyStatus.Draft;
                    break;
                case SurveyStatus.Approved:
                    break;
                case SurveyStatus.Cancelled:
                    break;
                default:
                    break;
            }

            //_context.SurveysCH.Update(model);
            await _context.SaveChangesAsync();
             return RedirectToAction("Details", "Investments", new { Id = model.Survey.InvestmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsEN(SurveyEnergy model, string ReturnUrl = null)
        {
            _context.SurveysEN.Update(model);
            await _context.SaveChangesAsync();
             return RedirectToAction("Details", "Investments", new { Id = model.InvestmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsHW(SurveyHotWater model, string ReturnUrl = null)
        {
            _context.SurveysHW.Update(model);
            await _context.SaveChangesAsync();
            //if (User.IsInRole("Inspector"))
            //{
            //    return RedirectToAction("List");
            //}
             return RedirectToAction("Details", "Investments", new { Id = model.InvestmentId });
        }

    }
}