﻿using System;
using System.Collections.Generic;
using DoEko.Models.DoEko.Survey;

namespace DoEko.Controllers.Helpers
{
    public enum SurveyViewMode
    {
        Maintain,
        Display
    }

    public enum SurveyFormStep
    {
        InvestmentGeneralInfo = 10,
        InvestmentOwnerData = 11,
        SurveyBuildingGeneralInfo = 21,
        SurveyPlannedInstallation = 22,
        SurveyBoilerRoom = 23,
        SurveyBathroom = 24,
        SurveyAirCondition = 25,
        SurveyGround = 31,
        SurveyWall = 32,
        SurveyRoofPlane = 33,
        SurveyAuditCH = 41,
        SurveyAuditHW = 42,
        SurveyAuditEN = 43,
        SurveySummary = 51,
        SurveyPhoto = 52,
    }

    public static class SurveyFormHelper
    {

        public static Dictionary<SurveyType, Dictionary<Enum, LinkedList<SurveyFormStep>>> Navigation;

        static SurveyFormHelper()
        {            
            Navigation = new Dictionary<SurveyType, Dictionary<Enum, LinkedList<SurveyFormStep>>>();

            Navigation.Add(SurveyType.CentralHeating, RSECentralHeating);
            Navigation.Add(SurveyType.HotWater, RSEHotWater);
            Navigation.Add(SurveyType.Energy, RSEEnergy);

        }

        private static Dictionary<Enum, LinkedList<SurveyFormStep>> RSEEnergy
        {
            get {
                var dict = new Dictionary<Enum, LinkedList<SurveyFormStep>>();

                dict.Add(SurveyRSETypeEnergy.PhotoVoltaic, Photovoltaic);

                return dict;
            }
        }
        private static Dictionary<Enum, LinkedList<SurveyFormStep>> RSEHotWater
        {
            get
            {
                var dict = new Dictionary<Enum, LinkedList<SurveyFormStep>>();

                dict.Add(SurveyRSETypeHotWater.HeatPump, HeatPump);
                dict.Add(SurveyRSETypeHotWater.Solar, Solar);

                return dict;
            }
        }
        private static Dictionary<Enum, LinkedList<SurveyFormStep>> RSECentralHeating
        {
            get
            {
                var dict = new Dictionary<Enum, LinkedList<SurveyFormStep>>();

                dict.Add(SurveyRSETypeCentralHeating.HeatPump, HeatPumpGround);
                dict.Add(SurveyRSETypeCentralHeating.HeatPumpAir, HeatPumpAir);
                dict.Add(SurveyRSETypeCentralHeating.PelletBoiler, PelletBoiler);

                return dict;
            }
        }

