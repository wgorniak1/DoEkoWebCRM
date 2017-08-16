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

                    if (srvch.InspectionDateTime <= DateTime.MinValue)
                    {
                        srvch.InspectionDateTime = null;
                    }

                    return View("MaintainCH", srvch);
                case SurveyType.HotWater:
                    SurveyHotWater srvhw = await _context.SurveysHW
                        .Include(s => s.Investment)
                        .ThenInclude(i => i.Address)
                        .Include(s => s.Investment)
                        .ThenInclude(i => i.InvestmentOwners)
                        .SingleAsync(s => s.SurveyId == Id);
                    if (srvhw.InspectionDateTime <= DateTime.MinValue)
                    {
                        srvhw.InspectionDateTime = null;
                    }
                    return View("MaintainHW", srvhw);
                case SurveyType.Energy:
                    SurveyEnergy srven = await _context.SurveysEN
                        .Include(s => s.Investment)
                        .ThenInclude(i => i.Address)
                        .Include(s => s.Investment)
                        .ThenInclude(i => i.InvestmentOwners)
                        .SingleAsync(s => s.SurveyId == Id);
                    if (srven.InspectionDateTime <= DateTime.MinValue)
                    {
                        srven.InspectionDateTime = null;
                    }
                    return View("MaintainEN", srven);
                default:
                    return NotFound();
            }
        }


        [HttpGet]
        public IActionResult MaintainStepAjax(int stepNo, Guid surveyId)
        {
            SurveyFormStep NextStep;
            Survey Srv;
            Enum RSEType;

            try
            {
                Srv = _context.Surveys
                      .Include(s => s.Investment)
                      .ThenInclude(i => i.InvestmentOwners)
                      .Include(s => s.RoofPlanes)
                      .Include(s => s.PlannedInstall)
                      .Single(s => s.SurveyId == surveyId);

                switch (Srv.Type)
                {
                    case SurveyType.CentralHeating:
                        RSEType = ((SurveyCentralHeating)Srv).RSEType;
                        break;
                    case SurveyType.HotWater:
                        RSEType = ((SurveyHotWater)Srv).RSEType;
                        break;
                    case SurveyType.Energy:
                        RSEType = ((SurveyEnergy)Srv).RSEType;
                        break;
                    default:
                        return BadRequest();
                }

                ////
                NextStep = SurveyFormHelper.Steps[Srv.Type][RSEType].ElementAt(stepNo - 1);
                ////

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
                        ViewComponentAttributes = new { investmentId = Srv.InvestmentId, ownerId = Srv.Investment.InvestmentOwners.ElementAt(1).OwnerId };
                        break;
                    case SurveyFormStep.SurveyRoofPlane:
                        //take first roofplane or pass null
                        var roofPlaneId = (Srv.RoofPlanes != null && Srv.RoofPlanes.Count > 0) ? Srv.RoofPlanes.First().RoofPlaneId : Guid.Empty;
                        ViewComponentAttributes = new { surveyId = surveyId, roofPlaneId = roofPlaneId };
                        break;
                    default:
                        ViewComponentAttributes = new { surveyId = surveyId };
                        break;
                }

                return ViewComponent(NextStep.ToString(), ViewComponentAttributes);

            }
            catch(Exception exc)
            {
                if (exc.InnerException != null)
                    return BadRequest(exc.InnerException.Message);
                else
                    return BadRequest(exc.Message);
            }
        }
        [HttpGet]
        public IActionResult MaintainNextStepAjax(string currentStep, Guid surveyId)
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
                Srv = _context.Surveys
                      .Include(s => s.Investment)
                      .ThenInclude(i => i.InvestmentOwners)
                      .Include(s=>s.RoofPlanes)
                      .Include(s=>s.PlannedInstall)
                      .Single(s => s.SurveyId == surveyId);

                switch (Srv.Type)
                {
                    case SurveyType.CentralHeating:
                        RSEType = (SurveyRSETypeCentralHeating)Srv.GetRSEType();
                        break;
                    case SurveyType.HotWater:
                        RSEType = (SurveyRSETypeHotWater)Srv.GetRSEType();
                        break;
                    case SurveyType.Energy:
                        RSEType = (SurveyRSETypeEnergy)Srv.GetRSEType();
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
                case SurveyFormStep.SurveyPhoto:
                    //No next step
                    return BadRequest();
                default:
                    NextStep = SurveyFormHelper.Navigation[Srv.Type][RSEType].Find(CurrentStep).Next.Value;
                    break;
            }

            //Special logic for Ground/Wall/Roof - next section depends on value of Localization
            if (NextStep == SurveyFormStep.SurveyGround && 
                !RSEType.Equals(SurveyRSETypeCentralHeating.HeatPump)) 
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
                    Guid roofId;
                    if (Srv.RoofPlanes.Count != 0)
                    {
                        roofId = Srv.RoofPlanes.ElementAt(NextStepNo - 1).RoofPlaneId;
                    }
                    else
                        roofId = Guid.Empty;

                    ViewComponentAttributes = new { surveyId = surveyId, roofPlaneId = roofId };
                    break;
                default:
                    ViewComponentAttributes = new { surveyId = surveyId };
                    break;
            }
            //4. Return viewcomponent

            return ViewComponent(NextStep.ToString(), ViewComponentAttributes);
        }
        [HttpGet]
        public IActionResult MaintainPreviousStepAjax(string currentStep, Guid surveyId)
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
                Srv = _context.Surveys
                      .Include(s => s.Investment)
                      .ThenInclude(i => i.InvestmentOwners)
                      .Include(s => s.RoofPlanes)
                      .Include(s => s.PlannedInstall)
                      .Single(s => s.SurveyId == surveyId);

                switch (Srv.Type)
                {
                    case SurveyType.CentralHeating:
                        RSEType = (SurveyRSETypeCentralHeating)Srv.GetRSEType();
                        break;
                    case SurveyType.HotWater:
                        RSEType = (SurveyRSETypeHotWater)Srv.GetRSEType();
                        break;
                    case SurveyType.Energy:
                        RSEType = (SurveyRSETypeEnergy)Srv.GetRSEType();
                        break;
                    default:
                        return BadRequest();
                }
            }
            catch (Exception )
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
                    Guid roofId;
                    if (Srv.RoofPlanes.Count != 0)
                    {
                        roofId = Srv.RoofPlanes.ElementAt(PreviousStepNo - 1).RoofPlaneId;
                    }
                    else
                        roofId = Guid.Empty;

                    ViewComponentAttributes = new { surveyId = surveyId, roofPlaneId = roofId };
                    break;
                default:
                    ViewComponentAttributes = new { surveyId = surveyId };
                    break;
            }
            //4. Return viewcomponent

            return ViewComponent(PrevStep.ToString(), ViewComponentAttributes);
        }
        [HttpPost]
        public IActionResult EditBuildingGeneralInfoAjax(Survey survey)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));
                survey.Building.SurveyId = survey.SurveyId;

                Survey srv = _context.Surveys
                    .Include(s=>s.Investment)
                    .Include(s=>s.Building)
                    .Single(s => s.SurveyId == survey.SurveyId);

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
                srv.Investment.Stage = survey.Investment.Stage;


                _context.Update(srv.Investment);

                int Result = _context.SaveChanges();
                //update status
                if (User.IsInRole(Roles.Inspector))
                {
                    Result = this.SetStatusOnEdit(survey.SurveyId);
                }
                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }
        [HttpPost]
        public IActionResult EditPlannedInstallationAjax(SurveyDetPlannedInstall plannedInstall)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

                Survey srv = _context.Surveys
                    .Include(s => s.PlannedInstall)
                    .Single(s => s.SurveyId == plannedInstall.SurveyId);

                if (srv.PlannedInstall == null)
                {
                    _context.Add(plannedInstall);
                }
                else
                {
                    srv.PlannedInstall.Configuration = plannedInstall.Configuration;
                    srv.PlannedInstall.Localization = plannedInstall.Localization;
                    srv.PlannedInstall.OnWallPlacementAvailable = plannedInstall.OnWallPlacementAvailable;
                    srv.PlannedInstall.Purpose = plannedInstall.Purpose;
                    _context.Update(srv.PlannedInstall);
                }

                int Result = _context.SaveChanges();
                //update status
                if (User.IsInRole(Roles.Inspector))
                {
                    Result = this.SetStatusOnEdit(plannedInstall.SurveyId);
                }
                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }
        [HttpPost]
        public IActionResult EditBoilerRoomAjax(SurveyDetBoilerRoom boilerRoom)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

                Survey srv = _context.Surveys
                    .Include(s => s.BoilerRoom)
                    .Single(s => s.SurveyId == boilerRoom.SurveyId);

                if (srv.BoilerRoom == null)
                {
                    boilerRoom.Volume = boilerRoom.Width * boilerRoom.Height * boilerRoom.Length;
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
                    srv.BoilerRoom.Width = boilerRoom.Width;
                    srv.BoilerRoom.Volume = boilerRoom.Width * boilerRoom.Height * boilerRoom.Length;
                    _context.Update(srv.BoilerRoom);
                }

                int Result = _context.SaveChanges();
                //update status
                if (User.IsInRole(Roles.Inspector))
                {
                    Result = this.SetStatusOnEdit(boilerRoom.SurveyId);
                }
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
                //update status
                if (User.IsInRole(Roles.Inspector))
                {
                    int result = this.SetStatusOnEdit(bathroom.SurveyId);
                }
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
                //update status
                if (User.IsInRole(Roles.Inspector))
                {
                    int result = this.SetStatusOnEdit(aircond.SurveyId);
                }
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

                //update status
                if (User.IsInRole(Roles.Inspector))
                {
                    Result = this.SetStatusOnEdit(ground.SurveyId);
                }

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
                    .Include(s => s.Wall)
                    .SingleAsync(s => s.SurveyId == wall.SurveyId);

                if (srv.Wall == null)
                {
                    wall.UsableArea = wall.Width * wall.Height;
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

                //update status
                if (User.IsInRole(Roles.Inspector))
                {
                    Result = this.SetStatusOnEdit(wall.SurveyId);
                }

                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }
        [HttpGet]
        public IActionResult CreateRoofPLaneAjax(Guid surveyId)
        {
            return ViewComponent("SurveyRoofPlane", new { surveyId = surveyId, viewMode = SurveyViewMode.Maintain });
        }
        [HttpPost]
        public async Task<IActionResult> CreateRoofPlaneAjax(SurveyRoofPlaneViewModel model)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

                Survey srv = await _context.Surveys
                    .Include(s => s.RoofPlanes)
                    .SingleAsync(s => s.SurveyId == model.SurveyId);

                if (model.Plane.RoofPlaneId == Guid.Empty)
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

                //update status
                if (User.IsInRole(Roles.Inspector))
                {
                    int result = this.SetStatusOnEdit(model.SurveyId);
                }

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

                if (model.Plane.RoofPlaneId == Guid.Empty)
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

                //update status
                if (User.IsInRole(Roles.Inspector))
                {
                    Result = this.SetStatusOnEdit(model.SurveyId);
                }

                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }

        }

        [HttpPost]
        public async Task<IActionResult> DeleteRoofPlaneAjax(Guid surveyId, Guid roofPlaneId)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

                var roofPlane = _context.Surveys
                    .Where(s => s.SurveyId == surveyId)
                    .Select(s => s.RoofPlanes.Where(rp => rp.RoofPlaneId == roofPlaneId).Single())
                    .Single();

                _context.Remove(roofPlane);
                int Result = await _context.SaveChangesAsync();

                //update status
                if (User.IsInRole(Roles.Inspector))
                {
                    Result = this.SetStatusOnEdit(surveyId);
                }

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

                //update status
                if (User.IsInRole(Roles.Inspector))
                {
                    Result = this.SetStatusOnEdit(model.SurveyId);
                }

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


                //update status
                if (User.IsInRole(Roles.Inspector))
                {
                    Result = this.SetStatusOnEdit(model.SurveyId);
                }


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

                //update status
                if (User.IsInRole(Roles.Inspector))
                {
                    Result = this.SetStatusOnEdit(model.SurveyId);
                }

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

                //update status
                if (User.IsInRole(Roles.Inspector))
                {
                    int result = this.SetStatusOnEdit(survey.SurveyId);
                }

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
        public IActionResult EditPhotoAjax(Guid SurveyId, Guid InvestmentId)//FormCollection Form, Guid SurveyId)
        {
            try {
                //update status
                if (User.IsInRole(Roles.Inspector))
                {
                    int result = this.SetStatusOnEdit(SurveyId);
                }

                return RedirectToAction("Details", "Investments", new { Id = InvestmentId });
            }
            catch (Exception exc)
            {
                if (exc.InnerException != null)
                    return BadRequest(exc.InnerException.Message);
                else
                    return BadRequest(exc.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInspectionDateAjax(Guid surveyId, DateTime inspectionDateTime)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

                SurveyType SurveyType = _context.Surveys.Where(s=> s.SurveyId == surveyId).Select(s => s.Type).Single();

                switch (SurveyType)
                {
                    case SurveyType.CentralHeating:
                        var SrvCH = _context.SurveysCH.Single(s => s.SurveyId == surveyId);
                        SrvCH.InspectionDateTime = inspectionDateTime;
                        _context.SurveysCH.Update(SrvCH);
                        break;
                    case SurveyType.HotWater:
                        var SrvHW = _context.SurveysHW.Single(s => s.SurveyId == surveyId);
                        SrvHW.InspectionDateTime = inspectionDateTime;
                        _context.SurveysHW.Update(SrvHW);
                        break;
                    case SurveyType.Energy:
                        var SrvEN = _context.SurveysEN.Single(s => s.SurveyId == surveyId);
                        SrvEN.InspectionDateTime = inspectionDateTime;
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
        public async Task<IActionResult> CancelAjax(SurveyCancelViewModel model)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

                Survey srv = await _context.Surveys
                    .Include(s=>s.Investment)
                    .SingleAsync(s => s.SurveyId == model.SurveyId);

                srv.CancelType = model.CancelType;
                srv.CancelComments = model.CancelComments;
                srv.Status = SurveyStatus.Cancelled;
                _context.Update(srv);

                //
                if (model.ReplaceWithType != null)
                {
                    switch (model.ReplaceWithType)
                    {
                        case SurveyType.CentralHeating:
                            SurveyCentralHeating srvCH = new SurveyCentralHeating() {
                                InvestmentId = srv.InvestmentId,
                                Status = SurveyStatus.New,
                                RSEType = model.ReplaceWithRSECH.Value,
                                Type = model.ReplaceWithType.Value
                            };

                            _context.Add(srvCH);
                            break;
                        case SurveyType.Energy:
                            SurveyEnergy srvEN = new SurveyEnergy() {
                                InvestmentId = srv.InvestmentId,
                                Status = SurveyStatus.New,
                                RSEType = model.ReplaceWithRSEEN.Value,
                                Type = model.ReplaceWithType.Value
                            };
                            _context.Add(srvEN);
                            break;
                        case SurveyType.HotWater:
                            SurveyHotWater srvHW = new SurveyHotWater() {
                                InvestmentId = srv.InvestmentId,
                                Status = SurveyStatus.New,
                                RSEType = model.ReplaceWithRSEHW.Value,
                                Type = model.ReplaceWithType.Value
                            };
                            _context.Add(srvHW);
                            break;
                        default:
                            break;
                    }
                }


                int Result = await _context.SaveChangesAsync();

                //UPDATE INVESTMENT STATUS
                await this.updateInvestmentStatus(srv.InvestmentId);

                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }
        [HttpPost]
        public async Task<IActionResult> SubmitAjax(Guid surveyId, bool submitInvestment = false)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

                int Result = this.SetApprovalStatus(surveyId, true);

                if (Result == 0)
                {
                    ModelState.AddModelError(nameof(Survey.Status), "Nieprawid³owy status ankiety");
                    return BadRequest(ModelState);
                }

                if (submitInvestment)
                {
                    //UPDATE INVESTMENT STATUS
                    await this.updateInvestmentStatus(_context.Surveys.Where(s=>s.SurveyId == surveyId).Select(s=>s.InvestmentId).First());
                }
                
                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApproveAjax(Guid surveyId)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

                int Result = this.SetApprovedStatus(surveyId, true);

                if (Result == 0)
                {
                    ModelState.AddModelError(nameof(Survey.Status), "Nieprawid³owy status ankiety");
                    return BadRequest(ModelState);
                }

                //UPDATE INVESTMENT STATUS
                await this.updateInvestmentStatus(_context.Surveys.Where(s => s.SurveyId == surveyId).Select(s => s.InvestmentId).First());

                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RejectAjax(Guid surveyId)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

                int Result = this.SetRejectedStatus(surveyId, true);

                if (Result == 0)
                {
                    ModelState.AddModelError(nameof(Survey.Status), "Nieprawid³owy status ankiety");
                    return BadRequest(ModelState);
                }

                //UPDATE INVESTMENT STATUS
                await this.updateInvestmentStatus(_context.Surveys.Where(s => s.SurveyId == surveyId).Select(s => s.InvestmentId).First());

                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }
        [HttpPost]
        public async Task<IActionResult> RevertAjax(Guid surveyId)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

                int Result = this.SetDraftStatus(surveyId, true,true);

                if (Result == 0)
                {
                    ModelState.AddModelError(nameof(Survey.Status), "Nieprawidłowy status ankiety");
                    return BadRequest(ModelState);
                }

                //UPDATE INVESTMENT STATUS
                await this.updateInvestmentStatus(_context.Surveys.Where(s => s.SurveyId == surveyId).Select(s => s.InvestmentId).First());

                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }
        public async Task<IActionResult> CreateAjax(SurveyCreateViewModel model)
        {
            try
            {
                _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                switch (model.SurveyType)
                {
                    case SurveyType.CentralHeating:
                        SurveyCentralHeating srvCH = new SurveyCentralHeating()
                        {
                            InvestmentId = model.InvestmentId,
                            Status = SurveyStatus.New,
                            RSEType = model.RSETypeCH.Value,
                            Type = model.SurveyType
                        };

                        _context.Add(srvCH);
                        break;
                    case SurveyType.Energy:
                        SurveyEnergy srvEN = new SurveyEnergy()
                        {
                            InvestmentId = model.InvestmentId,
                            Status = SurveyStatus.New,
                            RSEType = model.RSETypeEN.Value,
                            Type = model.SurveyType
                        };
                        _context.Add(srvEN);
                        break;
                    case SurveyType.HotWater:
                        SurveyHotWater srvHW = new SurveyHotWater()
                        {
                            InvestmentId = model.InvestmentId,
                            Status = SurveyStatus.New,
                            RSEType = model.RSETypeHW.Value,
                            Type = model.SurveyType
                        };
                        _context.Add(srvHW);
                        break;
                    default:
                        break;
                }


                int Result = await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
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

        private int SetStatusOnEdit (Guid surveyId)
        {
            Investment inv = _context.Investments.Single(i => i.Surveys.Any(s=>s.SurveyId == surveyId) == true);

            switch (inv.InspectionStatus)
            {
                case InspectionStatus.ValueToDelete:
                case InspectionStatus.NotExists:
                case InspectionStatus.Draft:
                case InspectionStatus.Rejected:
                    this.SetDraftStatus(surveyId, true);
                    inv.Status = InvestmentStatus.Initial;
                    inv.InspectionStatus = InspectionStatus.Draft;
                    _context.Investments.Update(inv);
                    return _context.SaveChanges();

                case InspectionStatus.Submitted:
                case InspectionStatus.Approved:
                case InspectionStatus.Completed:
                    break;
                default:
                    break;
            }
            return 0;
        }
        private int SetDraftStatus (Guid surveyId, bool commit = false, bool force = false)
        {
            try
            {
                switch (_context.Surveys.Where(s => s.SurveyId == surveyId).Select(s => s.Type).First())
                {
                    case SurveyType.CentralHeating:
                        SurveyCentralHeating srvCH = _context.SurveysCH.Single(s => s.SurveyId == surveyId);
                        if (srvCH.Status == SurveyStatus.New || 
                            srvCH.Status == SurveyStatus.Rejected || 
                            (srvCH.Status == SurveyStatus.Cancelled && force))
                            srvCH.Status = SurveyStatus.Draft;
                        _context.Update(srvCH);
                        break;
                    case SurveyType.HotWater:
                        SurveyHotWater srvHW = _context.SurveysHW.Single(s => s.SurveyId == surveyId);
                        if (srvHW.Status == SurveyStatus.New || 
                            srvHW.Status == SurveyStatus.Rejected ||
                            (srvHW.Status == SurveyStatus.Cancelled && force))
                            srvHW.Status = SurveyStatus.Draft;
                        _context.Update(srvHW);
                        break;
                    case SurveyType.Energy:
                        SurveyEnergy srvEN = _context.SurveysEN.Single(s => s.SurveyId == surveyId);
                        if (srvEN.Status == SurveyStatus.New || 
                            srvEN.Status == SurveyStatus.Rejected ||
                           (srvEN.Status == SurveyStatus.Cancelled && force))
                            srvEN.Status = SurveyStatus.Draft;
                        _context.Update(srvEN);
                        break;
                    default:
                        return 0;
                }
                
                if (commit)
                {
                    return _context.SaveChanges();
                }
                else
                    return 1;
            }
            catch (Exception)
            {
                //0 records have been updated
                return 0;
            }
            //

        }
        /// <summary>
        /// Sets Survey status to "Approval"
        /// </summary>
        /// <param name="surveyId">Survey to upate the status</param>
        /// <param name="commit">commits changes on DB</param>
        /// <returns>number of updated records</returns>
        private int SetApprovalStatus(Guid surveyId, bool commit = false)
        {
            try
            {
                switch (_context.Surveys.Where(s => s.SurveyId == surveyId).Select(s => s.Type).First())
                {
                    case SurveyType.CentralHeating:
                        SurveyCentralHeating srvCH = _context.SurveysCH.Single(s => s.SurveyId == surveyId);
                        if (srvCH.Status != SurveyStatus.Draft)
                            return 0;
                        else
                        {
                            srvCH.Status = SurveyStatus.Approval;
                            _context.Update(srvCH);
                        }
                        break;
                    case SurveyType.HotWater:
                        SurveyHotWater srvHW = _context.SurveysHW.Single(s => s.SurveyId == surveyId);
                        if (srvHW.Status != SurveyStatus.Draft)
                            return 0;
                        else
                        {
                            srvHW.Status = SurveyStatus.Approval;
                            _context.Update(srvHW);
                        }
                        break;
                    case SurveyType.Energy:
                        SurveyEnergy srvEN = _context.SurveysEN.Single(s => s.SurveyId == surveyId);
                        if (srvEN.Status != SurveyStatus.Draft)
                            return 0;
                        else
                        {
                            srvEN.Status = SurveyStatus.Approval;
                            _context.Update(srvEN);
                        }
                        break;
                    default:
                        return 0;
                }
                if (commit)
                {
                    return _context.SaveChanges();
                }
                else
                    return 1;
            }
            catch (Exception)
            {
                //0 records have been updated
                return 0;
            }
            //

        }
        /// <summary>
        /// Sets Survey status to "Approved"
        /// </summary>
        /// <param name="surveyId">Survey to upate the status</param>
        /// <param name="commit">commits changes on DB</param>
        /// <returns>number of updated records</returns>
        private int SetApprovedStatus(Guid surveyId, bool commit = false)
        {
            try
            {
                switch (_context.Surveys.Where(s => s.SurveyId == surveyId).Select(s => s.Type).First())
                {
                    case SurveyType.CentralHeating:
                        SurveyCentralHeating srvCH = _context.SurveysCH.Single(s => s.SurveyId == surveyId);
                        if (srvCH.Status != SurveyStatus.Approval)
                            return 0;
                        else
                        {
                            srvCH.Status = SurveyStatus.Approved;
                            _context.Update(srvCH);
                        }
                        break;
                    case SurveyType.HotWater:
                        SurveyHotWater srvHW = _context.SurveysHW.Single(s => s.SurveyId == surveyId);
                        if (srvHW.Status != SurveyStatus.Approval)
                            return 0;
                        else
                        {
                            srvHW.Status = SurveyStatus.Approved;
                            _context.Update(srvHW);
                        }
                        break;
                    case SurveyType.Energy:
                        SurveyEnergy srvEN = _context.SurveysEN.Single(s => s.SurveyId == surveyId);
                        if (srvEN.Status != SurveyStatus.Approval)
                            return 0;
                        else
                        {
                            srvEN.Status = SurveyStatus.Approved;
                            _context.Update(srvEN);
                        }
                        break;
                    default:
                        return 0;
                }
                if (commit)
                {
                    return _context.SaveChanges();
                }
                else
                    return 1;
            }
            catch (Exception)
            {
                //0 records have been updated
                return 0;
            }
            //

        }

        /// <summary>
        /// Sets Survey status to "Approved"
        /// </summary>
        /// <param name="surveyId">Survey to upate the status</param>
        /// <param name="commit">commits changes on DB</param>
        /// <returns>number of updated records</returns>
        private int SetRejectedStatus(Guid surveyId, bool commit = false)
        {
            try
            {
                switch (_context.Surveys.Where(s => s.SurveyId == surveyId).Select(s => s.Type).First())
                {
                    case SurveyType.CentralHeating:
                        SurveyCentralHeating srvCH = _context.SurveysCH.Single(s => s.SurveyId == surveyId);
                        if (srvCH.Status != SurveyStatus.Approval)
                            return 0;
                        else
                        {
                            srvCH.Status = SurveyStatus.Rejected;
                            _context.Update(srvCH);
                        }
                        break;
                    case SurveyType.HotWater:
                        SurveyHotWater srvHW = _context.SurveysHW.Single(s => s.SurveyId == surveyId);
                        if (srvHW.Status != SurveyStatus.Approval)
                            return 0;
                        else
                        {
                            srvHW.Status = SurveyStatus.Rejected;
                            _context.Update(srvHW);
                        }
                        break;
                    case SurveyType.Energy:
                        SurveyEnergy srvEN = _context.SurveysEN.Single(s => s.SurveyId == surveyId);
                        if (srvEN.Status != SurveyStatus.Approval)
                            return 0;
                        else
                        {
                            srvEN.Status = SurveyStatus.Rejected;
                            _context.Update(srvEN);
                        }
                        break;
                    default:
                        return 0;
                }
                if (commit)
                {
                    return _context.SaveChanges();
                }
                else
                    return 1;
            }
            catch (Exception)
            {
                //0 records have been updated
                return 0;
            }
            //

        }

        private async Task<int> updateInvestmentStatus( Guid investmentId)
        {
            int Result = 0;

            Investment inv = _context.Investments
                .Include(i=>i.Surveys)
                .Single(i => i.InvestmentId == investmentId);

            if (inv.Surveys.Any(s => s.Status == SurveyStatus.Draft ||
                                     s.Status == SurveyStatus.New))
            {
                //draft
                inv.InspectionStatus = InspectionStatus.Draft;
                inv.Status = InvestmentStatus.Initial;
                _context.Investments.Update(inv);
                Result = await _context.SaveChangesAsync();
            }
            else if (inv.Surveys.All(s => s.Status == SurveyStatus.Cancelled))
            {   //completed
                inv.InspectionStatus = InspectionStatus.Completed;
                inv.Status = InvestmentStatus.Cancelled;
                _context.Investments.Update(inv);
                Result = await _context.SaveChangesAsync();
            }
            else if (inv.Surveys.All(s => s.Status == SurveyStatus.Approved ||
                                          s.Status == SurveyStatus.Cancelled))
            {   //completed
                inv.InspectionStatus = InspectionStatus.Completed;
                inv.Status = InvestmentStatus.Completed;
                _context.Investments.Update(inv);
                Result = await _context.SaveChangesAsync();
            }
            else if (inv.Surveys.All(s => s.Status == SurveyStatus.Rejected ||
                                          s.Status == SurveyStatus.Approved ||
                                          s.Status == SurveyStatus.Cancelled))
            {   //rejected
                inv.InspectionStatus = InspectionStatus.Rejected;
                inv.Status = InvestmentStatus.Initial;
                _context.Investments.Update(inv);
                Result = await _context.SaveChangesAsync();
            }
            else if (inv.Surveys.All(s => s.Status == SurveyStatus.Approval ||
                                          s.Status == SurveyStatus.Approved ||
                                          s.Status == SurveyStatus.Cancelled))
            {
                //submitted
                inv.InspectionStatus = InspectionStatus.Submitted;
                inv.Status = InvestmentStatus.Initial;
                _context.Investments.Update(inv);
                Result = await _context.SaveChangesAsync();
            }

            return Result;
        }

        public bool isSurveyType( int type, Guid surveyId)
        {
            int RSEType;
            switch (_context.Surveys.Where(s => s.SurveyId == surveyId).Select(s=>s.Type).SingleOrDefault())
            {
                case SurveyType.CentralHeating:
                    RSEType = (int)_context.SurveysCH.Where(s=>s.SurveyId == surveyId).Select(s=>s.RSEType).SingleOrDefault();
                    break;
                case SurveyType.Energy:
                    RSEType = (int)_context.SurveysEN.Where(s => s.SurveyId == surveyId).Select(s => s.RSEType).SingleOrDefault();
                    break;
                case SurveyType.HotWater:
                    RSEType = (int)_context.SurveysHW.Where(s => s.SurveyId == surveyId).Select(s => s.RSEType).SingleOrDefault();
                    break;
                default:
                    RSEType = -4;
                    break;
            }

            return type == RSEType;
        }
    }
}