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
    public class SurveyPlannedInstallationViewComponent : ViewComponent
    {
        private DoEkoContext _context;

        public SurveyPlannedInstallationViewComponent(DoEkoContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(Guid surveyId, SurveyViewMode viewMode)
        {
            Survey srv = await _context.Surveys
                .Include(s=>s.PlannedInstall)
                .SingleAsync(s => s.SurveyId == surveyId);
            if (srv.PlannedInstall == null )
            {
                srv.PlannedInstall = new SurveyDetPlannedInstall();
            }
            switch (srv.Type)
            {
                case SurveyType.CentralHeating:
                    return View("PlannedInstallationCH", srv.PlannedInstall);
                case SurveyType.HotWater:
                    return View("PlannedInstallationHW", srv.PlannedInstall);
                case SurveyType.Energy:
                    return View("PlannedInstallationEN", srv.PlannedInstall);
                default:
                    return Content(string.Empty);
            }
        }
    }
}