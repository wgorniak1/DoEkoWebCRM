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
using DoEko.Services;
using DoEko.ViewComponents.ViewModels;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DoEko.ViewComponents
{
    [ViewComponent]
    public class ReportTemplateViewComponent : ViewComponent
    {
        //private DoEkoContext _context;
        //private IFileStorage _fileStorage;

        //public ReportTemplateViewComponent(DoEkoContext context, IFileStorage fileStorage)
        //{
        //    _context = context;
        //    _fileStorage = fileStorage;
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> Invoke(ReportTemplateViewModel model)
        {
            return await Task.Factory.StartNew<IViewComponentResult>(() => View("Default", model));
        }

    }
}