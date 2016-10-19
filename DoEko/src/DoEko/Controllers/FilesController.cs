using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using Microsoft.Extensions.Configuration;
using DoEko.Models.DoEko;
using Microsoft.WindowsAzure.Storage;

namespace DoEko.Controllers
{
    public class FilesController : Controller
    { 
        public FilesController(IConfiguration Configuration)
        {
            _azureStorage = new AzureStorage(Configuration.GetConnectionString("doekostorage_AzureStorageConnectionString"));
        }

        private AzureStorage _azureStorage;

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Upload(enuAzureStorageContainerType Type, int? ID, Guid? Guid, string ReturnUrl = null)
        {
            
            CloudBlobContainer Container = _azureStorage.GetBlobContainer(Type);
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
            CloudBlobContainer Container = _azureStorage.GetBlobContainer(Type);
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

        public JsonResult Delete(enuAzureStorageContainerType Type, int? Id, Guid? Guid,
                                         string Name, string ReturnUrl = null)
        {
            CloudBlobContainer Container = _azureStorage.GetBlobContainer(Type);
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
            CloudBlobContainer Container = _azureStorage.GetBlobContainer(Type);
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