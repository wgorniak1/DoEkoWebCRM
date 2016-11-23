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
    public class SurveyBoilerRoomViewComponent : ViewComponent
    {
        private DoEkoContext _context;

        public SurveyBoilerRoomViewComponent(DoEkoContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(Guid surveyId, SurveyViewMode viewMode)
        {
            Survey srv = await _context.Surveys
                .Include(s=>s.BoilerRoom)
                .SingleAsync(s => s.SurveyId == surveyId);

            if (srv.BoilerRoom == null)
            {
                srv.BoilerRoom = new SurveyDetBoilerRoom() { Survey = srv, SurveyId = srv.SurveyId };
            }

            switch (srv.Type)
            {
                case SurveyType.CentralHeating:
                    return View("BoilerRoomCH", srv.BoilerRoom);
                case SurveyType.HotWater:
                    return View("BoilerRoomHW", srv.BoilerRoom);
                case SurveyType.Energy:
                    return View("BoilerRoomEN", srv.BoilerRoom);
                default:
                    return Content(string.Empty);
            }
        }
    }
}