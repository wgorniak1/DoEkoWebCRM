using DocumentFormat.OpenXml.Packaging;
using DoEko.Controllers.Extensions;
using DoEko.Models.DoEko;
using DoEko.Models.DoEko.Survey;
using DoEko.Services;
using DoEko.ViewComponents.ViewModels;
using DoEko.ViewModels.ReportsViewModels;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Controllers.Helpers
{
    public class InspectionSummaryBuilder
    {
        public DoEkoContext _context;
        public IFileStorage _fileStorage;

        public InspectionSummaryBuilder(DoEkoContext context, IFileStorage fileStorage)
        {
            this._context = context;
            this._fileStorage = fileStorage;
        }

        public CloudBlockBlob Build(InvestmentViewModel inv)
        {
            //1. Initialize:
            inv.ReadPictures(_fileStorage);

            //1. Title section is always populated
            Stream MainStream = this.GetTemplate("InspectionSummary", OfficeTemplateType.Title);

            WordprocessingDocument doc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(stream: MainStream, isEditable: true);
            doc = doc.MailMerge(inv).MergePictures(inv);

            doc.MainDocumentPart.Document.Save();


            //2. PV section
            if (inv.Surveys.Any(s => s.Status != SurveyStatus.Cancelled && s.GetRSEType() == (int)SurveyRSETypeEnergy.PhotoVoltaic))
            {
                using (Stream StreamToMerge = this.GetTemplate("InspectionSummary", OfficeTemplateType.PhotoVoltaic))
                {
                    inv.Survey = inv.Surveys.First(s => s.GetRSEType() == (int)SurveyRSETypeEnergy.PhotoVoltaic && s.Status != SurveyStatus.Cancelled);

                    WordprocessingDocument tmpDoc = WordprocessingDocument.Open(stream: StreamToMerge, isEditable: true);
                    tmpDoc.MailMerge(inv)
                        .MergePictures(inv)
                        .Dispose();

                    StreamToMerge.Position = 0;
                    doc = doc.MergeStream(StreamToMerge);
                }
            }

            //3. Ground heat pump
            if (inv.Surveys.Any(s => s.Status != SurveyStatus.Cancelled && s.GetRSEType() == (int)SurveyRSETypeCentralHeating.HeatPump))
            {
                using (Stream StreamToMerge = this.GetTemplate("InspectionSummary", OfficeTemplateType.HeatPump))
                {
                    inv.Survey = inv.Surveys.First(s => s.GetRSEType() == (int)SurveyRSETypeCentralHeating.HeatPump && s.Status != SurveyStatus.Cancelled);

                    WordprocessingDocument tmpDoc = WordprocessingDocument.Open(stream: StreamToMerge, isEditable: true);
                    tmpDoc.MailMerge(inv)
                        .MergePictures(inv)
                        .Dispose();

                    StreamToMerge.Position = 0;
                    doc = doc.MergeStream(StreamToMerge);
                }
            }

            //4. Air Heat Pump
            if (inv.Surveys.Any(s => s.Status != SurveyStatus.Cancelled &&  ( s.Type == SurveyType.CentralHeating && s.GetRSEType() == (int)SurveyRSETypeCentralHeating.HeatPumpAir ) ||
                                                                            ( s.Type == SurveyType.HotWater && s.GetRSEType() == (int)SurveyRSETypeHotWater.HeatPump))
                                                                           )
            {
                using (Stream StreamToMerge = this.GetTemplate("InspectionSummary", OfficeTemplateType.HeatPumpAir))
                {
                    inv.Survey = inv.Surveys.First(s => s.Status != SurveyStatus.Cancelled && 
                    ( s.Type == SurveyType.CentralHeating && s.GetRSEType() == (int)SurveyRSETypeCentralHeating.HeatPumpAir ) ||
                    ( s.Type == SurveyType.HotWater && s.GetRSEType() == (int)SurveyRSETypeHotWater.HeatPump));

                    WordprocessingDocument tmpDoc = WordprocessingDocument.Open(stream: StreamToMerge, isEditable: true);
                    tmpDoc.MailMerge(inv)
                        .MergePictures(inv)
                        .Dispose();

                    StreamToMerge.Position = 0;
                    doc = doc.MergeStream(StreamToMerge);
                }
            }

            //5. Boiler
            if (inv.Surveys.Any(s => s.Status != SurveyStatus.Cancelled && s.GetRSEType() == (int)SurveyRSETypeCentralHeating.PelletBoiler))
            {
                using (Stream StreamToMerge = this.GetTemplate("InspectionSummary", OfficeTemplateType.PelletBoiler))
                {
                    inv.Survey = inv.Surveys.First(s => s.GetRSEType() == (int)SurveyRSETypeCentralHeating.PelletBoiler && s.Status != SurveyStatus.Cancelled);

                    WordprocessingDocument tmpDoc = WordprocessingDocument.Open(stream: StreamToMerge, isEditable: true);
                    tmpDoc.MailMerge(inv)
                        .MergePictures(inv)
                        .Dispose();

                    StreamToMerge.Position = 0;
                    doc = doc.MergeStream(StreamToMerge);
                }
            }

            //6. Solar
            if (inv.Surveys.Any(s => s.Status != SurveyStatus.Cancelled && s.GetRSEType() == (int)SurveyRSETypeHotWater.Solar))
            {
                using (Stream StreamToMerge = this.GetTemplate("InspectionSummary", OfficeTemplateType.Solar))
                {
                    inv.Survey = inv.Surveys.First(s => s.GetRSEType() == (int)SurveyRSETypeHotWater.Solar && s.Status != SurveyStatus.Cancelled);

                    WordprocessingDocument tmpDoc = WordprocessingDocument.Open(stream: StreamToMerge, isEditable: true);
                    tmpDoc.MailMerge(inv)
                        .MergePictures(inv)
                        .Dispose();

                    StreamToMerge.Position = 0;
                    doc = doc.MergeStream(StreamToMerge);
                }
            }

            //Save & Close main document, reset stream position for downloading it to azure storage
            doc.Dispose();
            MainStream.Position = 0;

            //Save data into file
            var documents = _fileStorage.GetBlobContainer(enuAzureStorageContainerType.Templates);
            var targetName = "Results/InspectionSummary/" + inv.InvestmentId+'_'+DateTime.Now.ToString("yyyyMMddHHmmss") + ".docx";
            var targetblob = documents.GetBlockBlobReference(targetName);
            targetblob.UploadFromStream(MainStream);

            //return Uri
            return targetblob;

        }

        private Stream GetTemplate(string templateType, OfficeTemplateType templateSection)
        {
            try
            {
                var templates = _fileStorage.GetBlobContainer(enuAzureStorageContainerType.Templates);
                var template = templates.GetDirectoryReference(templateType);
                var templateDoc = template.GetDirectoryReference(templateSection.ToString()).ListBlobs().OfType<CloudBlockBlob>().First();

                Stream stream = new MemoryStream();
                templateDoc.DownloadToStream(stream);

                return stream;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
