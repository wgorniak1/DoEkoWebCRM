using DoEko.Controllers.Extensions;
using DoEko.Models.DoEko.Survey;
using DoEko.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.ViewModels.API.SurveyViewModels
{
    public class SurveyNeoVM
    {
        public SurveyNeoVM(Survey srv, IFileStorage fileStorage)
        {
            InvestmentId  = srv.InvestmentId;
            PriorityIndex = srv.Investment.PriorityIndex;
            SurveyId      = srv.SurveyId;
            TypeFullDescription = srv.TypeFullDescription();
            Status = srv.Status.DisplayName();
            
            //ADRES INW.
            State = srv.Investment.Address.State.Text;
            District = srv.Investment.Address.District.Text;
            Commune = srv.Investment.Address.Commune.FullName;
            PostalCode = srv.Investment.Address.PostalCode;
            City = srv.Investment.Address.City;
            Street = srv.Investment.Address.Street;
            BuildingNo = srv.Investment.Address.BuildingNo;
            ApartmentNo = srv.Investment.Address.ApartmentNo;

            //WŁAŚCICIELE
            int i = 0;
            var me = this.GetType();
            foreach (var io in srv.Investment.InvestmentOwners)
            {
                var index = i > 0 ? i.ToString() : "";
                var bp = io.Owner;
                me.GetProperty("Owner" + index).SetValue(this, bp.PartnerName2 + ' ' + bp.PartnerName1);
                me.GetProperty("OwnerAddress" + index).SetValue(this, bp.Address.SingleLine);
                me.GetProperty("OwnerPhone" + index).SetValue(this, bp.PhoneNumber);
                me.GetProperty("OwnerEmail" + index).SetValue(this, bp.Email);
            }                    

            BusinessActivity = srv.Investment.BusinessActivity.DisplayName();
            CentralHeatingFuel = srv.Investment.CentralHeatingFuel.DisplayName();
            CentralHeatingType = srv.Investment.CentralHeatingType != DoEko.Models.DoEko.CentralHeatingType.Other ?
                                srv.Investment.CentralHeatingType.DisplayName() :
                                srv.Investment.CentralHeatingTypeOther;
            ConstructionYear = srv.Investment.CompletionYear;
            HeatedArea = srv.Investment.HeatedArea;
            HotWaterFuel = srv.Investment.HotWaterFuel.DisplayName();
            HotWaterType = srv.Investment.HotWaterType.DisplayName();
            InternetAvailable = srv.Investment.InternetAvailable.AsYesNo();
            LandRegisterNo = srv.Investment.LandRegisterNo;
            NumberOfOccupants = srv.Investment.NumberOfOccupants;
            PlotNumber = srv.Investment.PlotNumber;
            BuildingStage =srv.Investment.Stage.DisplayName();
            TotalArea = srv.Investment.TotalArea;
            BuildingType  = srv.Investment.Type.DisplayName();
            UsableArea = srv.Investment.UsableArea;

            if (srv.Audit != null)
            {
                AdditionalHeatParams = srv.Audit.AdditionalHeatParams;
                AdditionalHeatSource = srv.Audit.AdditionalHeatSource.AsYesNo();
                AverageYearlyFuelConsumption = srv.Audit.AverageYearlyFuelConsumption;
                AverageYearlyHeatingCosts = srv.Audit.AverageYearlyHeatingCosts;
                BoilerMaxTemp =  srv.Audit.BoilerMaxTemp;
                BoilerNominalPower = srv.Audit.BoilerNominalPower;
                BoilerPlannedReplacement = srv.Audit.BoilerPlannedReplacement.AsYesNo();
                BoilerProductionYear = srv.Audit.BoilerProductionYear;
                CHFRadiantFloorInstalled = srv.Audit.CHFRadiantFloorInstalled.AsYesNo();
                CHIsHPOnlySource = srv.Audit.CHIsHPOnlySource.AsYesNo();
                CHRadiantFloorAreaPerc = srv.Audit.CHRadiantFloorAreaPerc.ToString();
                CHRadiatorsInstalled = srv.Audit.CHRadiatorsInstalled.AsYesNo();
                CHRadiatorType = srv.Audit.CHRadiatorType.DisplayName();
                EEComplexAgreement = srv.Audit.ComplexAgreement.AsYesNo();
                EEAvgMonthlyCost = srv.Audit.ElectricityAvgMonthlyCost;
                EEElectricityPower  = srv.Audit.ElectricityPower;
                EEAdditionalMeter = srv.Audit.ENAdditionalConsMeter.AsYesNo();
                EEIsGround  = srv.Audit.ENIsGround.AsYesNo();
                EEPlannedPower  = srv.Audit.ENPowerLevel;
                HWSourcePower = srv.Audit.HWSourcePower;
                EEPhaseCount  = srv.Audit.PhaseCount.DisplayName();
                EEYearlyConsumption = srv.Audit.PowerAvgYearlyConsumption;
                EEPowerCompanyName = srv.Audit.PowerCompanyName.DisplayName();
                EEMeterLocation  = srv.Audit.PowerConsMeterLocation.DisplayName();
                EEPowerSupplyType  = srv.Audit.PowerSupplyType.DisplayName();
                TankCoilSize = srv.Audit.TankCoilSize;
                TankExists = srv.Audit.TankExists.AsYesNo();
                TankVolume = srv.Audit.TankVolume;
            }
            //BOILERROOM
            if (srv.BoilerRoom != null)
            {
                BoilerRoomExists  = srv.BoilerRoom.RoomExists.AsYesNo();
                BoilerRoomAirVentilationExists  = srv.BoilerRoom.AirVentilationExists.AsYesNo();
                BoilerRoomDoorHeight  = srv.BoilerRoom.DoorHeight;
                BoilerRoomHeight = srv.BoilerRoom.Height;
                BoilerRoomHighVoltagePowerSupply  = srv.BoilerRoom.HighVoltagePowerSupply.AsYesNo();
                BoilerRoomHWCirculationInstalled  = srv.BoilerRoom.HWCirculationInstalled.AsYesNo();
                BoilerRoomHWInstalled  = srv.BoilerRoom.HWInstalled.AsYesNo();
                BoilerRoomHWPressureReductorExists  = srv.BoilerRoom.HWPressureReductorExists.AsYesNo();
                BoilerRoomIsDryAndWarm  = srv.BoilerRoom.IsDryAndWarm.AsYesNo();
                BoilerRoomLength = srv.BoilerRoom.Length;
                BoilerRoomThreePowerSuppliesExists  = srv.BoilerRoom.ThreePowerSuppliesExists.AsYesNo();
                BoilerRoomVolume = srv.BoilerRoom.Volume;
                BoilerRoomWidth  = srv.BoilerRoom.Width;
            }
            //BATHROOM
            if (srv.BathRoom != null)
            {
                BathRoomBathExists  = srv.BathRoom.BathExsists.AsYesNo();
                BathRoomBathVolume = srv.BathRoom.BathVolume;
                BathRoomNumberOfBathrooms = srv.BathRoom.NumberOfBathrooms;
                BathRoomShowerExists  = srv.BathRoom.ShowerExists.AsYesNo();
            }
            //BUILDING
            if (srv.Building != null)
            {
                InsulationThickness = srv.Building.InsulationThickness;
                InsulationType = srv.Building.InsulationType == DoEko.Models.DoEko.Survey.InsulationType.Ins_3 ?
                               srv.Building.InsulationTypeOther != null ?
                               srv.Building.InsulationTypeOther.ToString() : "" :
                               srv.Building.InsulationType.DisplayName();
                BuildingTechnologyType = srv.Building.TechnologyType.DisplayName();
                BuildingVolume = srv.Building.Volume;
                WallMaterial = srv.Building.WallMaterialOther != null ? srv.Building.WallMaterialOther.ToString() : "";
                WallThickness = srv.Building.WallThickness;
            }
            if (srv.AirCondition != null)
            {
                AirConditionExists = srv.AirCondition.Exists.AsYesNo();
                AirConditionIsPlanned = srv.AirCondition.isPlanned.AsYesNo();
                BoilerRoomAirVentilationExists = srv.AirCondition.MechVentilationExists.AsYesNo();
                AirConditionType = srv.AirCondition.Type.DisplayName();
            }
            // PLANNED INSTALLATION
            if (srv.PlannedInstall != null)
            {
                PlannedInstallConfiguration = srv.PlannedInstall.Configuration.DisplayName();
                Localization = srv.PlannedInstall.Localization.DisplayName();
                OnWallPlacementAvailable = srv.PlannedInstall.OnWallPlacementAvailable.AsYesNo();
                Purpose = srv.PlannedInstall.Purpose.DisplayName();
            }
            //GROUND
            if (srv.Ground != null)
            {
                GroundArea = srv.Ground.Area;
                GroundFormerMilitary = srv.Ground.FormerMilitary.AsYesNo();
                GroundOtherInstallation = srv.Ground.OtherInstallation.AsYesNo();
                GroundOtherInstallationType = srv.Ground.OtherInstallationType != null ? srv.Ground.OtherInstallationType : "";
                GroundRocks = srv.Ground.Rocks.AsYesNo();
                GroundSlopeTerrain = srv.Ground.SlopeTerrain.DisplayName();
                GroundWetLand = srv.Ground.WetLand.AsYesNo();
            }        
            //ROOF
            i = 0;
            foreach (var roof in srv.RoofPlanes)
            {
                var index = i > 0 ? i.ToString() : "";
                me.GetProperty("RoofType" + index).SetValue(this, roof.Type.DisplayName());
                me.GetProperty("RoofBuildingHeight" + index).SetValue(this,  roof.BuildingHeight);
                me.GetProperty("RoofChimney" + index).SetValue(this,  roof.Chimney.AsYesNo());
                me.GetProperty("RoofEdgeLength" + index).SetValue(this,   roof.EdgeLength);
                me.GetProperty("RoofInstallationUnderPlane" + index).SetValue(this,  roof.InstallationUnderPlane.AsYesNo());
                me.GetProperty("RoofWidth" + index).SetValue(this,   roof.Width);
                me.GetProperty("RoofLength" + index).SetValue(this,  roof.Length);
                me.GetProperty("RoofLightingProtection" + index).SetValue(this,  roof.LightingProtection.AsYesNo());
                me.GetProperty("RoofOkapHeight" + index).SetValue(this,   roof.OkapHeight);
                me.GetProperty("RoofRidgeWeight" + index).SetValue(this,  roof.RidgeWeight);
                me.GetProperty("RoofRoofLength" + index).SetValue(this,  roof.RoofLength);
                me.GetProperty("RoofRoofMaterial" + index).SetValue(this,  roof.RoofMaterial.DisplayName());
                me.GetProperty("RoofSkyLights" + index).SetValue(this,   roof.SkyLights.AsYesNo());
                me.GetProperty("RoofSlopeAngle" + index).SetValue(this,  roof.SlopeAngle);
                me.GetProperty("RoofSurfaceArea" + index).SetValue(this,  roof.SurfaceArea);
                me.GetProperty("RoofSurfaceAzimuth" + index).SetValue(this,  roof.SurfaceAzimuth);
                me.GetProperty("RoofWindows" + index).SetValue(this, roof.Windows.AsYesNo());
                i++;
            }

            //WALL
            if (srv.Wall != null)
            {
                WallAzimuth = srv.Wall.Azimuth;
                WallHeight = srv.Wall.Height;
                WallWidth = srv.Wall.Width;
                WallUsableArea = srv.Wall.UsableArea;
            }

            CancelComments = srv.CancelComments != null ? srv.CancelComments.ToString() : "";
            CancelType = srv.CancelType.HasValue? srv.CancelType.DisplayName() : "";
            ChangedAt = srv.ChangedAt.ToLocalTime();
            FreeCommments = srv.FreeCommments != null ? srv.FreeCommments.ToString() : "";

            //ZDJECIA
            foreach (var item in srv.Photos(fileStorage))
            {
                me.GetProperty(item.Key).SetValue(this, item.Value);
            }

        }

        [Display(Name = "ID INWESTYCJI")]
        public Guid InvestmentId { get; set; }
        [Display(Name = "PRIORYTET")]
        public long PriorityIndex { get; set; }
        [Display(Name = "ID ANKIETY")]
        public Guid SurveyId { get; set; }
        [Display(Name = "TYP OZE")]
        public string TypeFullDescription { get; set; }
        [Display(Name = "STATUS ANKIETY")]
        public string Status { get; set; }
        [Display(Name = "INWEST - ADRES - WOJ.")]
        public string State { get; set; }
        [Display(Name = "INWEST - ADRES - POW.")]
        public string District { get; set; }
        [Display(Name = "INWEST - ADRES - GM.")]
        public string Commune { get; set; }
        [Display(Name = "INWEST - ADRES - KOD")]
        public string PostalCode { get; set; }
        [Display(Name = "INWEST - ADRES - MIEJSC")]
        public string City { get; set; }
        [Display(Name = "INWEST - ADRES - ULICA")]
        public string Street { get; set; }
        [Display(Name = "INWEST - ADRES - NR BUD.")]
        public string BuildingNo { get; set; }
        [Display(Name = "INWEST - ADRES - NR MIESZK")]
        public string ApartmentNo { get; set; }
        [Display(Name = "WŁAŚCICIEL 0")]
        public string Owner { get; set; }
        [Display(Name = "WŁAŚCICIEL ADRES 0")]
        public string OwnerAddress { get; set; }
        [Display(Name = "WŁAŚCICIEL TEL 0")]
        public string OwnerPhone { get; set; }
        [Display(Name = "WŁAŚCICIEL MAIL 0")]
        public string OwnerEmail { get; set; }
        [Display(Name = "WŁAŚCICIEL 1")]
        public string Owner2 { get; set; }
        [Display(Name = "WŁAŚCICIEL ADRES 1")]
        public string OwnerAddress2 { get; set; }
        [Display(Name = "WŁAŚCICIEL TEL 1")]
        public string OwnerPhone2 { get; set; }
        [Display(Name = "WŁAŚCICIEL MAIL 1")]
        public string OwnerMail2 { get; set; }
        [Display(Name = "WŁAŚCICIEL 2")]
        public string Owner3 { get; set; }
        [Display(Name = "WŁAŚCICIEL ADRES 2")]
        public string OwnerAddress3 { get; set; }
        [Display(Name = "WŁAŚCICIEL TEL 2")]
        public string OwnerPhone3 { get; set; }
        [Display(Name = "WŁAŚCICIEL MAIL 2")]
        public string OwnerMail3 { get; set; }
        [Display(Name = "INTERNET W M.INW.")]
        public string InternetAvailable { get; set; }
        [Display(Name = "RODZ. DZIAŁALN.")]
        public string BusinessActivity { get; set; }
        [Display(Name = "NR KS. WIECZ.")]
        public string LandRegisterNo { get; set; }
        [Display(Name = "NR DZIAŁKI")]
        public string PlotNumber { get; set; }
        [Display(Name = "PALIWO GŁ.CO")]
        public string CentralHeatingFuel { get; set; }
        [Display(Name = "RODZAJ GŁ.CO")]
        public string CentralHeatingType { get; set; }
        [Display(Name = "PALIWO GŁ. CW")]
        public string HotWaterFuel { get; set; }
        [Display(Name = "RODZAJ GŁ. CW")]
        public string HotWaterType { get; set; }
        [Display(Name = "STAN BUD.")]
        public string BuildingStage { get; set; }
        [Display(Name = "ROK BUDOWY")]
        public short ConstructionYear { get; set; }
        [Display(Name = "L.MIESZKAŃCÓW")]
        public short NumberOfOccupants { get; set; }
        [Display(Name = "POW. UŻYTK.")]
        public double UsableArea { get; set; }
        [Display(Name = "POW. OGRZEWANA")]
        public double HeatedArea { get; set; }
        [Display(Name = "POW. CAŁK.")]
        public double TotalArea { get; set; }
        [Display(Name = "RODZ.BUD.")]
        public string BuildingType { get; set; }
        [Display(Name = "UMOWA KOMPLEKS.")]
        public string EEComplexAgreement { get; set; }
        [Display(Name = "EE - DYSTRYBUTOR")]
        public string EEPowerCompanyName { get; set; }
        [Display(Name = "EE - MOC PRZYŁ.")]
        public double EEElectricityPower { get; set; }
        [Display(Name = "EE - RODZ. PRZYŁ.")]
        public string EEPowerSupplyType { get; set; }
        [Display(Name = "EE - L. FAZ")]
        public string EEPhaseCount { get; set; }
        [Display(Name = "EE - UZIEMIENIE")]
        public string EEIsGround { get; set; }
        [Display(Name = "EE - UMIEJSC. LICZNIKA")]
        public string EEMeterLocation { get; set; }
        [Display(Name = "EE - DOD. LICZNIK")]
        public string EEAdditionalMeter { get; set; }
        [Display(Name = "EE - ROCZNE ZUŻYCIE")]
        public double EEYearlyConsumption { get; set; }
        [Display(Name = "EE - ŚR. KOSZT/MC.")]
        public decimal EEAvgMonthlyCost { get; set; }
        [Display(Name = "EE - PLANOWANA MOC")]
        public double EEPlannedPower { get; set; }
        [Display(Name = "KOTŁOWNIA - ISTNIEJE")]
        public string BoilerRoomExists { get; set; }
        [Display(Name = "KOTŁOWNIA - SZER.DRZWI")]
        public double BoilerRoomDoorHeight { get; set; }
        [Display(Name = "KOTŁOWNIA - DŁUG.")]
        public double BoilerRoomLength { get; set; }
        [Display(Name = "KOTŁOWNIA - SZER.")]
        public double BoilerRoomWidth { get; set; }
        [Display(Name = "KOTŁOWNIA - WYS.")]
        public double BoilerRoomHeight { get; set; }
        [Display(Name = "KOTŁOWNIA - KUBATURA")]
        public double BoilerRoomVolume { get; set; }
        [Display(Name = "KOTŁOWNIA - ISTN. INST. CW")]
        public string BoilerRoomHWInstalled { get; set; }
        [Display(Name = "KOTŁOWNIA - ISTN. CYRKULACJA")]
        public string BoilerRoomHWCirculationInstalled { get; set; }
        [Display(Name = "KOTŁOWNIA - ISTN. REDUKTOR C.")]
        public string BoilerRoomHWPressureReductorExists { get; set; }
        [Display(Name = "KOTŁOWNIA - ISTN.WOLNY PRZEW.WENT")]
        public string BoilerRoomAirVentilationExists  { get; set; }
        [Display(Name = "KOTŁOWNIA - SUCHA I > 0 ST.")]
        public string BoilerRoomIsDryAndWarm { get; set; }
        [Display(Name = "KOTŁOWNIA - ISTN. 3 UZIEM.GNIAZDA")]
        public string BoilerRoomThreePowerSuppliesExists { get; set; }
        [Display(Name = "KOTŁOWNIA - INST.400V")]
        public string BoilerRoomHighVoltagePowerSupply { get; set; }
        [Display(Name = "L.ŁAZIENEK")]
        public short BathRoomNumberOfBathrooms { get; set; }
        [Display(Name = "ISTN. WANNA")]
        public string BathRoomBathExists { get; set; }
        [Display(Name = "OBJ. WANNY")]
        public double BathRoomBathVolume { get; set; }
        [Display(Name = "ISTN.PRYSZNIC")]
        public string BathRoomShowerExists { get; set; }
        [Display(Name = "ZESTAW KLIENTA")]
        public string PlannedInstallConfiguration { get; set; }
        [Display(Name = "CW - MOC")]
        public double HWSourcePower { get; set; }
        [Display(Name = "PIEC - MOC")]
        public double BoilerNominalPower { get; set; }
        [Display(Name = "PIEC - ROK PROD.")]
        public short BoilerProductionYear { get; set; }
        [Display(Name = "PIEC - PLAN. WYM.")]
        public string BoilerPlannedReplacement { get; set; }
        [Display(Name = "CO GRZEJNIKI")]
        public string CHRadiatorsInstalled { get; set; }
        [Display(Name = "CO GRZEJNIKI - TYP")]
        public string CHRadiatorType { get; set; }
        [Display(Name = "CO PODLOG.")]
        public string CHFRadiantFloorInstalled { get; set; }
        [Display(Name = "CO PODLOG. - PROCENT POW.")]
        public string CHRadiantFloorAreaPerc { get; set; }
        [Display(Name = "MAX TEMP. PIECA")]
        public double BoilerMaxTemp { get; set; }
        [Display(Name = "ŚR.ROCZNE ZUŻYCIE CO")]
        public double AverageYearlyFuelConsumption { get; set; }
        [Display(Name = "ŚR.ROCZNE KOSZTY CO")]
        public decimal AverageYearlyHeatingCosts { get; set; }
        [Display(Name = "MECH.WENT.ISTN.")]
        public string MechVentilationExists { get; set; }
        [Display(Name = "ISTN. KLIMAT.")]
        public string AirConditionExists { get; set; }
        [Display(Name = "KLIMAT.PLANOWANA")]
        public string AirConditionIsPlanned { get; set; }
        [Display(Name = "TYP INST.CHŁODZ.")]
        public string AirConditionType { get; set; }
        [Display(Name = "DOD.ŹR.CIEPŁA")]
        public string AdditionalHeatSource { get; set; }
        [Display(Name = "PAR.DOD.ŹR.CIEPŁA")]
        public string AdditionalHeatParams { get; set; }
        [Display(Name = "CW - ISTN. ZASOBNIK")]
        public string TankExists { get; set; }
        [Display(Name = "CW - OBJ. ZASOBNIKA")]
        public double TankVolume { get; set; }
        [Display(Name = "CW - POW. WĘŻ.")]
        public double TankCoilSize { get; set; }
        [Display(Name = "PLAN.POMPA BĘDZIE JEDYNYM ŹR.")]
        public string CHIsHPOnlySource { get; set; }
        [Display(Name = "TECHNOLOGIA WYKONANIA")]
        public string BuildingTechnologyType { get; set; }
        [Display(Name = "MATERIAŁ ŚCIAN")]
        public string WallMaterial { get; set; }
        [Display(Name = "GRUBOŚĆ ŚCIAN")]
        public double WallThickness { get; set; }
        [Display(Name = "IZOLACJA - RODZAJ")]
        public string InsulationType { get; set; }
        [Display(Name = "IZOLACJA - GRUBOŚĆ")]
        public double InsulationThickness { get; set; }
        [Display(Name = "KUBATURA BUD.")]
        public double  BuildingVolume { get; set; }
        [Display(Name = "LOKALIZACJA INSTALACJI")]
        public string Localization { get; set; }
        [Display(Name = "INSTALACJA NA SCIANIE")]
        public string OnWallPlacementAvailable { get; set; }
        [Display(Name = "PRZEZN. BUDYNKU")]
        public string Purpose { get; set; }
        [Display(Name = "GRUNT - POW.")]
        public double GroundArea { get; set; }
        [Display(Name = "GRUNT - BYLY TEREN WOJSK")]
        public string GroundFormerMilitary { get; set; }
        [Display(Name = "GRUNT - ISTN.INSTALACJA")]
        public string GroundOtherInstallation { get; set; }
        [Display(Name = "GRUNT - INSTALACJA TYP")]
        public string GroundOtherInstallationType { get; set; }
        [Display(Name = "GRUNT - GRUZ,SKAŁY")]
        public string GroundRocks { get; set; }
        [Display(Name = "GRUNT - NACHYLENIE")]
        public string GroundSlopeTerrain { get; set; }
        [Display(Name = "GRUNT - PODMOKŁY")]
        public string GroundWetLand { get; set; }
        [Display(Name = "POŁAĆ 0 TYP")]
        public string RoofType { get; set; }
        [Display(Name = "POŁAĆ 0 WYS.BUD.")]
        public double RoofBuildingHeight { get; set; }
        [Display(Name = "POŁAĆ 0 WYS.OKAPU")]
        public double RoofOkapHeight { get; set; }
        [Display(Name = "POŁAĆ 0 DŁ. DACHU")]
        public double RoofRoofLength { get; set; }
        [Display(Name = "POŁAĆ 0 DŁ.KRAW.")]
        public double RoofEdgeLength { get; set; }
        [Display(Name = "POŁAĆ 0 DŁ.GRZBIETU")]
        public double RoofRidgeWeight { get; set; }
        [Display(Name = "POŁAĆ 0 KĄT NACH.")]
        public double RoofSlopeAngle { get; set; }
        [Display(Name = "POŁAĆ 0 DŁUG.")]
        public double RoofWidth { get; set; }
        [Display(Name = "POŁAĆ 0 SZER.")]
        public double RoofLength { get; set; }
        [Display(Name = "POŁAĆ 0 POWIERZCHNIA")]
        public double RoofSurfaceArea { get; set; }
        [Display(Name = "POŁAĆ 0 POKRYCIE")]
        public string RoofRoofMaterial { get; set; }
        [Display(Name = "POŁAĆ 0 AZYMUT")]
        public double RoofSurfaceAzimuth { get; set; }
        [Display(Name = "POŁAĆ 0 OKNA")]
        public string RoofWindows { get; set; }
        [Display(Name = "POŁAĆ 0 ŚWIETLIKI")]
        public string RoofSkyLights { get; set; }
        [Display(Name = "POŁAĆ 0 KOMINY")]
        public string RoofChimney { get; set; }
        [Display(Name = "POŁAĆ 0 INSTALACJA POD")]
        public string RoofInstallationUnderPlane { get; set; }
        [Display(Name = "POŁAĆ 0 INST.ODGROM")]
        public string RoofLightingProtection { get; set; }
        [Display(Name = "POŁAĆ 1 TYP")]
        public string RoofType1 { get; set; }
        [Display(Name = "POŁAĆ 1 WYS.BUD.")]
        public double RoofBuildingHeight1 { get; set; }
        [Display(Name = "POŁAĆ 1 WYS.OKAPU")]
        public double RoofOkapHeight1 { get; set; }
        [Display(Name = "POŁAĆ 1 DŁ. DACHU")]
        public double RoofRoofLength1 { get; set; }
        [Display(Name = "POŁAĆ 1 DŁ.KRAW.")]
        public double RoofEdgeLength1 { get; set; }
        [Display(Name = "POŁAĆ 1 DŁ.GRZBIETU")]
        public double RoofRidgeWeight1 { get; set; }
        [Display(Name = "POŁAĆ 1 KĄT NACH.")]
        public double RoofSlopeAngle1 { get; set; }
        [Display(Name = "POŁAĆ 1 DŁUG.")]
        public double RoofWidth1 { get; set; }
        [Display(Name = "POŁAĆ 1 SZER.")]
        public double RoofLength1 { get; set; }
        [Display(Name = "POŁAĆ 1 POWIERZCHNIA")]
        public double RoofSurfaceArea1 { get; set; }
        [Display(Name = "POŁAĆ 1 POKRYCIE")]
        public string RoofRoofMaterial1 { get; set; }
        [Display(Name = "POŁAĆ 1 AZYMUT")]
        public double RoofSurfaceAzimuth1 { get; set; }
        [Display(Name = "POŁAĆ 1 OKNA")]
        public string RoofWindows1 { get; set; }
        [Display(Name = "POŁAĆ 1 ŚWIETLIKI")]
        public string RoofSkyLights1 { get; set; }
        [Display(Name = "POŁAĆ 1 KOMINY")]
        public string RoofChimney1 { get; set; }
        [Display(Name = "POŁAĆ 1 INSTALACJA POD")]
        public string RoofInstallationUnderPlane1 { get; set; }
        [Display(Name = "POŁAĆ 1 INST.ODGROM")]
        public string RoofLightingProtection1 { get; set; }
        [Display(Name = "POŁAĆ 2 TYP")]
        public string RoofType2 { get; set; }
        [Display(Name = "POŁAĆ 2 WYS.BUD.")]
        public double RoofBuildingHeight2 { get; set; }
        [Display(Name = "POŁAĆ 2 WYS.OKAPU")]
        public double RoofOkapHeight2 { get; set; }
        [Display(Name = "POŁAĆ 2 DŁ. DACHU")]
        public double RoofRoofLength2 { get; set; }
        [Display(Name = "POŁAĆ 2 DŁ.KRAW.")]
        public double RoofEdgeLength2 { get; set; }
        [Display(Name = "POŁAĆ 2 DŁ.GRZBIETU")]
        public double RoofRidgeWeight2 { get; set; }
        [Display(Name = "POŁAĆ 2 KĄT NACH.")]
        public double RoofSlopeAngle2 { get; set; }
        [Display(Name = "POŁAĆ 2 DŁUG.")]
        public double RoofWidth2 { get; set; }
        [Display(Name = "POŁAĆ 2 SZER.")]
        public double RoofLength2 { get; set; }
        [Display(Name = "POŁAĆ 2 POWIERZCHNIA")]
        public double RoofSurfaceArea2 { get; set; }
        [Display(Name = "POŁAĆ 2 POKRYCIE")]
        public string RoofRoofMaterial2 { get; set; }
        [Display(Name = "POŁAĆ 2 AZYMUT")]
        public double RoofSurfaceAzimuth2 { get; set; }
        [Display(Name = "POŁAĆ 2 OKNA")]
        public string RoofWindows2 { get; set; }
        [Display(Name = "POŁAĆ 2 ŚWIETLIKI")]
        public string RoofSkyLights2 { get; set; }
        [Display(Name = "POŁAĆ 2 KOMINY")]
        public string RoofChimney2 { get; set; }
        [Display(Name = "POŁAĆ 2 INSTALACJA POD")]
        public string RoofInstallationUnderPlane2 { get; set; }
        [Display(Name = "POŁAĆ 2 INST.ODGROM")]
        public string RoofLightingProtection2 { get; set; }
        [Display(Name = "ELEW - WYS.")]
        public double WallHeight { get; set; }
        [Display(Name = "ELEW - SZER.")]
        public double WallWidth { get; set; }
        [Display(Name = "ELEW - AZYMUT")]
        public double WallAzimuth { get; set; }
        [Display(Name = "ELEW - POW.")]
        public double WallUsableArea { get; set; }
        [Display(Name = "ANULOWANA - KOMENTARZ")]
        public string CancelComments { get; set; }
        [Display(Name = "ANULOWANA - POWÓD")]
        public string CancelType { get; set; }
        [Display(Name = "OST.ZM. - DATA")]
        public DateTime ChangedAt { get; set; }
        [Display(Name = "UWAGI")]
        public string FreeCommments { get; set; }
        [Display(Name = "Picture0")]
        public Uri Picture0 { get; set; }
        [Display(Name = "Picture1")]
        public Uri Picture1 { get; set; }
        [Display(Name = "Picture2")]
        public Uri Picture2 { get; set; }
        [Display(Name = "Picture3")]
        public Uri Picture3 { get; set; }
        [Display(Name = "Picture4")]
        public Uri Picture4 { get; set; }
        [Display(Name = "Picture5")]
        public Uri Picture5 { get; set; }
        [Display(Name = "Picture6")]
        public Uri Picture6 { get; set; }
        [Display(Name = "Picture7")]
        public Uri Picture7 { get; set; }
        [Display(Name = "Picture8")]
        public Uri Picture8 { get; set; }
        [Display(Name = "Picture9")]
        public Uri Picture9 { get; set; }
        [Display(Name = "Picture10")]
        public Uri Picture10 { get; set; }
        [Display(Name = "Picture11")]
        public Uri Picture11 { get; set; }

    }
}
