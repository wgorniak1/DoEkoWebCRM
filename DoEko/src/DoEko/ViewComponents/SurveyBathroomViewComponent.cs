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
    public class SurveyBathroomViewComponent : ViewComponent
    {
        private DoEkoContext _context;

        public SurveyBathroomViewComponent(DoEkoContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(Guid surveyId, SurveyViewMode viewMode)
        {
            Survey srv = await _context.Surveys
                .Include(s=>s.BathRoom)
                .SingleAsync(s => s.SurveyId == surveyId);

            if (srv.BathRoom == null)
            {
                srv.BathRoom = new SurveyDetBathroom() { Survey = srv, SurveyId = srv.SurveyId };
            }

            switch (srv.Type)
            {
                case SurveyType.CentralHeating:
                    return View("BathRoomCH", srv.BathRoom);
                case SurveyType.HotWater:
                    return View("BathRoomHW", srv.BathRoom);
                case SurveyType.Energy:
                    return View("BathRoomEN", srv.BathRoom);
                default:
                    return Content(string.Empty);
            }
        }
    }
}