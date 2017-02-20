using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Services
{
    public interface IFileStorage
    {
        CloudBlobContainer GetBlobContainer(enuAzureStorageContainerType ContainerType);
        void Upload(IFormFile File, enuAzureStorageContainerType Type, string Key = "Not assigned");
        Task<bool> DeleteFolderAsync(enuAzureStorageContainerType ContainerType, string FolderName);
    }
}
