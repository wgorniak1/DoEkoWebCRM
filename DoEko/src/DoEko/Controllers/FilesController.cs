using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using DoEko.Services;

namespace DoEko.Controllers
{
    [Authorize()]
    public class FilesController : Controller
    { 
        public FilesController(IFileStorage fileStorage)
        {
            //    _azureStorage = new AzureStorage(Configuration.GetConnectionString("doekostorage_AzureStorageConnectionString"));
            _fileStorage = fileStorage;
        }

        // private AzureStorage _azureStorage;
        private IFileStorage _fileStorage;

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Upload(EnuAzureStorageContainerType Type, int? ID, Guid? Guid, string ReturnUrl = null)
        {
            CloudBlobContainer cloudBlobContainer = await _fileStorage.GetBlobContainerAsync(Type);
            
            var model = (await cloudBlobContainer.ListBlobsAsync(new BlobContinuationToken())).Select(i => i.Uri.AbsoluteUri).ToList();

            ViewData["Id"] = ID?.ToString() ?? Guid?.ToString();
            ViewData["ReturnURL"] = ReturnUrl;

            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Upload(EnuAzureStorageContainerType Type, int? Id, Guid? Guid, IFormCollection Form, string ReturnUrl = null)
        {
            CloudBlobContainer cloudBlobContainer = await _fileStorage.GetBlobContainerAsync(Type);

            string Key = Id?.ToString() ?? Guid?.ToString() ?? "NoID";

            foreach (var file in Request.Form.Files.Where(f => f.Length > 0))
            {
                string blobName = Key + '/' + file.FileName;

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);

                await cloudBlockBlob.UploadFromStreamAsync(file.OpenReadStream());

            }

            return Ok("OK");
        }

        [HttpPost]
        public async Task<IActionResult> UploadPhoto(EnuAzureStorageContainerType type, Guid guid)
        {

            if (guid == Guid.Empty)
                ModelState.AddModelError("GuID", "Nr Ankiety nie może być pusty");

            if (Request.Form.Files.Count != 1)
                ModelState.AddModelError("File", "Proszę wskazać plik ze zdjęciem");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            CloudBlobContainer Container = await _fileStorage.GetBlobContainerAsync(type);
            var file = Request.Form.Files.First();
            
            if (file.Length > 0)
            {
                // surveyId / Picture0 / filename
                string blobName = guid.ToString() + '/' + file.Name + '/' + file.FileName;

                CloudBlockBlob cloudBlockBlob = Container.GetBlockBlobReference(blobName);

                await cloudBlockBlob.UploadFromStreamAsync(file.OpenReadStream());
                //cloudBlockBlob.Properties.ContentType = file.ContentType;
                cloudBlockBlob.Properties.ContentType = this.GetFileContentType(file.FileName);

                await cloudBlockBlob.SetPropertiesAsync();

                return Ok(cloudBlockBlob.Uri.AbsoluteUri);
            }
            else
            {
                return Ok();
            }

        }

        private string GetFileContentType(string fileName)
        {
            string ContentType = String.Empty;
            string Extension = Path.GetExtension(fileName).ToLower();

            switch (Extension)
            {
                case ".pdf":
                    ContentType = "application/pdf";
                    break;
                case ".txt":
                    ContentType = "text/plain";
                    break;
                case ".bmp":
                    ContentType = "image/bmp";
                    break;
                case ".gif":
                    ContentType = "image/gif";
                    break;
                case ".png":
                    ContentType = "image/png";
                    break;
                case ".jpg":
                    ContentType = "image/jpeg";
                    break;
                case ".jpeg":
                    ContentType = "image/jpeg";
                    break;
                case ".xls":
                    ContentType = "application/vnd.ms-excel";
                    break;
                case ".xlsx":
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case ".csv":
                    ContentType = "text/csv";
                    break;
                case ".html":
                    ContentType = "text/html";
                    break;
                case ".xml":
                    ContentType = "text/xml";
                    break;
                case ".zip":
                    ContentType = "application/zip";
                    break;
                default:
                    ContentType = "application/octet-stream";
                    break;
            }

            return ContentType;
        }

        [HttpPost]
        public async Task<IActionResult> DeletePhoto(EnuAzureStorageContainerType type, Guid guid, string pictureId, string fileName)
        {
            try
            {
                CloudBlobContainer Container = await _fileStorage.GetBlobContainerAsync(type);
            
                string fullname = guid.ToString() + '/' + pictureId + '/' + fileName;

                CloudBlockBlob blob = Container.GetBlockBlobReference(fullname);

                await blob.DeleteIfExistsAsync();

                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }

        }

        [HttpPost]
        public async Task<JsonResult> Delete(EnuAzureStorageContainerType Type, int? Id, Guid? Guid, string Name, string ReturnUrl = null)
        {
            CloudBlobContainer Container = await _fileStorage.GetBlobContainerAsync(Type);

            string blobName = ( Id?.ToString() ?? Guid?.ToString() ?? "NoID" ) + '/' + Name;
            CloudBlockBlob cloudBlobkBlob = Container.GetBlockBlobReference(blobName);
            try
            {
                await cloudBlobkBlob.DeleteIfExistsAsync();

                return Json("OK");
            }
            catch (Microsoft.WindowsAzure.Storage.StorageException)
            {
                return Json("Error");
            }
        }

        private async Task<IList<Models.DoEko.File>> Files(EnuAzureStorageContainerType Type, string Key)
        {
            CloudBlobContainer Container = await _fileStorage.GetBlobContainerAsync(Type);

            var ContainerBlockBlobs = await Container.ListBlobsAsync(Key, true, BlobListingDetails.None, null, new BlobContinuationToken(), null, null);

            var fileList = ContainerBlockBlobs
                .OfType<CloudBlockBlob>()
                .Select(b => new Models.DoEko.File {
                    ParentType = Type.ToString(),
                    Name = b.Uri.Segments.Last(),
                    ChangedAt = b.Properties.LastModified.Value.LocalDateTime,
                    Url = b.Uri.AbsoluteUri })
                .ToList();

            return fileList;

        }
    }
}