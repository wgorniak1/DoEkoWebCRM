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
        public const string ReportName = "InspectionSummary";

        //public DoEkoContext _context;
        public IFileStorage _fileStorage;
        private readonly DoEkoContext _context;
        public InspectionSummaryBuilder(DoEkoContext context, IFileStorage fileStorage)
        {
            this._context = context;
            this._fileStorage = fileStorage;
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inv"></param>
        /// <param name="resultsFolder"></param>
        /// <returns></returns>
        //public Task<string> BuildAsync(InvestmentViewModel inv, string resultsFolder)
        //{
        //    Task<string> task = Task.Factory.StartNew( () => (Build(inv, resultsFolder)));
        //    return task;
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inv">Investment for which data will be generated</param>
        /// <param name="resultsFolder">folder to place generated document</param>
        /// <returns>URL of the generated document</returns>
        public Task BuildAsync(InvestmentViewModel inv, string resultsFolder)
        {
            //1. Initialize:
            inv.ReadPictures(_fileStorage);

            //1. Title section is always populated
            Stream MainStream = this.GetTemplate(ReportName, OfficeTemplateType.Title);

            WordprocessingDocument doc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(stream: MainStream, isEditable: true);
            doc = doc.MergeFields(inv).MergePictures(inv);

            doc.MainDocumentPart.Document.Save();

            List<Task<Stream>> docParts = new List<Task<Stream>>();
            foreach (OfficeTemplateType type in Enum.GetValues(typeof(OfficeTemplateType)))
            {
                docParts.Add(this.BuildPartAsync(inv, type));
            }

            //Wait untill all parts are generated
            Task.WaitAll(docParts.ToArray());

            //Merge all parts
            foreach (var part in docParts)
            {
                doc = part.Result != Stream.Null ? doc.MergeStream(part.Result) : doc;
            }

            //Save & Close main document, reset stream position for downloading it to azure storage
            doc.Dispose();

            //Save data into file

            
            var blob = _fileStorage
                .GetBlobContainer(EnuAzureStorageContainerType.ReportResults)
                .GetBlockBlobReference(CreateFileName(resultsFolder, inv));

            MainStream.Position = 0;
            return blob.UploadFromStreamAsync(MainStream);
        }

        public string CreateFileName( string folder, InvestmentViewModel inv)
        {
            return string.Join("/", ReportName, folder,
                                string.Join("_", inv.FirstOwner.FullName,
                                inv.InvestmentId.ToString())) + ".docm";

            //string.Join("_",inv.FirstOwner.FullName,
            //                inv.Address.City,
            //                inv.Address.Street,
            //                inv.Address.BuildingNo,
            //                inv.Address.ApartmentNo)) + ".docm";
        }

        private Stream GetTemplate(string templateType, OfficeTemplateType templateSection)
        {
            try
            {
                var templates = _fileStorage.GetBlobContainer(EnuAzureStorageContainerType.Templates);
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

        private Stream BuildPart(InvestmentViewModel inv, OfficeTemplateType type)
        {
            try
            {
                switch (type)
                {
                    case OfficeTemplateType.PhotoVoltaic:
                        inv.Survey = inv.Surveys.First(s => s.Status != SurveyStatus.Cancelled &&
                                                            s.GetRSEType() == (int)SurveyRSETypeEnergy.PhotoVoltaic);
                        break;
                    case OfficeTemplateType.Solar:
                        inv.Survey = inv.Surveys.First(s => s.Status != SurveyStatus.Cancelled &&
                                                            s.Type == SurveyType.HotWater &&
                                                            s.GetRSEType() == (int)SurveyRSETypeHotWater.Solar);
                        break;
                    case OfficeTemplateType.HWHeatPump:
                        inv.Survey = inv.Surveys.First(s => s.Status != SurveyStatus.Cancelled &&
                                                            s.Type == SurveyType.HotWater &&
                                                            s.GetRSEType() == (int)SurveyRSETypeHotWater.HeatPump);
                        break;
                    case OfficeTemplateType.CHHeatPump:
                        inv.Survey = inv.Surveys.First(s => s.Status != SurveyStatus.Cancelled &&
                                                            s.Type == SurveyType.CentralHeating &&
                                                            s.GetRSEType() == (int)SurveyRSETypeCentralHeating.HeatPump);
                        break;
                    case OfficeTemplateType.HeatPumpAir:
                        inv.Survey = inv.Surveys.First(s => s.Status != SurveyStatus.Cancelled &&
                                                            s.Type == SurveyType.CentralHeating && 
                                                            s.GetRSEType() == (int)SurveyRSETypeCentralHeating.HeatPumpAir);
                        break;
                    case OfficeTemplateType.PelletBoiler:
                        inv.Survey = inv.Surveys.First(s => s.Status != SurveyStatus.Cancelled && 
                                                            s.Type == SurveyType.CentralHeating &&
                                                            s.GetRSEType() == (int)SurveyRSETypeCentralHeating.PelletBoiler);
                        break;
                    default:
                        return Stream.Null;
                }
            }
            catch (Exception)
            {
                //NOT FOUND
                return Stream.Null;
            }

            try
            {
                RSEPriceHelper rsePrice = new RSEPriceHelper(this._context, inv.Survey);

                inv.Survey.ResultCalculation.RSENetPrice = Decimal.ToDouble(rsePrice.Net);
                inv.Survey.ResultCalculation.RSETax = Decimal.ToDouble(rsePrice.Tax);
                inv.Survey.ResultCalculation.RSEGrossPrice = Decimal.ToDouble(rsePrice.Gross);
                inv.Survey.ResultCalculation.RSEOwnerContrib = Decimal.ToDouble(rsePrice.OwnerContribution);
                ////inv.Survey.ResultCalculation.RSEOwnerContrib = (100 - 60) * inv.Survey.ResultCalculation.RSENetPrice / 100 + inv.Survey.ResultCalculation.RSETax;
                //inv.Survey.ResultCalculation.RSEOwnerContrib = inv.Contract.Project.GrossNetFundsType ? 
                //    (100 - inv.Contract.Project.UEFundsLevel) * inv.Survey.ResultCalculation.RSEGrossPrice / 100 :
                //    (100 - inv.Contract.Project.UEFundsLevel) * inv.Survey.ResultCalculation.RSENetPrice / 100;
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception exc)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                
            }


            Stream partStream = this.GetTemplate("InspectionSummary", type);

            WordprocessingDocument tmpDoc = WordprocessingDocument.Open(stream: partStream, isEditable: true);
            tmpDoc
                .MergeFields(inv)
                .MergePictures(inv)
                .Dispose();

            partStream.Position = 0;

            return partStream;
        }

        private Task<Stream> BuildPartAsync(InvestmentViewModel inv, OfficeTemplateType type)
        {
            Task<Stream> task = Task.Factory.StartNew(() => BuildPart(new InvestmentViewModel(inv), type));
            return task;
        }
    }
}
