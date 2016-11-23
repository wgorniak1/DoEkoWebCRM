﻿using DoEko.Models.DoEko;
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
    public class SurveyPhotoViewComponent : ViewComponent
    {
        private DoEkoContext _context;

        public SurveyPhotoViewComponent(DoEkoContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(Guid surveyId, SurveyViewMode viewMode)
        {
            Survey srv = await _context.Surveys.SingleAsync(s => s.SurveyId == surveyId);

            switch (srv.Type)
            {
                case SurveyType.CentralHeating:
                    return View("PhotoCH");
                case SurveyType.HotWater:
                    return View("PhotoHW");
                case SurveyType.Energy:
                    return View("PhotoEN");
                default:
                    return Content(string.Empty);
            }
        }
    }
}