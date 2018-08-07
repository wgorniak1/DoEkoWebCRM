using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.DoEko;
using DoEko.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using DoEko.Services;
using DoEko.ViewComponents.ViewModels;
using Microsoft.WindowsAzure.Storage.Blob;
using DoEko.Controllers.Extensions;
using System.IO;

namespace DoEko.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class ReportTemplatesController : Controller
    {
        private readonly DoEkoContext _context;
        private readonly IFileStorage _fileStorage;
        public ReportTemplatesController(DoEkoContext context, IFileStorage fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        [HttpPost]
        public async Task<IActionResult> Create(string templateName)
        {
            try
            {
                string TargetUrl = "";
                EnuAzureStorageContainerType containerType = EnuAzureStorageContainerType.Templates;

                if (Request.Form.Files.Count == 0)
                    return BadRequest();

                CloudBlobContainer Container = await _fileStorage.GetBlobContainerAsync(containerType);
                var file = Request.Form.Files.First();

                Stream stream = file.OpenReadStream();
                if (file.Length > 0)
                {
                    // InspectionSummary / template section / filename
                    OfficeTemplateType section = (OfficeTemplateType)Enum.Parse(typeof(OfficeTemplateType), file.Name);
                    string name = templateName + '/' + section.ToString() + '/' + file.FileName;
                    CloudBlockBlob blob = Container.GetBlockBlobReference(name);

                    await blob.UploadFromStreamAsync(file.OpenReadStream());

                    TargetUrl = blob.Uri.ToString();

                    return Ok(TargetUrl);
                }
                else
                    return BadRequest();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string templateName, OfficeTemplateType templateSection)
        {
            try
            {
                EnuAzureStorageContainerType containerType = EnuAzureStorageContainerType.Templates;

                var blobsToDelete = (await (await _fileStorage
                    .GetBlobContainerAsync(containerType))
                    .GetDirectoryReference(templateName)
                    .GetDirectoryReference(templateSection.ToString())
                    .ListBlobsAsync(new BlobContinuationToken()))
                    .OfType<CloudBlockBlob>();
                    
                //string fullname = templateName + '/' + templateSection.ToString();
                //var TemplateBlockBlobs = Container.ListBlobs(prefix: fullname, useFlatBlobListing: true).OfType<CloudBlockBlob>();

                foreach (var blob in blobsToDelete)
                {
                    await blob.DeleteIfExistsAsync();
                }
                
                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }
        }

        [HttpGet]
        public ActionResult List()
        {   
            //
            if (HttpContext.Request.IsAjaxRequest())
            {
                ReportTemplateViewModel model = new ReportTemplateViewModel();

                foreach (var item in this.GetTemplates("InspectionSummary"))
                {
                    model.Templates[item.Key] = item.Value;
                }
                //
                return Json(new { data = model.Templates.Select(t => new { Key = t.Key, KeyText = t.Key.DisplayName(), Name = t.Value.Name, Url = t.Value.Url }) });
            }
            else
            {
                return View();
            }
            
        }

        #region Private

        //private Dictionary<OfficeTemplateType, OfficeTemplate> GetTemplates(string templateCategory = null)
        private IEnumerable<KeyValuePair<OfficeTemplateType, OfficeTemplate>> GetTemplates(string templateCategory = null)
        {
            var TemplateBlockBlobs = _fileStorage
                .GetBlobContainerAsync(EnuAzureStorageContainerType.Templates)
                .GetAwaiter()
                .GetResult()
                .GetDirectoryReference(templateCategory)
                .ListBlobsAsync(true,BlobListingDetails.None,null,new BlobContinuationToken(),null,null)
                .GetAwaiter()
                .GetResult()
                .OfType<CloudBlockBlob>();

            //Dictionary<OfficeTemplateType, OfficeTemplate> TemplateList = new Dictionary<OfficeTemplateType, OfficeTemplate>();
            foreach (var BlockBlob in TemplateBlockBlobs)
            {
                var partNames = BlockBlob.Name.Split('/').Reverse().ToArray();
                yield return new KeyValuePair<OfficeTemplateType, OfficeTemplate>(
                    (OfficeTemplateType)Enum.Parse(typeof(OfficeTemplateType), partNames[1]), 
                    new OfficeTemplate { Name = partNames[0], Url = BlockBlob.Uri.ToString() });
            };
        }

        #endregion
    }
}
