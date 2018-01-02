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

                    model.Attachments = this.GetAttachments(srvCH.SurveyId, srvCH.InvestmentId);
                    model.InvestmentId = srvCH.InvestmentId;
                    model.SurveyId = srvCH.SurveyId;
                    model.Type_CH = srvCH.RSEType;

                    return View("PhotoCH", model);
                case SurveyType.HotWater:
                    SurveyHotWater srvHW = await _context.SurveysHW.SingleAsync(s => s.SurveyId == surveyId);

                    model.Attachments = this.GetAttachments(srvHW.SurveyId, srvHW.InvestmentId);
                    model.InvestmentId = srvHW.InvestmentId;
                    model.SurveyId = srvHW.SurveyId;
                    model.Type_HW = srvHW.RSEType;

                    return View("PhotoHW", model);
                case SurveyType.Energy:
                    SurveyEnergy srvEN = await _context.SurveysEN.SingleAsync(s => s.SurveyId == surveyId);

                    model.Attachments = this.GetAttachments(srvEN.SurveyId, srvEN.InvestmentId);
                    model.InvestmentId = srvEN.InvestmentId;
                    model.SurveyId = srvEN.SurveyId;
                    model.Type_EN = srvEN.RSEType;

                    return View("PhotoEN", model);
                default:
                    return Content(string.Empty);
            }
        }

        private Dictionary<string,SurveyPhoto> GetAttachments(Guid surveyId, Guid investmentId)
        {
            CloudBlobContainer Container = _fileStorage.GetBlobContainer(EnuAzureStorageContainerType.Survey);//account.GetBlobContainer(enuAzureStorageContainerType.Project);
            CloudBlobContainer ContainerInv = _fileStorage.GetBlobContainer(EnuAzureStorageContainerType.Investment);//account.GetBlobContainer(enuAzureStorageContainerType.Project);
            var SurveyBlockBlobs     = Container.ListBlobs(prefix: surveyId.ToString(), useFlatBlobListing: true).OfType<CloudBlockBlob>();

            Dictionary<string,SurveyPhoto> FileList = new Dictionary<string, SurveyPhoto>();

            foreach (var BlockBlob in SurveyBlockBlobs)
            {
                var partNames = BlockBlob.Name.Split('/').Reverse().ToArray();

                if (partNames[1].Equals("Picture0") || partNames[1].Equals("Picture5") || partNames[1].Equals("Picture10"))
                {
                    partNames[2] = investmentId.ToString();
                    string targetName = partNames[2] + '/' + partNames[1] + '/' + partNames[0];
                    CloudBlockBlob targetBlob = ContainerInv.GetBlockBlobReference(targetName);
                    targetBlob.StartCopy(BlockBlob);
                    while (targetBlob.CopyState.Status != CopyStatus.Success)
                    {
                        //
                    }
                    BlockBlob.Delete();
                }
                else
                {
                    FileList.Add(partNames[1], new SurveyPhoto {Name = partNames[0], Url = BlockBlob.Uri.ToString() });
                }
            };
            
            //
            var InvestmentBlockBlobs = ContainerInv.ListBlobs(prefix: investmentId.ToString(), useFlatBlobListing: true).OfType<CloudBlockBlob>();

            foreach (var BlockBlob in InvestmentBlockBlobs)
            {
                var partNames = BlockBlob.Name.Split('/').Reverse().ToArray();
                if (partNames[1].Contains("Picture"))
                {
                    try
                    {
                        FileList.Add(partNames[1], new SurveyPhoto { Name = partNames[0], Url = BlockBlob.Uri.ToString() });
                    }
                    catch (Exception)
                    {
                        
                    }
                }
            };

            return FileList;

        }
    }
}