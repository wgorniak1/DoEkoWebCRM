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
            Survey srv = await _context.Surveys
                .Include(s=>s.Audit)
                .Include(s=>s.Investment)
                .SingleAsync(s => s.SurveyId == surveyId);
            if (srv.Audit == null)
            {
                srv.Audit = new SurveyDetEnergyAudit() { Survey = srv, SurveyId = srv.SurveyId };
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