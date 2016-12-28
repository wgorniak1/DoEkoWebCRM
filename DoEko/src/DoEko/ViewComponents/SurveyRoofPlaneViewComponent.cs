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
                .Where(s => s.SurveyId == surveyId)
                .Select(s => new Survey{ RoofPlanes = s.RoofPlanes,
                                         InvestmentId = s.InvestmentId,
                                         SurveyId = s.SurveyId }).SingleAsync();

            SurveyRoofPlaneViewModel model = new SurveyRoofPlaneViewModel();

            if (roofPlaneId == Guid.Empty)
            {
                model.RoofNumber = srv.RoofPlanes.Count + 1;
                model.RoofTotal = srv.RoofPlanes.Count + 1;
                model.SurveyId = srv.SurveyId;
                //
                //Copy roof plane from solar => pv or pv => solar
                //or create new or just get
                if (srv.RoofPlanes.Count == 0)
                {
                    Survey refSrv = null;
                    if (isSurveyType((int)SurveyRSETypeEnergy.PhotoVoltaic, srv.SurveyId))
                        refSrv = getSurveyByRSE(srv.InvestmentId,SurveyRSETypeHotWater.Solar);
                    else if (isSurveyType((int)SurveyRSETypeHotWater.Solar, srv.SurveyId))
                        refSrv = getSurveyByRSE(srv.InvestmentId, SurveyRSETypeEnergy.PhotoVoltaic);

                    if (refSrv != null && refSrv.RoofPlanes != null)
                    {
                        model.Plane = refSrv.RoofPlanes.First();
                        model.Plane.SurveyId = srv.SurveyId;
                        model.Plane.RoofPlaneId = Guid.Empty;
                    }
                    else
                    {
                        model.Plane = new SurveyDetRoof() { SurveyId = srv.SurveyId };
                    }
                }
                else
                {
                    model.Plane = new SurveyDetRoof() { SurveyId = srv.SurveyId };
                }

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public bool isSurveyType(int type, Guid surveyId)
        {
            int RSEType;
            switch (_context.Surveys.Where(s => s.SurveyId == surveyId).Select(s => s.Type).SingleOrDefault())
            {
                case SurveyType.CentralHeating:
                    RSEType = (int)_context.SurveysCH.Where(s => s.SurveyId == surveyId).Select(s => s.RSEType).SingleOrDefault();
                    break;
                case SurveyType.Energy:
                    RSEType = (int)_context.SurveysEN.Where(s => s.SurveyId == surveyId).Select(s => s.RSEType).SingleOrDefault();
                    break;
                case SurveyType.HotWater:
                    RSEType = (int)_context.SurveysHW.Where(s => s.SurveyId == surveyId).Select(s => s.RSEType).SingleOrDefault();
                    break;
                default:
                    RSEType = -4;
                    break;
            }

            return type == RSEType;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="investmentId"></param>
        /// <param name="CHType"></param>
        /// <param name="ENType"></param>
        /// <param name="HWType"></param>
        /// <returns></returns>
        public Survey getSurveyByRSE(Guid investmentId, SurveyRSETypeCentralHeating Type)
        {
                return _context.SurveysCH.Where(s => s.InvestmentId == investmentId && s.RSEType == Type).SingleOrDefault();
        }
        public Survey getSurveyByRSE(Guid investmentId, SurveyRSETypeEnergy Type)
        {
                return _context.SurveysEN
                .Include(s => s.RoofPlanes)
                .Where(s => s.InvestmentId == investmentId && s.RSEType == Type).SingleOrDefault();
        }
        public Survey getSurveyByRSE(Guid investmentId, SurveyRSETypeHotWater Type)
        {
                return _context.SurveysHW
                .Include(s=>s.RoofPlanes)
                .Where(s => s.InvestmentId == investmentId && s.RSEType == Type).SingleOrDefault();
        }

    }
}