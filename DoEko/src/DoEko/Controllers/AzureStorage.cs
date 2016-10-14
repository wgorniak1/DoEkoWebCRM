using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using Microsoft.AspNetCore.Http;
using System.IO;

namespace DoEko.Controllers
{
    public enum enuAzureStorageContainerType
    {
        Project,
        Contract,
        Investment,
        Survey
    }
    public class AzureStorage
    {
        public CloudStorageAccount Account { get; set; }
        public AzureStorage(string connectionString)
        {
            Account = CloudStorageAccount.Parse( connectionString );

        }

        public CloudBlobContainer GetBlobContainer(enuAzureStorageContainerType ContainerType)
        {
            string ContainerName = ContainerType.ToString().ToLower();// + "/" + Id.ToString();

            CloudBlobClient BlobClient = Account.CreateCloudBlobClient();
            CloudBlobContainer container = BlobClient.GetContainerReference(ContainerName);
            container.CreateIfNotExists( accessType: BlobContainerPublicAccessType.Blob);

            return container;
            
        }

        public void UploadAsync(IFormFile File, enuAzureStorageContainerType Type, string Key = "Not assigned")
        {
            CloudBlobContainer Container = GetBlobContainer(Type);
            if (File.Length > 0)
            {
                string BlobName = Key + '/' + File.FileName;

                Stream stream = File.OpenReadStream();
                CloudBlockBlob BlockBlob = Container.GetBlockBlobReference(BlobName);

                BlockBlob.UploadFromStream(stream);
                //if (BlockBlob.Properties.LeaseState)
                //{
                //return true;
                //}
                stream.Close();
            }
            //else
                //return false;
        }
    }
}
