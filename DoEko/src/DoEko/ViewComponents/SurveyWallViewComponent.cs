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
    public class SurveyWallViewComponent : ViewComponent
    {
        private DoEkoContext _context;

        public SurveyWallViewComponent(DoEkoContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(Guid surveyId, SurveyViewMode viewMode)
        {
            Survey srv = await _context.Surveys
                .Include(s=>s.Wall)
                .SingleAsync(s => s.SurveyId == surveyId);
            if (srv.Wall == null)
            {
                srv.Wall = new SurveyDetWall() { Survey = srv, SurveyId = srv.SurveyId };
            }
            switch (srv.Type)
            {
                case SurveyType.CentralHeating:
                    return View("WallCH", srv.Wall);
                case SurveyType.HotWater:
                    return View("WallHW", srv.Wall);
                case SurveyType.Energy:
                    return View("WallEN", srv.Wall);
                default:
                    return Content(string.Empty);
            }
        }
    }
}