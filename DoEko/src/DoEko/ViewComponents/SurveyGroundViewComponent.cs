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
    public class SurveyGroundViewComponent : ViewComponent
    {
        private DoEkoContext _context;

        public SurveyGroundViewComponent(DoEkoContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(Guid surveyId, SurveyViewMode viewMode)
        {
            Survey srv = await _context.Surveys
                .Include(s=>s.Ground)
                .SingleAsync(s => s.SurveyId == surveyId);
            if (srv.Ground == null)
            {
                srv.Ground = new SurveyDetGround() { Survey = srv, SurveyId = srv.SurveyId };
            }
            switch (srv.Type)
            {
                case SurveyType.CentralHeating:
                    return View("GroundCH", srv.Ground);
                case SurveyType.HotWater:
                    return View("GroundHW", srv.Ground);
                case SurveyType.Energy:
                    return View("GroundEN", srv.Ground);
                default:
                    return Content(string.Empty);
            }
        }
    }
}