        private static LinkedList<SurveyFormStep> Photovoltaic
        {
            get {
                var FormSteps = new LinkedList<SurveyFormStep>();
                FormSteps.AddLast(SurveyFormStep.InvestmentGeneralInfo);
                FormSteps.AddLast(SurveyFormStep.InvestmentOwnerData);
                FormSteps.AddLast(SurveyFormStep.SurveyBuildingGeneralInfo);
                FormSteps.AddLast(SurveyFormStep.SurveyPlannedInstallation);
                FormSteps.AddLast(SurveyFormStep.SurveyGround);
                FormSteps.AddLast(SurveyFormStep.SurveyWall);
                FormSteps.AddLast(SurveyFormStep.SurveyRoofPlane);
                FormSteps.AddLast(SurveyFormStep.SurveyAuditCH);
                FormSteps.AddLast(SurveyFormStep.SurveyAuditHW);
                FormSteps.AddLast(SurveyFormStep.SurveyAuditEN);
                FormSteps.AddLast(SurveyFormStep.SurveyPhoto);
                FormSteps.AddLast(SurveyFormStep.SurveySummary);
                return FormSteps;
            }
        }
        private static LinkedList<SurveyFormStep> HeatPump
        {
            get
            {
                var FormSteps = new LinkedList<SurveyFormStep>();
                FormSteps.AddLast(SurveyFormStep.InvestmentGeneralInfo);
                FormSteps.AddLast(SurveyFormStep.InvestmentOwnerData);
                FormSteps.AddLast(SurveyFormStep.SurveyBuildingGeneralInfo);
                FormSteps.AddLast(SurveyFormStep.SurveyBoilerRoom);
                FormSteps.AddLast(SurveyFormStep.SurveyBathroom);
                FormSteps.AddLast(SurveyFormStep.SurveyAuditCH);
                FormSteps.AddLast(SurveyFormStep.SurveyAuditHW);
                FormSteps.AddLast(SurveyFormStep.SurveyPhoto);
                FormSteps.AddLast(SurveyFormStep.SurveySummary);
                return FormSteps;
            }
        }
        private static LinkedList<SurveyFormStep> Solar
        {
            get
            {
                var FormSteps = new LinkedList<SurveyFormStep>();
                FormSteps.AddLast(SurveyFormStep.InvestmentGeneralInfo);
                FormSteps.AddLast(SurveyFormStep.InvestmentOwnerData);
                FormSteps.AddLast(SurveyFormStep.SurveyBuildingGeneralInfo);
                FormSteps.AddLast(SurveyFormStep.SurveyPlannedInstallation);
                FormSteps.AddLast(SurveyFormStep.SurveyBoilerRoom);
                FormSteps.AddLast(SurveyFormStep.SurveyBathroom);
                FormSteps.AddLast(SurveyFormStep.SurveyGround);
                FormSteps.AddLast(SurveyFormStep.SurveyWall);
                FormSteps.AddLast(SurveyFormStep.SurveyRoofPlane);
                FormSteps.AddLast(SurveyFormStep.SurveyAuditCH);
                FormSteps.AddLast(SurveyFormStep.SurveyAuditHW);
                FormSteps.AddLast(SurveyFormStep.SurveyPhoto);
                FormSteps.AddLast(SurveyFormStep.SurveySummary);
                return FormSteps;
            }
        }
        private static LinkedList<SurveyFormStep> HeatPumpGround
        {
            get
            {
                var FormSteps = new LinkedList<SurveyFormStep>();
                FormSteps.AddLast(SurveyFormStep.InvestmentGeneralInfo);
                FormSteps.AddLast(SurveyFormStep.InvestmentOwnerData);
                FormSteps.AddLast(SurveyFormStep.SurveyBuildingGeneralInfo);
                FormSteps.AddLast(SurveyFormStep.SurveyBoilerRoom);
                FormSteps.AddLast(SurveyFormStep.SurveyBathroom);
                FormSteps.AddLast(SurveyFormStep.SurveyAirCondition);
                FormSteps.AddLast(SurveyFormStep.SurveyAuditCH);
                FormSteps.AddLast(SurveyFormStep.SurveyAuditHW);
                FormSteps.AddLast(SurveyFormStep.SurveyAuditEN);
                FormSteps.AddLast(SurveyFormStep.SurveyPhoto);
                FormSteps.AddLast(SurveyFormStep.SurveySummary);
                return FormSteps;
            }
        }
        private static LinkedList<SurveyFormStep> HeatPumpAir
        {
            get
            {
                var FormSteps = new LinkedList<SurveyFormStep>();
                FormSteps.AddLast(SurveyFormStep.InvestmentGeneralInfo);
                FormSteps.AddLast(SurveyFormStep.InvestmentOwnerData);
                FormSteps.AddLast(SurveyFormStep.SurveyBuildingGeneralInfo);
                FormSteps.AddLast(SurveyFormStep.SurveyPlannedInstallation);
                FormSteps.AddLast(SurveyFormStep.SurveyBoilerRoom);
                FormSteps.AddLast(SurveyFormStep.SurveyBathroom);
                FormSteps.AddLast(SurveyFormStep.SurveyAirCondition);
                FormSteps.AddLast(SurveyFormStep.SurveyAuditCH);
                FormSteps.AddLast(SurveyFormStep.SurveyAuditHW);
                FormSteps.AddLast(SurveyFormStep.SurveyAuditEN);
                FormSteps.AddLast(SurveyFormStep.SurveyPhoto);
                FormSteps.AddLast(SurveyFormStep.SurveySummary);
                return FormSteps;
            }
        }
        private static LinkedList<SurveyFormStep> PelletBoiler
        {
            get
            {
                var FormSteps = new LinkedList<SurveyFormStep>();
                FormSteps.AddLast(SurveyFormStep.InvestmentGeneralInfo);
                FormSteps.AddLast(SurveyFormStep.InvestmentOwnerData);
                FormSteps.AddLast(SurveyFormStep.SurveyBuildingGeneralInfo);
                FormSteps.AddLast(SurveyFormStep.SurveyBoilerRoom);
                FormSteps.AddLast(SurveyFormStep.SurveyBathroom);
                FormSteps.AddLast(SurveyFormStep.SurveyAirCondition);
                FormSteps.AddLast(SurveyFormStep.SurveyAuditCH);
                FormSteps.AddLast(SurveyFormStep.SurveyAuditHW);
                FormSteps.AddLast(SurveyFormStep.SurveyPhoto);
                FormSteps.AddLast(SurveyFormStep.SurveySummary);
                return FormSteps;
            }
        }
    }
}
