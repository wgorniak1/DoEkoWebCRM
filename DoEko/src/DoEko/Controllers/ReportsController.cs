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
            var sl = _context.Surveys.AsQueryable(); //Where(s => s.Status != SurveyStatus.Cancelled);

            if (projectId.HasValue)
            {
                //
                var project = _context.Projects
                    .Include(p=>p.ChildProjects)
                    .Single(p => p.ProjectId == projectId);
                if (project.ChildProjects != null && project.ChildProjects.Count > 0)
                {
                    sl = sl.Where(s => s.Investment.Contract.ProjectId == projectId || project.ChildProjects.Any(cp=>cp.ProjectId == s.Investment.Contract.ProjectId));
                }
                else
                {
                    sl = sl.Where(s => s.Investment.Contract.ProjectId == projectId);
                }
            }
            if (contractId.HasValue)
                sl = sl.Where(s => s.Investment.ContractId == contractId);
            if (investmentId.HasValue)
                sl = sl.Where(s => s.InvestmentId == investmentId);

            sl = sl.OrderBy(s => s.Investment.Contract.ProjectId)
                .ThenBy(s => s.Investment.ContractId)
                .ThenBy(s => s.InvestmentId);

            sl = sl.Include(s => s.Investment).ThenInclude(i => i.InvestmentOwners).ThenInclude(io => io.Owner).ThenInclude(o => o.Address)
                   .Include(s => s.Investment).ThenInclude(i => i.Address).ThenInclude(a=>a.State)
                   .Include(s => s.Investment).ThenInclude(i => i.Address).ThenInclude(a=>a.District)
                   .Include(s => s.Investment).ThenInclude(i => i.Address).ThenInclude(a=>a.Commune)
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
        private void AddRow( ref CsvExport csv)
        {
            csv.AddRow();

            //DATA
            csv["ID INWESTYCJI"] = "";
            csv["ID ANKIETY"] = "";
            csv["TYP OZE"] = "";
            csv["STATUS ANKIETY"] = "";
            csv["INWEST - ADRES - WOJ."] = "";
            csv["INWEST - ADRES - POW."] = "";
            csv["INWEST - ADRES - GM."] = "";
            csv["INWEST - ADRES - KOD"] = "";
            csv["INWEST - ADRES - MIEJSC"] = "";
            csv["INWEST - ADRES - ULICA"] = "";
            csv["INWEST - ADRES - NR BUD."] = "";
            csv["INWEST - ADRES - NR MIESZK"] = "";
            csv["WŁAŚCICIEL 0"] = "";
            csv["WŁAŚCICIEL ADRES 0"] = "";
            csv["WŁAŚCICIEL TEL 0"] = "";
            csv["WŁAŚCICIEL MAIL 0"] = "";
            csv["WŁAŚCICIEL 1"] = "";
            csv["WŁAŚCICIEL ADRES 1"] = "";
            csv["WŁAŚCICIEL TEL 1"] = "";
            csv["WŁAŚCICIEL MAIL 1"] = "";
            csv["WŁAŚCICIEL 2"] = "";
            csv["WŁAŚCICIEL ADRES 2"] = "";
            csv["WŁAŚCICIEL TEL 2"] = "";
            csv["WŁAŚCICIEL MAIL 2"] = "";
            csv["INTERNET W M.INW."] = "";
            csv["RODZ. DZIAŁALN."] = "";
            csv["NR KS. WIECZ."] = "";
            csv["NR DZIAŁKI"] = "";
            csv["PALIWO GŁ.CO"] = "";
            csv["RODZAJ GŁ.CO"] = "";
            csv["PALIWO GŁ. CW"] = "";
            csv["RODZAJ GŁ. CW"] = "";
            csv["STAN BUD."] = "";
            csv["ROK BUDOWY"] = "";
            csv["L.MIESZKAŃCÓW"] = "";
            csv["POW. UŻYTK."] = "";
            csv["POW. OGRZEWANA"] = "";
            csv["POW. CAŁK."] = "";
            csv["RODZ.BUD."] = "";
            csv["UMOWA KOMPLEKS."] = "";
            csv["EE - DYSTRYBUTOR"] = "";
            csv["EE - MOC PRZYŁ."] = "";
            csv["EE - RODZ. PRZYŁ."] = "";
            csv["EE - L. FAZ"] = "";
            csv["EE - UZIEMIENIE"] = "";
            csv["EE - UMIEJSC. LICZNIKA"] = "";
            csv["EE - DOD. LICZNIK"] = "";
            csv["EE - ROCZNE ZUŻYCIE"] = "";
            csv["EE - ŚR. KOSZT/MC."] = "";
            csv["EE - PLANOWANA MOC"] = "";
            csv["KOTŁOWNIA - ISTNIEJE"] = "";
            csv["KOTŁOWNIA - SZER.DRZWI"] = "";
            csv["KOTŁOWNIA - DŁUG."] = "";
            csv["KOTŁOWNIA - SZER."] = "";
            csv["KOTŁOWNIA - WYS."] = "";
            csv["KOTŁOWNIA - KUBATURA"] = "";
            csv["KOTŁOWNIA - ISTN. INST. CW"] = "";
            csv["KOTŁOWNIA - ISTN. CYRKULACJA"] = "";
            csv["KOTŁOWNIA - ISTN. REDUKTOR C."] = "";
            csv["KOTŁOWNIA - ISTN.WOLNY PRZEW.WENT"] = "";
            csv["KOTŁOWNIA - SUCHA I > 0 ST."] = "";
            csv["KOTŁOWNIA - ISTN. 3 UZIEM.GNIAZDA"] = "";
            csv["KOTŁOWNIA - INST.400V"] = "";
            csv["L.ŁAZIENEK"] = "";
            csv["ISTN. WANNA"] = "";
            csv["OBJ. WANNY"] = "";
            csv["ISTN.PRYSZNIC"] = "";
            csv["ZESTAW KLIENTA"] = "";
            csv["CW - MOC"] = "";
            csv["PIEC - MOC"] = "";
            csv["PIEC - ROK PROD."] = "";
            csv["PIEC - PLAN. WYM."] = "";
            csv["CO GRZEJNIKI"] = "";
            csv["CO GRZEJNIKI - TYP"] = "";
            csv["CO PODLOG."] = "";
            csv["CO PODLOG. - PROCENT POW."] = "";
            csv["MAX TEMP. PIECA"] = "";
            csv["ŚR.ROCZNE ZUŻYCIE CO"] = "";
            csv["ŚR.ROCZNE KOSZTY CO"] = "";
            csv["MECH.WENT.ISTN."] = "";
            csv["ISTN. KLIMAT."] = "";
            csv["KLIMAT.PLANOWANA"] = "";
            csv["TYP INST.CHŁODZ."] = "";
            csv["DOD.ŹR.CIEPŁA"] = "";
            csv["PAR.DOD.ŹR.CIEPŁA"] = "";
            csv["CW - ISTN. ZASOBNIK"] = "";
            csv["CW - OBJ. ZASOBNIKA"] = "";
            csv["CW - POW. WĘŻ."] = "";
            csv["PLAN.POMPA BĘDZIE JEDYNYM ŹR."] = "";
            csv["TECHNOLOGIA WYKONANIA"] = "";
            csv["MATERIAŁ ŚCIAN"] = "";
            csv["GRUBOŚĆ ŚCIAN"] = "";
            csv["IZOLACJA - RODZAJ"] = "";
            csv["IZOLACJA - GRUBOŚĆ"] = "";
            csv["KUBATURA BUD."] = "";
            csv["LOKALIZACJA INSTALACJI"] = "";
            csv["INSTALACJA NA SCIANIE"] = "";
            csv["PRZEZN. BUDYNKU"] = "";
            csv["GRUNT - POW."] = "";
            csv["GRUNT - BYLY TEREN WOJSK"] = "";
            csv["GRUNT - ISTN.INSTALACJA"] = "";
            csv["GRUNT - INSTALACJA TYP"] = "";
            csv["GRUNT - GRUZ,SKAŁY"] = "";
            csv["GRUNT - NACHYLENIE"] = "";
            csv["GRUNT - PODMOKŁY"] = "";
            csv["POŁAĆ 0 TYP"] = "";
            csv["POŁAĆ 0 WYS.BUD."] = "";
            csv["POŁAĆ 0 WYS.OKAPU"] = "";
            csv["POŁAĆ 0 DŁ. DACHU"] = "";
            csv["POŁAĆ 0 DŁ.KRAW."] = "";
            csv["POŁAĆ 0 DŁ.GRZBIETU"] = "";
            csv["POŁAĆ 0 KĄT NACH."] = "";
            csv["POŁAĆ 0 DŁUG."] = "";
            csv["POŁAĆ 0 SZER."] = "";
            csv["POŁAĆ 0 POWIERZCHNIA"] = "";
            csv["POŁAĆ 0 POKRYCIE"] = "";
            csv["POŁAĆ 0 AZYMUT"] = "";
            csv["POŁAĆ 0 OKNA"] = "";
            csv["POŁAĆ 0 ŚWIETLIKI"] = "";
            csv["POŁAĆ 0 KOMINY"] = "";
            csv["POŁAĆ 0 INSTALACJA POD"] = "";
            csv["POŁAĆ 0 INST.ODGROM"] = "";
            csv["POŁAĆ 1 TYP"] = "";
            csv["POŁAĆ 1 WYS.BUD."] = "";
            csv["POŁAĆ 1 WYS.OKAPU"] = "";
            csv["POŁAĆ 1 DŁ. DACHU"] = "";
            csv["POŁAĆ 1 DŁ.KRAW."] = "";
            csv["POŁAĆ 1 DŁ.GRZBIETU"] = "";
            csv["POŁAĆ 1 KĄT NACH."] = "";
            csv["POŁAĆ 1 DŁUG."] = "";
            csv["POŁAĆ 1 SZER."] = "";
            csv["POŁAĆ 1 POWIERZCHNIA"] = "";
            csv["POŁAĆ 1 POKRYCIE"] = "";
            csv["POŁAĆ 1 AZYMUT"] = "";
            csv["POŁAĆ 1 OKNA"] = "";
            csv["POŁAĆ 1 ŚWIETLIKI"] = "";
            csv["POŁAĆ 1 KOMINY"] = "";
            csv["POŁAĆ 1 INSTALACJA POD"] = "";
            csv["POŁAĆ 1 INST.ODGROM"] = "";
            csv["POŁAĆ 2 TYP"] = "";
            csv["POŁAĆ 2 WYS.BUD."] = "";
            csv["POŁAĆ 2 WYS.OKAPU"] = "";
            csv["POŁAĆ 2 DŁ. DACHU"] = "";
            csv["POŁAĆ 2 DŁ.KRAW."] = "";
            csv["POŁAĆ 2 DŁ.GRZBIETU"] = "";
            csv["POŁAĆ 2 KĄT NACH."] = "";
            csv["POŁAĆ 2 DŁUG."] = "";
            csv["POŁAĆ 2 SZER."] = "";
            csv["POŁAĆ 2 POWIERZCHNIA"] = "";
            csv["POŁAĆ 2 POKRYCIE"] = "";
            csv["POŁAĆ 2 AZYMUT"] = "";
            csv["POŁAĆ 2 OKNA"] = "";
            csv["POŁAĆ 2 ŚWIETLIKI"] = "";
            csv["POŁAĆ 2 KOMINY"] = "";
            csv["POŁAĆ 2 INSTALACJA POD"] = "";
            csv["POŁAĆ 2 INST.ODGROM"] = "";
            csv["ELEW - WYS."] = "";
            csv["ELEW - SZER."] = "";
            csv["ELEW - AZYMUT"] = "";
            csv["ELEW - POW."] = "";
            csv["ANULOWANA - KOMENTARZ"] = "";
            csv["ANULOWANA - POWÓD"] = "";
            csv["OST.ZM. - DATA"] = "";
            csv["OST.ZM. - PRZEZ"] = "";
            csv["UWAGI"] = "";
            csv["ZAPLACONA"] = "";

            csv["Picture0"] = "";
            csv["Picture1"] = "";
            csv["Picture2"] = "";
            csv["Picture3"] = "";
            csv["Picture4"] = "";
            csv["Picture5"] = "";
            csv["Picture6"] = "";
            csv["Picture7"] = "";
            csv["Picture8"] = "";
            csv["Picture9"] = "";
        }
        private async Task<CsvExport> SurveyListAsCSV(List<Survey> data)
        {
            CsvExport myExport = new CsvExport(columnSeparator: ";");
            
            foreach (var srv in data)
            {
                //HEADER
                this.AddRow(ref myExport);

                //DATA
                myExport["ID INWESTYCJI"] = srv.InvestmentId.ToString();
                myExport["ID ANKIETY"] = srv.SurveyId.ToString();

                //SURVEY GENERAL
                myExport["TYP OZE"] = srv.TypeFullDescription();
                myExport["STATUS ANKIETY"] = srv.Status.DisplayName();

                //INWESTYCJA
                myExport["INWEST - ADRES - WOJ."] = srv.Investment.Address.State.Text;
                myExport["INWEST - ADRES - POW."] = srv.Investment.Address.District.Text;
                myExport["INWEST - ADRES - GM."] = srv.Investment.Address.Commune.FullName;
                myExport["INWEST - ADRES - KOD"] = srv.Investment.Address.PostalCode;
                myExport["INWEST - ADRES - MIEJSC"] = srv.Investment.Address.City;
                myExport["INWEST - ADRES - ULICA"] = srv.Investment.Address.Street;
                myExport["INWEST - ADRES - NR BUD."] = srv.Investment.Address.BuildingNo;
                myExport["INWEST - ADRES - NR MIESZK"] = srv.Investment.Address.ApartmentNo;
                
                //WŁAŚCICIELE
                for (int i = 0; i < 3; i++)
                {
                    if (srv.Investment.InvestmentOwners != null & srv.Investment.InvestmentOwners.Count >= (i + 1))
                    {
                        var owner = srv.Investment.InvestmentOwners.ElementAt(i).Owner;
                        myExport["WŁAŚCICIEL " + i.ToString()] = owner.PartnerName2 + " " + srv.Investment.InvestmentOwners.ElementAt(i).Owner.PartnerName1;
                        myExport["WŁAŚCICIEL ADRES " + i.ToString()] = owner.Address.SingleLine;
                        myExport["WŁAŚCICIEL TEL " + i.ToString()] = owner.PhoneNumber;
                        myExport["WŁAŚCICIEL MAIL " + i.ToString()] = owner.Email;
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
                    myExport["TYP INST.CHŁODZ."] = srv.AirCondition.Type.DisplayName();
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

                //BATHROOM
                if (srv.BathRoom != null)
                {
                    myExport["ISTN. WANNA"] = srv.BathRoom.BathExsists.AsYesNo();
                    myExport["OBJ. WANNY"] = srv.BathRoom.BathVolume.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                    myExport["L.ŁAZIENEK"] = srv.BathRoom.NumberOfBathrooms.ToString();
                    myExport["ISTN.PRYSZNIC"] = srv.BathRoom.ShowerExists.AsYesNo();
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

                //BUILDING
                if (srv.Building != null)
                {
                    myExport["IZOLACJA - GRUBOŚĆ"] = srv.Building.InsulationThickness.ToString();
                    myExport["IZOLACJA - RODZAJ"] = srv.Building.InsulationType == InsulationType.Ins_3 ?
                                   srv.Building.InsulationTypeOther != null ?
                                   srv.Building.InsulationTypeOther.ToString() : "" :
                                   srv.Building.InsulationType.DisplayName();
                    myExport["TECHNOLOGIA WYKONANIA"] = srv.Building.TechnologyType.DisplayName();
                    myExport["KUBATURA BUD."] = srv.Building.Volume.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                    myExport["MATERIAŁ ŚCIAN"] = srv.Building.WallMaterialOther != null ? srv.Building.WallMaterialOther.ToString() : "";
                    myExport["GRUBOŚĆ ŚCIAN"] = srv.Building.WallThickness.ToString();
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

                // PLANNED INSTALLATION
                if (srv.PlannedInstall != null)
                {
                    myExport["ZESTAW KLIENTA"] = srv.PlannedInstall.Configuration.DisplayName();
                    myExport["LOKALIZACJA INSTALACJI"] = srv.PlannedInstall.Localization.DisplayName();
                    myExport["INSTALACJA NA SCIANIE"] = srv.PlannedInstall.OnWallPlacementAvailable.AsYesNo();
                    myExport["PRZEZN. BUDYNKU"] = srv.PlannedInstall.Purpose.DisplayName();
                }

                //myExport[""] = srv.RejectComments.ToString();

                for (int i = 0; i < 3; i++)
                {
                    if (srv.RoofPlanes != null && srv.RoofPlanes.Count >= (i + 1))
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
                }

                //WALL
                if (srv.Wall != null)
                {
                    myExport["ELEW - AZYMUT"] = srv.Wall.Azimuth.ToString();
                    myExport["ELEW - WYS."] = srv.Wall.Height.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                    myExport["ELEW - SZER."] = srv.Wall.Width.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                    myExport["ELEW - POW."] = srv.Wall.UsableArea.ToString(System.Globalization.CultureInfo.GetCultureInfo("pl-PL").NumberFormat);
                }

                //ZDJECIA
                foreach (var item in this.SurveyPhotos(srv.SurveyId, srv.InvestmentId))
                {
                    myExport[item.Key] =
                        "=HYPERLINK(\"" + item.Value + "\";" + "\"" + item.Key + "\"" + ")";
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

        private Dictionary<string,string> SurveyPhotos(Guid surveyId, Guid investmentId)
        {
            CloudBlobContainer Container = _fileStorage.GetBlobContainer(enuAzureStorageContainerType.Survey);
            var SurveyBlockBlobs = Container.ListBlobs(prefix: surveyId.ToString(), useFlatBlobListing: true).OfType<CloudBlockBlob>();

            Dictionary<string, string> FileList = new Dictionary<string, string>();

            foreach (var BlockBlob in SurveyBlockBlobs)
            {
                try
                {
                    FileList.Add(BlockBlob.Name.Split('/').Reverse().ToArray().ElementAt(1), BlockBlob.Uri.ToString());
                }
                catch (Exception) { }
            };

            //
            CloudBlobContainer ContainerInv = _fileStorage.GetBlobContainer(enuAzureStorageContainerType.Investment);
            var InvestmentBlockBlobs = ContainerInv.ListBlobs(prefix: investmentId.ToString(), useFlatBlobListing: true).OfType<CloudBlockBlob>();

            foreach (var BlockBlob in InvestmentBlockBlobs)
            {
                var partNames = BlockBlob.Name.Split('/').Reverse().ToArray();
                if (partNames[1].Contains("Picture"))
                {
                    try
                    {
                        FileList.Add(partNames[1], BlockBlob.Uri.ToString());
                    }
                    catch (Exception){ }
                }
            };

            return FileList;
    }
}
}