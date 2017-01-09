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
            //disable change tracking
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            //View model
            SurveyExtractViewModel model = new SurveyExtractViewModel();

            model.ProjectList = new SelectList(await _context.Projects.Select(p => new SelectListItem() {
                Value = p.ProjectId.ToString(),
                Text = p.ShortDescription + " (" +
                       p.StartDate.ToShortDateString() + " - " +
                       p.EndDate.ToShortDateString() + ")"
            }).ToListAsync(),"Value","Text",null);

            model.ContractList = new SelectList(await _context.Contracts.Select(c => new SelectListItem() {
                 Value = c.ContractId.ToString(),
                 Text = c.FullfilmentDate.HasValue ?  
                        c.Number + ' ' +
                        c.ContractDate.ToShortDateString() + " - " +
                        c.FullfilmentDate.Value.ToShortDateString() + ' ' +
                        c.ShortDescription :

                        c.Number + " " +
                        c.ContractDate.ToShortDateString() + " " +
                        c.ShortDescription
            }).ToListAsync(), "Value","Text",null);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SurveyToCSV(int? projectId, int? contractId, Guid? investmentId)
        {
            //disable change tracking
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            //file name
            string fileName = "DaneZAnkiet";
            if (projectId.HasValue) fileName = fileName + "_Projekt=" + projectId.ToString();
            if (contractId.HasValue) fileName = fileName + "_Umowa=" + contractId.ToString();
            if (investmentId.HasValue) fileName = fileName + "_Inwestycja=" + investmentId.ToString();
            fileName += ".csv";

            //
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

            sl = sl.Include(s => s.Investment).ThenInclude(i => i.InvestmentOwners).ThenInclude(io => io.Owner).ThenInclude(o => o.Address)
                   .Include(s => s.Investment).ThenInclude(i => i.Address)
                   .Include(s => s.AirCondition)
                   .Include(s => s.Audit)
                   .Include(s => s.BathRoom)
                   .Include(s => s.BoilerRoom)
                   .Include(s => s.Building)
                   .Include(s => s.Ground)
                   .Include(s => s.PlannedInstall)
                   .Include(s => s.RoofPlanes)
                   .Include(s => s.Wall);

            var result = await sl.ToListAsync();

            CsvExport list = await SurveyListAsCSV(result);
            return File(list.ExportToBytes(), "application/csv", fileName);
    }

        private async Task<CsvExport> SurveyListAsCSV(List<Survey> data)
        {
            CsvExport myExport = new CsvExport(columnSeparator: ";");
            
            foreach (var srv in data)
            {
                //HEADER
                myExport.AddRow();

                //DATA

                //SURVEY GENERAL
                myExport["TYP OZE"] = srv.TypeFullDescription();
                myExport["STATUS ANKIETY"] = srv.Status.DisplayName();

                //INWESTYCJA
                myExport["INWESTYCJA - ADRES"] = srv.Investment.Address.SingleLine;


                //WŁAŚCICIELE
                for (int i = 0; i < 3; i++)
                {
                    if (srv.Investment.InvestmentOwners != null & srv.Investment.InvestmentOwners.Count == (i + 1))
                    {

                        myExport["WŁAŚCICIEL " + i.ToString()] = srv.Investment.InvestmentOwners.ElementAt(i).Owner.PartnerName2 + " " + srv.Investment.InvestmentOwners.ElementAt(i).Owner.PartnerName1;
                        myExport["WŁAŚCICIEL ADRES " + i.ToString()] = srv.Investment.InvestmentOwners.ElementAt(i).Owner.Address.SingleLine;
                    }
                    else
                    {
                        myExport["WŁAŚCICIEL " + i.ToString()] = "";
                        myExport["WŁAŚCICIEL ADRES " + i.ToString()] = "";
                    }
                }

                //INWESTYCJA OGOLNE

                myExport["RODZ. DZIAŁALN."] = srv.Investment.BusinessActivity.DisplayName();
                myExport["PALIWO GŁ.CO"] = srv.Investment.CentralHeatingFuel.DisplayName();
                myExport["RODZAJ GŁ.CO"] = srv.Investment.CentralHeatingType != CentralHeatingType.Other ?
                                           srv.Investment.CentralHeatingType.DisplayName() :
                                           srv.Investment.CentralHeatingTypeOther;
                myExport["ROK BUDOWY"] = srv.Investment.CompletionYear.ToString();
                myExport["POW. OGRZEWANA"] = srv.Investment.HeatedArea.ToString();
                myExport["PALIWO GŁ. CW"] = srv.Investment.HotWaterFuel.DisplayName();
                myExport["RODZAJ GŁ. CW"] = srv.Investment.HotWaterType.DisplayName();
                myExport["INTERNET W M.INW."] = srv.Investment.InternetAvailable.AsYesNo();
                myExport["NR KS. WIECZ."] = srv.Investment.LandRegisterNo;
                myExport["L.MIESZKAŃCÓW"] = srv.Investment.NumberOfOccupants.ToString();
                //myExport[""] = srv.Investment.PlotAreaNumber;
                myExport["NR DZIAŁKI"] = srv.Investment.PlotNumber;
                myExport["STAN BUD."] = srv.Investment.Stage.DisplayName();
                myExport["POW. CAŁK."] = srv.Investment.TotalArea.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                myExport["RODZ.BUD."] = srv.Investment.Type.DisplayName();
                myExport["POW. UŻYTK."] = srv.Investment.UsableArea.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);

                //AIRCOND
                if (srv.AirCondition != null)
                {
                    myExport["ISTN. KLIMAT."] = srv.AirCondition.Exists.AsYesNo();
                    myExport["KLIMAT.PLANOWANA"] = srv.AirCondition.isPlanned.AsYesNo();
                    myExport["MECH.WENT.ISTN."] = srv.AirCondition.MechVentilationExists.AsYesNo();
                    myExport["RODZAJ WENT."] = srv.AirCondition.Type.DisplayName();
                }
                else
                {
                    myExport["ISTN. KLIMAT."] = "";
                    myExport["KLIMAT.PLANOWANA"] = "";
                    myExport["MECH.WENT.ISTN."] = "";
                    myExport["RODZAJ WENT."] = "";
                }
                //ENERGY AUDIT
                if (srv.Audit != null)
                {
                    myExport["PAR.DOD.ŹR.CIEPŁA"] = srv.Audit.AdditionalHeatParams;
                    myExport["DOD.ŹR.CIEPŁA"] = srv.Audit.AdditionalHeatSource.AsYesNo();
                    myExport["ŚR.ROCZNE ZUŻYCIE CO"] = srv.Audit.AverageYearlyFuelConsumption.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                    myExport["ŚR.ROCZNE KOSZTY CO"] = srv.Audit.AverageYearlyHeatingCosts.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                    myExport["MAX TEMP. PIECA"] = srv.Audit.BoilerMaxTemp.ToString();
                    myExport["PIEC - MOC"] = srv.Audit.BoilerNominalPower.ToString();
                    myExport["PIEC - PLAN. WYM."] = srv.Audit.BoilerPlannedReplacement.AsYesNo();
                    myExport["PIEC - ROK PROD."] = srv.Audit.BoilerProductionYear.ToString();
                    myExport["CO PODLOG."] = srv.Audit.CHFRadiantFloorInstalled.AsYesNo();
                    myExport["PLAN.POMPA BĘDZIE JEDYNYM ŹR."] = srv.Audit.CHIsHPOnlySource.AsYesNo();
                    myExport["CO PODLOG. - PROCENT POW."] = srv.Audit.CHRadiantFloorAreaPerc.ToString();
                    myExport["CO GRZEJNIKI"] = srv.Audit.CHRadiatorsInstalled.AsYesNo();
                    myExport["CO GRZEJNIKI - TYP"] = srv.Audit.CHRadiatorType.DisplayName();
                    myExport["UMOWA KOMPLEKS."] = srv.Audit.ComplexAgreement.AsYesNo();
                    myExport["EE - ŚR. KOSZT/MC."] = srv.Audit.ElectricityAvgMonthlyCost.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                    myExport["EE - MOC PRZYŁ."] = srv.Audit.ElectricityPower.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                    myExport["EE - DOD. LICZNIK"] = srv.Audit.ENAdditionalConsMeter.AsYesNo();
                    myExport["EE - UZIEMIENIE"] = srv.Audit.ENIsGround.AsYesNo();
                    myExport["EE - PLANOWANA MOC"] = srv.Audit.ENPowerLevel.ToString();
                    myExport["CW - MOC"] = srv.Audit.HWSourcePower.ToString();
                    myExport["EE - L. FAZ"] = srv.Audit.PhaseCount.DisplayName();
                    myExport["EE - ROCZNE ZUŻYCIE"] = srv.Audit.PowerAvgYearlyConsumption.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                    myExport["EE - DYSTRYBUTOR"] = srv.Audit.PowerCompanyName.DisplayName();
                    myExport["EE - UMIEJSC. LICZNIKA"] = srv.Audit.PowerConsMeterLocation.DisplayName();
                    myExport["EE - RODZ. PRZYŁ."] = srv.Audit.PowerSupplyType.DisplayName();
                    myExport["CW - POW. WĘŻ."] = srv.Audit.TankCoilSize.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                    myExport["CW - ISTN. ZASOBNIK"] = srv.Audit.TankExists.AsYesNo();
                    myExport["CW - OBJ. ZASOBNIKA"] = srv.Audit.TankVolume.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                }
                else
                {
                    myExport["PAR.DOD.ŹR.CIEPŁA"] = "";
                    myExport["DOD.ŹR.CIEPŁA"] = "";
                    myExport["ŚR.ROCZNE ZUŻYCIE CO"] = "";
                    myExport["ŚR.ROCZNE KOSZTY CO"] = "";
                    myExport["MAX TEMP. PIECA"] = "";
                    myExport["PIEC - MOC"] = "";
                    myExport["PIEC - PLAN. WYM."] = "";
                    myExport["PIEC - ROK PROD."] = "";
                    myExport["CO PODLOG."] = "";
                    myExport["PLAN.POMPA BĘDZIE JEDYNYM ŹR."] = "";
                    myExport["CO PODLOG. - PROCENT POW."] = "";
                    myExport["CO GRZEJNIKI"] = "";
                    myExport["CO GRZEJNIKI - TYP"] = "";
                    myExport["UMOWA KOMPLEKS."] = "";
                    myExport["EE - ŚR. KOSZT/MC."] = "";
                    myExport["EE - MOC PRZYŁ."] = "";
                    myExport["EE - DOD. LICZNIK"] = "";
                    myExport["EE - UZIEMIENIE"] = "";
                    myExport["EE - PLANOWANA MOC"] = "";
                    myExport["CW - MOC"] = "";
                    myExport["EE - L. FAZ"] = "";
                    myExport["EE - ROCZNE ZUŻYCIE"] = "";
                    myExport["EE - DYSTRYBUTOR"] = "";
                    myExport["EE - UMIEJSC. LICZNIKA"] = "";
                    myExport["EE - RODZ. PRZYŁ."] = "";
                    myExport["CW - POW. WĘŻ."] = "";
                    myExport["CW - ISTN. ZASOBNIK"] = "";
                    myExport["CW - OBJ. ZASOBNIKA"] = "";
                }

                //BATHROOM
                if (srv.BathRoom != null)
                {
                    myExport["ISTN. ŁAŹ."] = srv.BathRoom.BathExsists.AsYesNo();
                    myExport["OBJ. WANNY"] = srv.BathRoom.BathVolume.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                    myExport["L.ŁAZIENEK"] = srv.BathRoom.NumberOfBathrooms.ToString();
                    myExport["ISTN.PRYSZNIC"] = srv.BathRoom.ShowerExists.AsYesNo();
                }
                else
                {
                    myExport["ISTN. ŁAŹ."] = "";
                    myExport["OBJ. WANNY"] = "";
                    myExport["L.ŁAZIENEK"] = "";
                    myExport["ISTN.PRYSZNIC"] = "";
                }
                //BOILERROOM
                if (srv.BoilerRoom != null)
                {
                    myExport["KOTŁOWNIA - ISTNIEJE"] = srv.BoilerRoom.RoomExists.AsYesNo();
                    myExport["KOTŁOWNIA - ISTN.WOLNY PRZEW.WENT"] = srv.BoilerRoom.AirVentilationExists.AsYesNo();
                    myExport["KOTŁOWNIA - SZER.DRZWI"] = srv.BoilerRoom.DoorHeight.ToString();
                    myExport["KOTŁOWNIA - WYS."] = srv.BoilerRoom.Height.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                    myExport["KOTŁOWNIA - INST.400V"] = srv.BoilerRoom.HighVoltagePowerSupply.AsYesNo();
                    myExport["KOTŁOWNIA - ISTN. CYRKULACJA"] = srv.BoilerRoom.HWCirculationInstalled.AsYesNo();
                    myExport["KOTŁOWNIA - ISTN. INST. CW"] = srv.BoilerRoom.HWInstalled.AsYesNo();
                    myExport["KOTŁOWNIA - ISTN. REDUKTOR C."] = srv.BoilerRoom.HWPressureReductorExists.AsYesNo();
                    myExport["KOTŁOWNIA - SUCHA I > 0 ST."] = srv.BoilerRoom.IsDryAndWarm.AsYesNo();
                    myExport["KOTŁOWNIA - DŁUG."] = srv.BoilerRoom.Length.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                    myExport["KOTŁOWNIA - ISTN. 3 UZIEM.GNIAZDA"] = srv.BoilerRoom.ThreePowerSuppliesExists.AsYesNo();
                    myExport["KOTŁOWNIA - KUBATURA"] = srv.BoilerRoom.Volume.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                    myExport["KOTŁOWNIA - SZER."] = srv.BoilerRoom.Width.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                }
                else
                {
                    myExport["KOTŁOWNIA - ISTNIEJE"] = "";
                    myExport["KOTŁOWNIA - ISTN.WOLNY PRZEW.WENT"] = "";
                    myExport["KOTŁOWNIA - SZER.DRZWI"] = "";
                    myExport["KOTŁOWNIA - WYS."] = "";
                    myExport["KOTŁOWNIA - INST.400V"] = "";
                    myExport["KOTŁOWNIA - ISTN. CYRKULACJA"] = "";
                    myExport["KOTŁOWNIA - ISTN. INST. CW"] = "";
                    myExport["KOTŁOWNIA - ISTN. REDUKTOR C."] = "";
                    myExport["KOTŁOWNIA - SUCHA I > 0 ST."] = "";
                    myExport["KOTŁOWNIA - DŁUG."] = "";
                    myExport["KOTŁOWNIA - ISTN. 3 UZIEM.GNIAZDA"] = "";
                    myExport["KOTŁOWNIA - KUBATURA"] = "";
                    myExport["KOTŁOWNIA - SZER."] = "";
                }

                //BUILDING
                if (srv.Building != null)
                {
                    myExport["IZOLACJA - GRUBOŚĆ"] = srv.Building.InsulationThickness.ToString();
                    myExport["IZOLACJA - RODZAJ"] = srv.Building.InsulationType == InsulationType.Ins_3 ?
                                   srv.Building.InsulationTypeOther.ToString() :
                                   srv.Building.InsulationType.DisplayName();
                    myExport["TECHNOLOGIA WYKONANIA"] = srv.Building.TechnologyType.DisplayName();
                    myExport["KUBATURA BUD."] = srv.Building.Volume.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                    myExport["MATERIAŁ ŚCIAN"] = srv.Building.WallMaterialOther != null ? srv.Building.WallMaterialOther.ToString() : "";
                    myExport["GRUBOŚĆ ŚCIAN"] = srv.Building.WallThickness.ToString();
                }
                else
                {
                    myExport["IZOLACJA - GRUBOŚĆ"] = "";
                    myExport["IZOLACJA - RODZAJ"] = "";
                    myExport["TECHNOLOGIA WYKONANIA"] = "";
                    myExport["KUBATURA BUD."] = "";
                    myExport["MATERIAŁ ŚCIAN"] = "";
                    myExport["GRUBOŚĆ ŚCIAN"] = "";
                }

                //GENERAL
                myExport["ANULOWANA - KOMENTARZ"] = srv.CancelComments != null ? srv.CancelComments.ToString() : "";
                myExport["ANULOWANA - POWÓD"] = srv.CancelType.HasValue ? srv.CancelType.DisplayName() : "";
                myExport["OST.ZM. - DATA"] = string.Format("{0:yyyy-M-dd hh:mm:ss}", srv.ChangedAt);
                if (srv.ChangedBy != Guid.Empty)
                {
                    var usr = await _userManager.FindByIdAsync(srv.ChangedBy.ToString());
                    myExport["OST.ZM. - PRZEZ"] = usr.LastName + " " + usr.FirstName;
                }
                else
                {
                    myExport["OST.ZM. - PRZEZ"] = "";
                }
                
                myExport["UWAGI"] = srv.FreeCommments != null ? srv.FreeCommments.ToString() : "";
                myExport["ZAPLACONA"] = srv.IsPaid.AsYesNo();

                //GROUND
                if (srv.Ground != null)
                {
                    myExport["GRUNT - POW."] = srv.Ground.Area.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                    myExport["GRUNT - BYLY TEREN WOJSK"] = srv.Ground.FormerMilitary.AsYesNo();
                    myExport["GRUNT - ISTN.INSTALACJA"] = srv.Ground.OtherInstallation.AsYesNo();
                    myExport["GRUNT - INSTALACJA TYP"] = srv.Ground.OtherInstallationType != null ? srv.Ground.OtherInstallationType : "";
                    myExport["GRUNT - GRUZ,SKAŁY"] = srv.Ground.Rocks.AsYesNo();
                    myExport["GRUNT - NACHYLENIE"] = srv.Ground.SlopeTerrain.DisplayName();
                    myExport["GRUNT - PODMOKŁY"] = srv.Ground.WetLand.AsYesNo();
                }
                else
                {
                    myExport["GRUNT - POW."] = "";
                    myExport["GRUNT - BYLY TEREN WOJSK"] = "";
                    myExport["GRUNT - ISTN.INSTALACJA"] = "";
                    myExport["GRUNT - INSTALACJA TYP"] = "";
                    myExport["GRUNT - GRUZ,SKAŁY"] = "";
                    myExport["GRUNT - NACHYLENIE"] = "";
                    myExport["GRUNT - PODMOKŁY"] = "";
                }

                // PLANNED INSTALLATION
                if (srv.PlannedInstall != null)
                {
                    myExport["ZESTAW KLIENTA"] = srv.PlannedInstall.Configuration.DisplayName();
                    myExport["LOKALIZACJA INSTALACJI"] = srv.PlannedInstall.Localization.DisplayName();
                    myExport["INSTALACJA NA SCIANIE"] = srv.PlannedInstall.OnWallPlacementAvailable.AsYesNo();
                    myExport["PRZEZN. BUDYNKU"] = srv.PlannedInstall.Purpose.DisplayName();
                }
                else
                {
                    myExport["ZESTAW KLIENTA"] = "";
                    myExport["LOKALIZACJA INSTALACJI"] = "";
                    myExport["INSTALACJA NA SCIANIE"] = "";
                    myExport["PRZEZN. BUDYNKU"] = "";
                }

                //myExport[""] = srv.RejectComments.ToString();

                for (int i = 0; i < 3; i++)
                {
                    if (srv.RoofPlanes != null && srv.RoofPlanes.Count == (i + 1))
                    {
                        var roof = srv.RoofPlanes.ElementAt(i);

                        myExport["POŁAĆ " + i.ToString() + " TYP"] = roof.Type.DisplayName();
                        myExport["POŁAĆ " + i.ToString() + " WYS.BUD."] = roof.BuildingHeight.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                        myExport["POŁAĆ " + i.ToString() + " KOMINY"] = roof.Chimney.AsYesNo();
                        myExport["POŁAĆ " + i.ToString() + " DŁ.KRAW."] = roof.EdgeLength.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                        myExport["POŁAĆ " + i.ToString() + " INSTALACJA POD"] = roof.InstallationUnderPlane.AsYesNo();
                        myExport["POŁAĆ " + i.ToString() + " DŁUG."] = roof.Width.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                        myExport["POŁAĆ " + i.ToString() + " SZER."] = roof.Length.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                        myExport["POŁAĆ " + i.ToString() + " INST.ODGROM"] = roof.LightingProtection.AsYesNo();
                        myExport["POŁAĆ " + i.ToString() + " WYS.OKAPU"] = roof.OkapHeight.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                        myExport["POŁAĆ " + i.ToString() + " DŁ.GRZBIETU"] = roof.RidgeWeight.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                        myExport["POŁAĆ " + i.ToString() + " DŁ. DACHU"] = roof.RoofLength.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                        myExport["POŁAĆ " + i.ToString() + " POKRYCIE"] = roof.RoofMaterial.DisplayName();
                        myExport["POŁAĆ " + i.ToString() + " ŚWIETLIKI"] = roof.SkyLights.AsYesNo();
                        myExport["POŁAĆ " + i.ToString() + " KĄT NACH."] = roof.SlopeAngle.ToString();
                        myExport["POŁAĆ " + i.ToString() + " POWIERZCHNIA"] = roof.SurfaceArea.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                        myExport["POŁAĆ " + i.ToString() + " AZYMUT"] = roof.SurfaceAzimuth.ToString();
                        myExport["POŁAĆ " + i.ToString() + " OKNA"] = roof.Windows.AsYesNo();
                    }
                    else
                    {
                        myExport["POŁAĆ " + i.ToString() + " TYP"] = "";
                        myExport["POŁAĆ " + i.ToString() + " WYS.BUD."] = "";
                        myExport["POŁAĆ " + i.ToString() + " KOMINY"] = "";
                        myExport["POŁAĆ " + i.ToString() + " DŁ.KRAW."] = "";
                        myExport["POŁAĆ " + i.ToString() + " INSTALACJA POD"] = "";
                        myExport["POŁAĆ " + i.ToString() + " DŁUG."] = "";
                        myExport["POŁAĆ " + i.ToString() + " SZER."] = "";
                        myExport["POŁAĆ " + i.ToString() + " INST.ODGROM"] = "";
                        myExport["POŁAĆ " + i.ToString() + " WYS.OKAPU"] = "";
                        myExport["POŁAĆ " + i.ToString() + " DŁ.GRZBIETU"] = "";
                        myExport["POŁAĆ " + i.ToString() + " DŁ. DACHU"] = "";
                        myExport["POŁAĆ " + i.ToString() + " POKRYCIE"] = "";
                        myExport["POŁAĆ " + i.ToString() + " ŚWIETLIKI"] = "";
                        myExport["POŁAĆ " + i.ToString() + " KĄT NACH."] = "";
                        myExport["POŁAĆ " + i.ToString() + " POWIERZCHNIA"] = "";
                        myExport["POŁAĆ " + i.ToString() + " AZYMUT"] = "";
                        myExport["POŁAĆ " + i.ToString() + " OKNA"] = "";
                    }
                }

                //srv.Status

                //WALL
                if (srv.Wall != null)
                {
                    myExport["ELEW - AZYMUT"] = srv.Wall.Azimuth.ToString();
                    myExport["ELEW - WYS."] = srv.Wall.Height.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                    myExport["ELEW - SZER."] = srv.Wall.Width.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                    myExport["ELEW - POW."] = srv.Wall.UsableArea.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                }
                else
                {
                    myExport["ELEW - AZYMUT"] = "";
                    myExport["ELEW - WYS."] = "";
                    myExport["ELEW - SZER."] = "";
                    myExport["ELEW - POW."] = "";
                }
            }

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