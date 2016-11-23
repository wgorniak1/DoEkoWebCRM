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
using DoEko.Controllers.Helpers;

namespace DoEko.ViewComponents
{
    [ViewComponent]
    public class SurveyBuildingGeneralInfoViewComponent : ViewComponent
    {
        private DoEkoContext _context;

        public SurveyBuildingGeneralInfoViewComponent(DoEkoContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(Guid surveyId, SurveyViewMode viewMode)
        {
            Survey srv = await _context.Surveys
                .Include(s=>s.Investment)
                .Include(s=>s.Building)
                .SingleAsync(s => s.SurveyId == surveyId);

            switch (srv.Type)
            {
                case SurveyType.CentralHeating:
                    return View("BuildingGeneralInfoCH", await _context.SurveysCH.SingleAsync(s => s.SurveyId == surveyId));
                case SurveyType.HotWater:
                    return View("BuildingGeneralInfoHW", await _context.SurveysHW.SingleAsync(s => s.SurveyId == surveyId));
                case SurveyType.Energy:
                    return View("BuildingGeneralInfoEN", await _context.SurveysEN.SingleAsync(s => s.SurveyId == surveyId));
                default:
                    return Content(string.Empty);
            }
        }
    }
}