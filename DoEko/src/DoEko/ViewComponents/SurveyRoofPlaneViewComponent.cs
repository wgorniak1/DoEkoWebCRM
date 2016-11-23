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
    public class SurveyRoofPlaneViewComponent : ViewComponent
    {
        private DoEkoContext _context;

        public SurveyRoofPlaneViewComponent(DoEkoContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(Guid surveyId, Guid roofPlaneId, SurveyViewMode viewMode)
        {
            Survey srv = await _context.Surveys
                .Include(s=>s.RoofPlanes)
                .SingleAsync(s => s.SurveyId == surveyId);
            SurveyRoofPlaneViewModel model = new SurveyRoofPlaneViewModel();

            if (string.IsNullOrEmpty(roofPlaneId.ToString()))
            {
                model.Plane = new SurveyDetRoof() { SurveyId = srv.SurveyId };
                model.RoofNumber = srv.RoofPlanes.Count + 1;
                model.RoofTotal = srv.RoofPlanes.Count + 1;
                model.SurveyId = srv.SurveyId;
            }
            else
            {
                model.Plane = srv.RoofPlanes.Single(r => r.RoofPlaneId == roofPlaneId);
                model.RoofNumber = srv.RoofPlanes.IndexOf(model.Plane) + 1;
                model.RoofTotal = srv.RoofPlanes.Count;
                model.SurveyId = srv.SurveyId;
            }
            switch (srv.Type)
            {
                case SurveyType.CentralHeating:
                    return View("RoofPlaneCH", model);
                case SurveyType.HotWater:
                    return View("RoofPlaneHW", model);
                case SurveyType.Energy:
                    return View("RoofPlaneEN", model);
                default:
                    return Content(string.Empty);
            }
        }
    }
}