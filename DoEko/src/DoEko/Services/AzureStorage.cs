using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Options;
using DoEko.Controllers.Settings;

namespace DoEko.Services
{
    public enum EnuAzureStorageContainerType
    {
        Project,
        Contract,
        Investment,
        Survey,
        Templates,
        ReportResults,
        NeoDownloads
    }

    public class AzureStorage : IFileStorage
    {
        private readonly CloudStorageAccount Account;

        public AzureStorage(IOptions<AppSettings> options)
        {
            Account = CloudStorageAccount.Parse(options.Value.AzureStorageOptions.ConnectionString);
        }

        public async Task<CloudBlobContainer> GetBlobContainerAsync(EnuAzureStorageContainerType ContainerType)
        {
            string ContainerName = ContainerType.ToString().ToLower();// + "/" + Id.ToString();

            CloudBlobClient BlobClient = Account.CreateCloudBlobClient();
            CloudBlobContainer container = BlobClient.GetContainerReference(ContainerName);
            bool result = await container.CreateIfNotExistsAsync();//BlobContainerPublicAccessType.Blob, new BlobRequestOptions() { }, new OperationContext() { });

            return container;
        }

        public void Upload(IFormFile File, EnuAzureStorageContainerType Type, string Key = "Not assigned")
        {
            CloudBlobContainer Container = GetBlobContainerAsync(Type).GetAwaiter().GetResult();
            if (File.Length > 0)
            {
                string BlobName = Key + '/' + File.FileName;

                Stream stream = File.OpenReadStream();
                CloudBlockBlob BlockBlob = Container.GetBlockBlobReference(BlobName);

                BlockBlob.UploadFromStreamAsync(stream).GetAwaiter().GetResult();

                stream.Close();
            }
        }

        public async Task<bool> DeleteFolderAsync(EnuAzureStorageContainerType ContainerType, string FolderName, BlobContinuationToken blobContinuationToken = null)
        {
            CloudBlobContainer container = await GetBlobContainerAsync(ContainerType);

            var l = await container.GetDirectoryReference(FolderName.ToLower()).ListBlobsAsync(blobContinuationToken);

            foreach (IListBlobItem blob in l)
            {
                if (blob.GetType() == typeof(CloudBlob) || blob.GetType().BaseType == typeof(CloudBlob))
                {
                    await ((CloudBlob)blob).DeleteAsync();
                }
            }

            return true;
            
        }

    }
}
