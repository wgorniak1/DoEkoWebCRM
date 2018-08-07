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
    public class SurveyPhotoViewComponent : ViewComponent
    {
        private DoEkoContext _context;
        private IFileStorage _fileStorage;

        public SurveyPhotoViewComponent(DoEkoContext context, IFileStorage fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }
        public async Task<IViewComponentResult> InvokeAsync(Guid surveyId, SurveyViewMode viewMode)
        {
            SurveyType type = await _context.Surveys.Where(s => s.SurveyId == surveyId).Select(s => s.Type).SingleAsync();

            SurveyPhotoViewModel model = new SurveyPhotoViewModel();
            
            switch (type)
            {
                case SurveyType.CentralHeating:

                    SurveyCentralHeating srvCH = await _context.SurveysCH.SingleAsync(s => s.SurveyId == surveyId);

                    model.Attachments = await this.GetAttachmentsAsync(srvCH.SurveyId, srvCH.InvestmentId);
                    model.InvestmentId = srvCH.InvestmentId;
                    model.SurveyId = srvCH.SurveyId;
                    model.Type_CH = srvCH.RSEType;

                    return View("PhotoCH", model);
                case SurveyType.HotWater:
                    SurveyHotWater srvHW = await _context.SurveysHW.SingleAsync(s => s.SurveyId == surveyId);

                    model.Attachments = await this.GetAttachmentsAsync(srvHW.SurveyId, srvHW.InvestmentId);
                    model.InvestmentId = srvHW.InvestmentId;
                    model.SurveyId = srvHW.SurveyId;
                    model.Type_HW = srvHW.RSEType;

                    return View("PhotoHW", model);
                case SurveyType.Energy:
                    SurveyEnergy srvEN = await _context.SurveysEN.SingleAsync(s => s.SurveyId == surveyId);

                    model.Attachments = await this.GetAttachmentsAsync(srvEN.SurveyId, srvEN.InvestmentId);
                    model.InvestmentId = srvEN.InvestmentId;
                    model.SurveyId = srvEN.SurveyId;
                    model.Type_EN = srvEN.RSEType;

                    return View("PhotoEN", model);
                default:
                    return Content(string.Empty);
            }
        }

        private async Task<Dictionary<string,SurveyPhoto>> GetAttachmentsAsync(Guid surveyId, Guid investmentId)
        {
            var surveyBlockBlobs = (await (await _fileStorage
                .GetBlobContainerAsync(EnuAzureStorageContainerType.Survey))
                .GetDirectoryReference(surveyId.ToString())
                .ListBlobsAsync(true, BlobListingDetails.None, null, new BlobContinuationToken(), null, null))
                .OfType<CloudBlockBlob>();

            var investmentBlockBlobs = (await (await _fileStorage
                .GetBlobContainerAsync(EnuAzureStorageContainerType.Investment))
                .GetDirectoryReference(investmentId.ToString())
                .ListBlobsAsync(true, BlobListingDetails.None, null, new BlobContinuationToken(), null, null))
                .OfType<CloudBlockBlob>()
                .Where(b => b.Name.Split('/').Reverse().ToArray().First().Contains("Picture"));

            
            Dictionary<string,SurveyPhoto> FileList = new Dictionary<string, SurveyPhoto>();

            foreach (var item in surveyBlockBlobs.Union(investmentBlockBlobs).OrderBy(i=>i.Name))
            {
                var partNames = item.Name.Split('/').Reverse().ToArray();

                FileList.Add(partNames[1], new SurveyPhoto { Name = partNames[0], Url = item.Uri.AbsoluteUri });
            }

            return FileList;

        }
    }
}