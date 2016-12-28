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
        public IActionResult Upload(enuAzureStorageContainerType Type, int? ID, Guid? Guid, string ReturnUrl = null)
        {
            
            //CloudBlobContainer Container = _azureStorage.GetBlobContainer(Type);
            CloudBlobContainer Container = _fileStorage.GetBlobContainer(Type);
            List<string> blobs = new List<string>();
            foreach (var blobItem in Container.ListBlobs())
            {
                blobs.Add(blobItem.Uri.ToString());
            }

            if (ID != null)
            {
                ViewData["Id"] = ID.ToString();
            }
            else if (Guid != null)
            {
                ViewData["Id"] = Guid.ToString();
            }
            return View(blobs);

        }

//        [HttpPost]
        public JsonResult Upload(enuAzureStorageContainerType Type, int? Id, Guid? Guid,
                                 FormCollection Form, string ReturnUrl = null)
        {
            //CloudBlobContainer Container = _azureStorage.GetBlobContainer(Type);
            CloudBlobContainer Container = _fileStorage.GetBlobContainer(Type);
            string Key = "Not assigned";
            if (Id != null)
            {
                Key = Id.ToString();
            }
            else if (Guid != null)
            {
                Key = Guid.ToString();
            }

            foreach (var file in Request.Form.Files)
            {
                Stream stream = file.OpenReadStream();
                if (file.Length == 0)
                    continue;

                if (file.Length > 0)
                {
                    string name = Key + '/' + file.FileName;
                    CloudBlockBlob blob = Container.GetBlockBlobReference(name);
                    blob.UploadFromStream(file.OpenReadStream());
                }
            }
            //if (true)
            //{

            //}
            return Json("OK");
        }

        [HttpPost]
        public IActionResult UploadPhoto(enuAzureStorageContainerType type, Guid guid)
        {
            string TargetUrl = "";

            if (guid == Guid.Empty)
            {
                ModelState.AddModelError("guid", "Nr Ankiety nie mo�e by� pusty");
                return BadRequest(ModelState);
            }
            if (Request.Form.Files.Count == 0)
                return Ok(TargetUrl);
            
            CloudBlobContainer Container = _fileStorage.GetBlobContainer(type);
            var file = Request.Form.Files.First();
            //foreach (var file in Request.Form.Files)
            //{
            Stream stream = file.OpenReadStream();
            if (file.Length > 0)
            {
                // surveyId / Picture0 / filename
                string name = guid.ToString() + '/' + file.Name + '/' + file.FileName;
                CloudBlockBlob blob = Container.GetBlockBlobReference(name);
                blob.UploadFromStream(file.OpenReadStream());

                TargetUrl = blob.Uri.ToString();
            }
            //}
            return Ok(TargetUrl);
        }

        [HttpPost]
        public IActionResult DeletePhoto(enuAzureStorageContainerType type, Guid guid, string pictureId, string fileName)
        {
            CloudBlobContainer Container = _fileStorage.GetBlobContainer(type);
            string RootKey = guid.ToString();

            string fullname = RootKey + '/' + pictureId + '/' + fileName;
            try
            {
                CloudBlockBlob blob = Container.GetBlockBlobReference(fullname);

                blob.Delete();

                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }

        }

        public JsonResult Delete(enuAzureStorageContainerType Type, int? Id, Guid? Guid,
                                         string Name, string ReturnUrl = null)
        {
            //CloudBlobContainer Container = _azureStorage.GetBlobContainer(Type);
            CloudBlobContainer Container = _fileStorage.GetBlobContainer(Type);
            string Key = "Not assigned";
            if (Id != null)
            {
                Key = Id.ToString();
            }
            else if (Guid != null)
            {
                Key = Guid.ToString();
            }
            
            string fullname = Key + '/' + Name;
            CloudBlockBlob blob = Container.GetBlockBlobReference(fullname);
            try
            {
                blob.Delete();

                return Json("OK");
            }
            catch (Microsoft.WindowsAzure.Storage.StorageException)
            {
                return Json("Error");
            }
        }

        private IList<Models.DoEko.File> Files(enuAzureStorageContainerType Type, string Key)
        {
            //CloudBlobContainer Container = _azureStorage.GetBlobContainer(Type);
            CloudBlobContainer Container = _fileStorage.GetBlobContainer(Type);
            var ContainerBlockBlobs = Container.ListBlobs(prefix: Key, useFlatBlobListing: true).OfType<CloudBlockBlob>();

            List<Models.DoEko.File> FileList = new List<Models.DoEko.File>();

            foreach (var BlockBlob in ContainerBlockBlobs)
            {
                FileList.Add(new Models.DoEko.File
                {
                    ParentType = Type.ToString(),
                    //ParentId = Key,
                    //ProjectId = Key,
                    Name = BlockBlob.Uri.Segments.Last(),
                    ChangedAt = BlockBlob.Properties.LastModified.Value.LocalDateTime,
                    Url = BlockBlob.Uri.ToString()
                });
            };
            return FileList;

        }
    }
}