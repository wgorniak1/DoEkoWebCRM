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
    public class SurveyAirConditionViewComponent : ViewComponent
    {
        private DoEkoContext _context;

        public SurveyAirConditionViewComponent(DoEkoContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(Guid surveyId, SurveyViewMode viewMode)
        {
            Survey srv = await _context.Surveys
                .Include(s=>s.AirCondition)
                .SingleAsync(s => s.SurveyId == surveyId);
            if (srv.AirCondition == null)
            {
                srv.AirCondition = new SurveyDetAirCond() { SurveyId = srv.SurveyId, Survey = srv };
            }
            switch (srv.Type)
            {
                case SurveyType.CentralHeating:
                    return View("AirConditionCH", srv.AirCondition);
                case SurveyType.HotWater:
                    return View("AirConditionHW", srv.AirCondition);
                case SurveyType.Energy:
                    return View("AirConditionEN", srv.AirCondition);
                default:
                    return Content(string.Empty);
            }
        }
    }
}