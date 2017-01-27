using DoEko.Models.DoEko;
using DoEko.Models.DoEko.Survey;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoEko.ViewComponents.ViewModels;
using DoEko.Controllers.Helpers;

namespace DoEko.ViewComponents
{
    [ViewComponent]
    public class SurveyAuditENViewComponent : ViewComponent
    {
        private DoEkoContext _context;

        public SurveyAuditENViewComponent(DoEkoContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(Guid surveyId, SurveyViewMode viewMode)
        {
            Guid investmentId = await _context.Surveys.Where(s => s.SurveyId == surveyId).Select(s => s.InvestmentId).SingleAsync();

            Investment inv = await _context.Investments
                .Include(i => i.Surveys).ThenInclude(s => s.Audit)
                .Include(i => i.Surveys).ThenInclude(s => s.Investment)
                .SingleAsync(i => i.InvestmentId == investmentId);

            Survey srv = inv.Surveys.Where(s => s.SurveyId == surveyId).Single();

            //Try to copy some values from other existing surveys 
            //in case this survey is filled for the first time
            if (srv.Audit == null)
            {
                srv.Audit = new SurveyDetEnergyAudit() { SurveyId = srv.SurveyId, Survey = srv };
            }
            if (srv.Audit.ElectricityPower == 0)
            {
                try
                {
                    double enPower = 0;
                    switch (srv.GetRSEType())
                    {
                        case (int)SurveyRSETypeCentralHeating.HeatPump:
                            enPower = inv.Surveys.Where(s => s.GetRSEType() == (int)SurveyRSETypeEnergy.PhotoVoltaic && s.SurveyId != surveyId && s.Audit.ElectricityPower != 0).Select(s=>s.Audit.ElectricityPower).First();
                            break;
                        case (int)SurveyRSETypeEnergy.PhotoVoltaic:
                            enPower = inv.Surveys.Where(s => s.GetRSEType() == (int)SurveyRSETypeCentralHeating.HeatPump && s.SurveyId != surveyId && s.Audit.ElectricityPower != 0).Select(s=>s.Audit.ElectricityPower).First();
                            break;
                        default:
                            break;
                    }
                    //
                    srv.Audit.ElectricityPower = enPower;
                }
                catch (Exception) { }
            }
            
            SurveyAuditViewModel model = new SurveyAuditViewModel(srv);

            switch (srv.Type)
            {
                case SurveyType.CentralHeating:
                    return View("AuditENCH", model);
                case SurveyType.HotWater:
                    return View("AuditENHW", model);
                case SurveyType.Energy:
                    return View("AuditENEN", model);
                default:
                    return Content(string.Empty);
            }
        }
    }
}