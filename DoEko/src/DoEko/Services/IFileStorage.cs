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
        Task<CloudBlobContainer> GetBlobContainerAsync(EnuAzureStorageContainerType ContainerType);
        void Upload(IFormFile File, EnuAzureStorageContainerType Type, string Key = "Not assigned");
        Task<bool> DeleteFolderAsync(EnuAzureStorageContainerType ContainerType, string FolderName, BlobContinuationToken blobContinuationToken);
    }
}
