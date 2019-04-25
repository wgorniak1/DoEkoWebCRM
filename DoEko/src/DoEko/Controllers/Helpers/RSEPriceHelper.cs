﻿using DoEko.Controllers.Extensions;
using DoEko.Models.DoEko;
using DoEko.Models.DoEko.Survey;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Controllers.Helpers
{
    public class RSEPriceHelper
    {
        private readonly bool _forceLoad;
        private static ICollection<RSEPriceTaxRule> _taxRules;
        private static ICollection<RSEPriceRule> _priceRules;
        private readonly DoEkoContext _context;
        private Survey _survey;
        private RSEPriceTaxRule _taxRuleCondition;
        private RSEPriceRule _priceRuleCondition;

        public RSEPriceHelper(DoEkoContext context, bool forceLoad = true, int projectId = 1)
        {
            _forceLoad = forceLoad;
            _context = context;

            _taxRules = RSEPriceTaxRuleHelper.GetTaxRules(context, projectId);
            _priceRules = RSEPriceRuleHelper.GetPriceRules(context, projectId);

            _taxRuleCondition = new RSEPriceTaxRule();
            _priceRuleCondition = new RSEPriceRule();

        }

        public Survey Survey
        {
            get { return _survey; }
            set
            {

                if (_survey is null)
                {
                    _survey = value;
                    if (_forceLoad)
                    {
                        try
                        {
                            _context.Entry(_survey).Reference(s => s.Investment).Load();
                            _context.Entry(_survey).Reference(s => s.PlannedInstall).Load();
                            _context.Entry(_survey.Investment).Reference(i => i.Contract).Load();
                            _context.Entry(_survey.Investment.Contract).Reference(c => c.Project).Load();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                else if (_survey != value)
                {
                    _survey = value;
                    if (_forceLoad)
                    {
                        try
                        {
                            _context.Entry(_survey).Reference(s => s.Investment).Load();
                            _context.Entry(_survey).Reference(s => s.PlannedInstall).Load();
                            _context.Entry(_survey.Investment).Reference(i => i.Contract).Load();
                            _context.Entry(_survey.Investment.Contract).Reference(c => c.Project).Load();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                else
                {
                    return;
                }

                //TAX CONDITIONS
                _taxRuleCondition.SurveyType = _survey.Type;
                _taxRuleCondition.RSEType = _survey.GetRSEType();
                _taxRuleCondition.UsableAreaMax = _survey.Investment.UsableArea;
                switch (_survey.Type)
                {
                    case SurveyType.CentralHeating:
                        switch (((SurveyCentralHeating)_survey).RSEType)
                        {
                            case SurveyRSETypeCentralHeating.HeatPump:
                                _taxRuleCondition.BuildingPurpose = _survey.PlannedInstall != null ? _survey.PlannedInstall.Purpose : BuildingPurpose.Housing;
                                _taxRuleCondition.InstallationLocalization = _survey.PlannedInstall != null ? _survey.PlannedInstall.Localization : InstallationLocalization.Ground;
                                break;
                            case SurveyRSETypeCentralHeating.PelletBoiler:
                                _taxRuleCondition.BuildingPurpose = BuildingPurpose.Housing;
                                _taxRuleCondition.InstallationLocalization = InstallationLocalization.Ground;
                                break;
                            case SurveyRSETypeCentralHeating.HeatPumpAir:
                                _taxRuleCondition.BuildingPurpose = BuildingPurpose.Housing;
                                _taxRuleCondition.InstallationLocalization = InstallationLocalization.Ground;
                                break;
                            default:
                                break;
                        }
                        break;
                    case SurveyType.HotWater:
                        switch (((SurveyHotWater)_survey).RSEType)
                        {
                            case SurveyRSETypeHotWater.Solar:
                                _taxRuleCondition.BuildingPurpose = _survey.PlannedInstall != null ? _survey.PlannedInstall.Purpose : BuildingPurpose.Housing;
                                _taxRuleCondition.InstallationLocalization = _survey.PlannedInstall != null ? _survey.PlannedInstall.Localization : InstallationLocalization.Ground;
                                break;
                            case SurveyRSETypeHotWater.HeatPump:
                                _taxRuleCondition.BuildingPurpose = BuildingPurpose.Housing;
                                _taxRuleCondition.InstallationLocalization = InstallationLocalization.Ground;
                                break;
                            default:
                                break;
                        }
                        break;
                    case SurveyType.Energy:
                        _taxRuleCondition.BuildingPurpose = _survey.PlannedInstall != null ? _survey.PlannedInstall.Purpose : BuildingPurpose.Housing;
                        _taxRuleCondition.InstallationLocalization = _survey.PlannedInstall != null ? _survey.PlannedInstall.Localization : InstallationLocalization.Ground;
                        break;
                    default:
                        break;
                }

                //NET PRICE CONDITIONS
                _priceRuleCondition.SurveyType = _survey.Type;
                _priceRuleCondition.RSEType = _survey.GetRSEType();
                _priceRuleCondition.Unit = _priceRules.First(r => r.SurveyType == _priceRuleCondition.SurveyType &&
                                           r.RSEType == _priceRuleCondition.RSEType).Unit;
                switch (_priceRuleCondition.Unit)
                {
                    case RSEPriceRuleUnit.FinalRSEPower:
                        _priceRuleCondition.NumberMax = _survey.ResultCalculation.FinalRSEPower;
                        break;
                    case RSEPriceRuleUnit.FinalSolConfig:
                        if (!string.IsNullOrEmpty(_survey.ResultCalculation.FinalSOLConfig))
                        {
                            double compareValue;
                            double.TryParse(_survey.ResultCalculation.FinalSOLConfig.Split(' ').First(), out compareValue);
                            _priceRuleCondition.NumberMax = compareValue;
                        }
                        break;
                    default:
                        break;
                }

            }
        }
        


        #region Properties
        public decimal Net
        {
            get
            {
                //double compareValue = double.MinValue;
                //switch (_unit)
                //{
                //    case RSEPriceRuleUnit.FinalRSEPower:
                //        compareValue = _survey.ResultCalculation.FinalRSEPower;
                //        break;
                //    case RSEPriceRuleUnit.FinalSolConfig:
                //        if (!string.IsNullOrEmpty(_survey.ResultCalculation.FinalSOLConfig))
                //        {
                //            double.TryParse(_survey.ResultCalculation.FinalSOLConfig.Split(' ').First(), out compareValue);
                //        }
                //        break;
                //    default:
                //        break;
                //}

                try
                {
                    var priceRule = _priceRules.Single(r =>
                                r.SurveyType == _priceRuleCondition.SurveyType &&
                                r.RSEType == _priceRuleCondition.RSEType &&
                                r.Unit == _priceRuleCondition.Unit && 
                                r.NumberMin.CompareTo(_priceRuleCondition.NumberMax) < 0 &&
                                r.NumberMax.CompareTo(_priceRuleCondition.NumberMax) >= 0);

                    return priceRule.Multiply ? 
                        decimal.Multiply(priceRule.NetPrice, Convert.ToDecimal(_priceRuleCondition.NumberMax)) : 
                        priceRule.NetPrice;

                }
                catch (Exception)
                {
                    //_logger.LogError("Nie znaleziono konfiguracji dla ceny netto: {0},{1},{2}", _survey.Type.DisplayName(), _survey.GetRSETypeName(), _survey.SurveyId);
                    return 0;
                }
            }

            private set { }
        }
        public decimal Tax
        {
            get
            {
                short tax = 0;

                try
                {
                    tax = _taxRules
                        .Single(r =>
                                r.SurveyType == _taxRuleCondition.SurveyType &&
                                r.RSEType == _taxRuleCondition.RSEType &&
                                r.BuildingPurpose == _taxRuleCondition.BuildingPurpose &&
                                r.InstallationLocalization == _taxRuleCondition.InstallationLocalization &&
                                r.UsableAreaMin < _taxRuleCondition.UsableAreaMax &&
                                r.UsableAreaMax >= _taxRuleCondition.UsableAreaMax )
                        .VAT;

                }
                catch (Exception)
                {
                    // System.Diagnostics.Debug.Print("VAT not found: PlannedInstall is null " + _survey.SurveyId.ToString());
                    System.Diagnostics.Debug.Print("VAT not found|" + _survey.SurveyId.ToString() +
                        "|Type:" + _survey.Type.ToString() +
                        "|RSE:" + _survey.GetRSEType() + "\n");
                   //     "|Purpose:" + _survey.PlannedInstall.Purpose.ToString() +
                   //     "|Localization:" + _survey.PlannedInstall.Localization.ToString() +
                   //     "|Area:" + _survey.Investment.UsableArea + "\n"); 
                }

                return Net * tax / 100;
            }
            private set { }
        }
        public decimal Gross
        {
            get => Net + Tax;
            private set { }
        }
        public decimal OwnerContribution
        {
            get
            {
                var p = _survey.Investment.Contract.Project;

                return p.GrossNetFundsType ?
                    (100 - p.UEFundsLevel) * this.Gross / 100 :
                    (100 - p.UEFundsLevel) * this.Net / 100;
            }
            private set { }
        }
        public decimal TaxLevel
        {
            get
            {
                short tax = 0;

                try
                {
                    tax = _taxRules
                        .Single(r =>
                                r.SurveyType == _taxRuleCondition.SurveyType &&
                                r.RSEType == _taxRuleCondition.RSEType &&
                                r.BuildingPurpose == _taxRuleCondition.BuildingPurpose &&
                                r.InstallationLocalization == _taxRuleCondition.InstallationLocalization &&
                                r.UsableAreaMin < _taxRuleCondition.UsableAreaMax &&
                                r.UsableAreaMax >= _taxRuleCondition.UsableAreaMax)
                        .VAT;
                }
                catch (Exception)
                {
                    // System.Diagnostics.Debug.Print("VAT not found: PlannedInstall is null " + _survey.SurveyId.ToString());
                    System.Diagnostics.Debug.Print("VAT not found|" + _survey.SurveyId.ToString() +
                        "|Type:" + _survey.Type.ToString() +
                        "|RSE:" + _survey.GetRSEType() + "\n");
                    //     "|Purpose:" + _survey.PlannedInstall.Purpose.ToString() +
                    //     "|Localization:" + _survey.PlannedInstall.Localization.ToString() +
                    //     "|Area:" + _survey.Investment.UsableArea + "\n"); 
                }

                return tax;
            }
            private set { }
        }
        #endregion  
    }

    public class RSEPriceRuleHelper
    {
        public static void CreateDefaultConfiguration(DoEkoContext context)
        {
            context.AddRange(GetDefaultPriceRules(1));
            context.SaveChanges();
        }
        public static ICollection<RSEPriceRule> GetPriceRules(DoEkoContext context, int projectId = 1)
        {
            ICollection<RSEPriceRule> rules = context.RSEPriceRules.Where(p => p.ProjectId == projectId).AsNoTracking().ToList();

            return rules.Count == 0 && projectId != 1 ? GetPriceRules(context, 1) : rules;
        }
        #region Defaults
        public static ICollection<RSEPriceRule> GetDefaultPriceRules(int projectId = 1)
        {
            return new List<RSEPriceRule>()
            {
                new RSEPriceRule(SurveyType.Energy, (int)SurveyRSETypeEnergy.PhotoVoltaic, netPrice: Convert.ToDecimal(4200), multiply: true,projectId: projectId),

                new RSEPriceRule(SurveyType.HotWater, (int)SurveyRSETypeHotWater.HeatPump, netPrice: Convert.ToDecimal(7812.78),projectId: projectId),

                new RSEPriceRule(SurveyType.HotWater, (int)SurveyRSETypeHotWater.Solar, RSEPriceRuleUnit.FinalSolConfig, 0, 2, Convert.ToDecimal(7400),projectId: projectId),
                new RSEPriceRule(SurveyType.HotWater, (int)SurveyRSETypeHotWater.Solar, RSEPriceRuleUnit.FinalSolConfig, 2, 3, Convert.ToDecimal(8900),projectId: projectId),
                new RSEPriceRule(SurveyType.HotWater, (int)SurveyRSETypeHotWater.Solar, RSEPriceRuleUnit.FinalSolConfig, 3, 4, Convert.ToDecimal(10900),projectId: projectId),

                new RSEPriceRule(SurveyType.CentralHeating, (int)SurveyRSETypeCentralHeating.HeatPumpAir, RSEPriceRuleUnit.FinalRSEPower, 0, 7, Convert.ToDecimal(22210.61),projectId: projectId),
                new RSEPriceRule(SurveyType.CentralHeating, (int)SurveyRSETypeCentralHeating.HeatPumpAir, RSEPriceRuleUnit.FinalRSEPower, 7, 9, Convert.ToDecimal(24442.83),projectId: projectId),
                new RSEPriceRule(SurveyType.CentralHeating, (int)SurveyRSETypeCentralHeating.HeatPumpAir, RSEPriceRuleUnit.FinalRSEPower, 9, 11, Convert.ToDecimal(27791.17),projectId: projectId),
                new RSEPriceRule(SurveyType.CentralHeating, (int)SurveyRSETypeCentralHeating.HeatPumpAir, RSEPriceRuleUnit.FinalRSEPower, 11, 13, Convert.ToDecimal(30023.39),projectId: projectId),
                new RSEPriceRule(SurveyType.CentralHeating, (int)SurveyRSETypeCentralHeating.HeatPumpAir, RSEPriceRuleUnit.FinalRSEPower, 13, double.MaxValue, Convert.ToDecimal(37836.17),projectId: projectId),

                new RSEPriceRule(SurveyType.CentralHeating, (int)SurveyRSETypeCentralHeating.HeatPump, RSEPriceRuleUnit.FinalRSEPower, Convert.ToDouble(0), Convert.ToDouble(7), Convert.ToDecimal(34260.16),projectId: projectId),
                new RSEPriceRule(SurveyType.CentralHeating, (int)SurveyRSETypeCentralHeating.HeatPump, RSEPriceRuleUnit.FinalRSEPower, Convert.ToDouble(7), Convert.ToDouble(13), Convert.ToDecimal(39837.4),projectId: projectId),
                new RSEPriceRule(SurveyType.CentralHeating, (int)SurveyRSETypeCentralHeating.HeatPump, RSEPriceRuleUnit.FinalRSEPower, Convert.ToDouble(13), Convert.ToDouble(17.1), Convert.ToDecimal(48601.63),projectId: projectId),
                new RSEPriceRule(SurveyType.CentralHeating, (int)SurveyRSETypeCentralHeating.HeatPump, RSEPriceRuleUnit.FinalRSEPower, Convert.ToDouble(17.1), Convert.ToDouble(25), Convert.ToDecimal(57365.85),projectId: projectId),
                new RSEPriceRule(SurveyType.CentralHeating, (int)SurveyRSETypeCentralHeating.HeatPump, RSEPriceRuleUnit.FinalRSEPower, Convert.ToDouble(25), Convert.ToDouble(28.8), Convert.ToDecimal(71707.32),projectId: projectId),
                new RSEPriceRule(SurveyType.CentralHeating, (int)SurveyRSETypeCentralHeating.HeatPump, RSEPriceRuleUnit.FinalRSEPower, Convert.ToDouble(28.8), double.MaxValue, Convert.ToDecimal(71707.32),projectId: projectId),

                new RSEPriceRule(SurveyType.CentralHeating, (int)SurveyRSETypeCentralHeating.PelletBoiler, RSEPriceRuleUnit.FinalRSEPower, Convert.ToDouble(0), Convert.ToDouble(10), Convert.ToDecimal(9500),projectId: projectId),
                new RSEPriceRule(SurveyType.CentralHeating, (int)SurveyRSETypeCentralHeating.PelletBoiler, RSEPriceRuleUnit.FinalRSEPower, Convert.ToDouble(10), Convert.ToDouble(15), Convert.ToDecimal(11500),projectId: projectId),
                new RSEPriceRule(SurveyType.CentralHeating, (int)SurveyRSETypeCentralHeating.PelletBoiler, RSEPriceRuleUnit.FinalRSEPower, Convert.ToDouble(15), Convert.ToDouble(20), Convert.ToDecimal(12500),projectId: projectId),
                new RSEPriceRule(SurveyType.CentralHeating, (int)SurveyRSETypeCentralHeating.PelletBoiler, RSEPriceRuleUnit.FinalRSEPower, Convert.ToDouble(20), Convert.ToDouble(25), Convert.ToDecimal(13500),projectId: projectId),
                new RSEPriceRule(SurveyType.CentralHeating, (int)SurveyRSETypeCentralHeating.PelletBoiler, RSEPriceRuleUnit.FinalRSEPower, Convert.ToDouble(25), Convert.ToDouble(35), Convert.ToDecimal(15000),projectId: projectId),
                new RSEPriceRule(SurveyType.CentralHeating, (int)SurveyRSETypeCentralHeating.PelletBoiler, RSEPriceRuleUnit.FinalRSEPower, Convert.ToDouble(35), double.MaxValue, Convert.ToDecimal(15000),projectId: projectId)
            };
        }
        #endregion
    }
    public class RSEPriceTaxRuleHelper
    {
        public static void CreateDefaultConfiguration(DoEkoContext context)
        {
            context.AddRange(GetDefaultTaxRules(1));
            context.SaveChanges();
        }

        public static ICollection<RSEPriceTaxRule> GetTaxRules(DoEkoContext context, int projectId = 1)
        {
            ICollection<RSEPriceTaxRule> rules = context.RSEPriceTaxRules.Where(p => p.ProjectId == projectId).AsNoTracking().ToList();

            return rules.Count == 0 && projectId != 1 ? GetTaxRules(context, 1) : rules;
        }

        #region Defaults
        public static ICollection<RSEPriceTaxRule> GetDefaultTaxRules(int projectId = 1)
        {
            return new List<RSEPriceTaxRule>()
            {
                new RSEPriceTaxRule(SurveyType.Energy,(int)SurveyRSETypeEnergy.PhotoVoltaic,InstallationLocalization.Roof,BuildingPurpose.Housing,0,300,8,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.Energy,(int)SurveyRSETypeEnergy.PhotoVoltaic,InstallationLocalization.Roof,BuildingPurpose.Housing,300, tax: 8,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.Energy,(int)SurveyRSETypeEnergy.PhotoVoltaic,InstallationLocalization.Roof,BuildingPurpose.Business,tax: 23,projectId: projectId),

                new RSEPriceTaxRule(SurveyType.Energy,(int)SurveyRSETypeEnergy.PhotoVoltaic,InstallationLocalization.Ground,BuildingPurpose.Business,tax: 23,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.Energy,(int)SurveyRSETypeEnergy.PhotoVoltaic,InstallationLocalization.Ground,BuildingPurpose.Housing,tax: 23,projectId: projectId),

                new RSEPriceTaxRule(SurveyType.Energy,(int)SurveyRSETypeEnergy.PhotoVoltaic,InstallationLocalization.Wall,BuildingPurpose.Housing,0,300,tax: 8,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.Energy,(int)SurveyRSETypeEnergy.PhotoVoltaic,InstallationLocalization.Wall,BuildingPurpose.Housing,300,tax: 23,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.Energy,(int)SurveyRSETypeEnergy.PhotoVoltaic,InstallationLocalization.Wall,BuildingPurpose.Housing,tax: 8,projectId: projectId),

                new RSEPriceTaxRule(SurveyType.HotWater,(int)SurveyRSETypeHotWater.Solar,InstallationLocalization.Roof,BuildingPurpose.Housing,0,300,tax: 8,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.HotWater,(int)SurveyRSETypeHotWater.Solar,InstallationLocalization.Roof,BuildingPurpose.Housing,300,tax: 23,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.HotWater,(int)SurveyRSETypeHotWater.Solar,InstallationLocalization.Roof,BuildingPurpose.Business,tax: 23,projectId: projectId),

                new RSEPriceTaxRule(SurveyType.HotWater,(int)SurveyRSETypeHotWater.Solar,InstallationLocalization.Ground,BuildingPurpose.Housing,tax: 23,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.HotWater,(int)SurveyRSETypeHotWater.Solar,InstallationLocalization.Ground,BuildingPurpose.Business,tax: 23,projectId: projectId),

                new RSEPriceTaxRule(SurveyType.HotWater,(int)SurveyRSETypeHotWater.Solar,InstallationLocalization.Wall,BuildingPurpose.Housing,0,300, tax: 8,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.HotWater,(int)SurveyRSETypeHotWater.Solar,InstallationLocalization.Wall,BuildingPurpose.Housing,300,tax: 23,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.HotWater,(int)SurveyRSETypeHotWater.Solar,InstallationLocalization.Wall,BuildingPurpose.Business,tax: 23,projectId: projectId),
                //HW
                new RSEPriceTaxRule(SurveyType.HotWater,(int)SurveyRSETypeHotWater.HeatPump,InstallationLocalization.Roof,BuildingPurpose.Housing,0,300,tax: 8,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.HotWater,(int)SurveyRSETypeHotWater.HeatPump,InstallationLocalization.Ground,BuildingPurpose.Housing,0,300,tax: 8,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.HotWater,(int)SurveyRSETypeHotWater.HeatPump,InstallationLocalization.Wall,BuildingPurpose.Housing,0,300,tax: 8,projectId: projectId),

                new RSEPriceTaxRule(SurveyType.HotWater,(int)SurveyRSETypeHotWater.HeatPump,InstallationLocalization.Roof,BuildingPurpose.Housing,300,tax: 23,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.HotWater,(int)SurveyRSETypeHotWater.HeatPump,InstallationLocalization.Ground,BuildingPurpose.Housing,300,tax: 23,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.HotWater,(int)SurveyRSETypeHotWater.HeatPump,InstallationLocalization.Wall,BuildingPurpose.Housing,300,tax: 23,projectId: projectId),

                new RSEPriceTaxRule(SurveyType.HotWater,(int)SurveyRSETypeHotWater.HeatPump,InstallationLocalization.Roof,BuildingPurpose.Business,tax: 23,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.HotWater,(int)SurveyRSETypeHotWater.HeatPump,InstallationLocalization.Ground,BuildingPurpose.Business,tax: 23,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.HotWater,(int)SurveyRSETypeHotWater.HeatPump,InstallationLocalization.Wall,BuildingPurpose.Business,tax: 23,projectId: projectId),
                //CH
                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.HeatPumpAir,InstallationLocalization.Roof,BuildingPurpose.Housing,0,300,tax: 8,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.HeatPumpAir,InstallationLocalization.Ground,BuildingPurpose.Housing,0,300,tax: 8,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.HeatPumpAir,InstallationLocalization.Wall,BuildingPurpose.Housing,0,300,tax: 8,projectId: projectId),

                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.HeatPumpAir,InstallationLocalization.Roof,BuildingPurpose.Housing,300,tax: 23,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.HeatPumpAir,InstallationLocalization.Ground,BuildingPurpose.Housing,300,tax: 23,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.HeatPumpAir,InstallationLocalization.Wall,BuildingPurpose.Housing,300,tax: 23,projectId: projectId),

                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.HeatPumpAir,InstallationLocalization.Roof,BuildingPurpose.Business,tax: 23,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.HeatPumpAir,InstallationLocalization.Ground,BuildingPurpose.Business,tax: 23,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.HeatPumpAir,InstallationLocalization.Wall,BuildingPurpose.Business,tax: 23,projectId: projectId),
                //
                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.HeatPump,InstallationLocalization.Roof,BuildingPurpose.Housing,tax: 23,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.HeatPump,InstallationLocalization.Ground,BuildingPurpose.Housing,tax: 23,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.HeatPump,InstallationLocalization.Wall,BuildingPurpose.Housing,tax: 23,projectId: projectId),

                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.HeatPump,InstallationLocalization.Roof,BuildingPurpose.Business,tax: 23,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.HeatPump,InstallationLocalization.Ground,BuildingPurpose.Business,tax: 23,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.HeatPump,InstallationLocalization.Wall,BuildingPurpose.Business,tax: 23,projectId: projectId),
                //
                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.PelletBoiler,InstallationLocalization.Roof,BuildingPurpose.Housing,0,300,tax: 8,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.PelletBoiler,InstallationLocalization.Ground,BuildingPurpose.Housing,0,300,tax: 8,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.PelletBoiler,InstallationLocalization.Wall,BuildingPurpose.Housing,0,300,tax: 8,projectId: projectId),

                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.PelletBoiler,InstallationLocalization.Roof,BuildingPurpose.Housing,300,tax: 23,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.PelletBoiler,InstallationLocalization.Ground,BuildingPurpose.Housing,300,tax: 23,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.PelletBoiler,InstallationLocalization.Wall,BuildingPurpose.Housing,300,tax: 23,projectId: projectId),

                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.PelletBoiler,InstallationLocalization.Roof,BuildingPurpose.Business,tax: 23,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.PelletBoiler,InstallationLocalization.Ground,BuildingPurpose.Business,tax: 23,projectId: projectId),
                new RSEPriceTaxRule(SurveyType.CentralHeating,(int)SurveyRSETypeCentralHeating.PelletBoiler,InstallationLocalization.Wall,BuildingPurpose.Business,tax: 23,projectId: projectId)
            };
        }
        #endregion
    }
    //public class EEPVRSEPriceHelper : RSEPriceHelper
    //{
    //    private readonly decimal price;
    //    public EEPVRSEPriceHelper(ILoggerFactory loggerFactory, DoEkoContext context, SurveyEnergy survey) : base(loggerFactory, context, survey)
    //    {
    //        price = _priceRules.Single(r => r.SurveyType == SurveyType.Energy && r.RSEType == (int)SurveyRSETypeEnergy.PhotoVoltaic).NetPrice;
    //    }
    //    public override decimal Net {
    //        get
    //        {
    //            return decimal.Multiply(Convert.ToDecimal(_survey.ResultCalculation.FinalRSEPower),price);
    //        }
    //    }
    //}

    //public class HWHPRSEPriceHelper : RSEPriceHelper
    //{
    //    private readonly decimal netPrice;
    //    public HWHPRSEPriceHelper(ILoggerFactory loggerFactory, DoEkoContext context, SurveyHotWater survey) : base(loggerFactory,context,survey)
    //    {
    //        netPrice = _priceRules
    //            .Single(r => r.SurveyType == SurveyType.HotWater &&
    //                        r.RSEType == (int)SurveyRSETypeHotWater.HeatPump)
    //            .NetPrice;
    //    }
    //    public override decimal Net {
    //        get
    //        {
    //            return netPrice;
    //        }
    //    }
    //}

    //public class HWSOLRSEPriceHelper : RSEPriceHelper
    //{
    //    public HWSOLRSEPriceHelper(ILoggerFactory loggerFactory, DoEkoContext context, SurveyHotWater survey) : base(loggerFactory, context, survey)
    //    {
    //        _priceRules = _priceRules.Where(r =>
    //                                        r.SurveyType == SurveyType.HotWater &&
    //                                        r.RSEType == (int)SurveyRSETypeHotWater.Solar)
    //                                 .ToList();
    //    }
    //    public override decimal Net
    //    {
    //        get
    //        {
    //            double.TryParse(_survey.ResultCalculation.FinalSOLConfig.Split(' ').First(), out double solConfig);
    //            try
    //            {
    //                decimal price = _priceRules.Single(r => r.Number.CompareTo(solConfig) == 0).NetPrice;
    //                return price;
    //            }
    //            catch (Exception)
    //            {
    //                _logger.LogError("Nie znaleziono konfiguracji dla ceny netto: {0},{1},{2}", SurveyType.HotWater.DisplayName(), SurveyRSETypeHotWater.Solar.DisplayName(), solConfig.ToString());
    //                return 0;
    //            }
    //        }
    //    }
    //}
}
