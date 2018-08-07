using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DoEko.Services
{
    public static class CloudBlobDirectoryExtensions
    {
        public static async Task<List<IListBlobItem>> ListBlobsAsync(this CloudBlobDirectory cloudBlobDirectory, bool useFlatBlobListing, BlobListingDetails blobListingDetails, int? maxResults, BlobContinuationToken currentToken, BlobRequestOptions options, OperationContext operationContext, CancellationToken cancellationToken)
        {
            List<IListBlobItem> results = new List<IListBlobItem>();
            do
            {
                var response = await cloudBlobDirectory.ListBlobsSegmentedAsync(useFlatBlobListing, blobListingDetails, maxResults, currentToken, options, operationContext, cancellationToken);
                currentToken = response.ContinuationToken;
                results.AddRange(response.Results);

                if (cancellationToken.IsCancellationRequested)
                {
                    results.Clear();
                    break;
                }
            }
            while (currentToken != null);
            return results;

        }
        public static async Task<List<IListBlobItem>> ListBlobsAsync(this CloudBlobDirectory cloudBlobDirectory, bool useFlatBlobListing, BlobListingDetails blobListingDetails, int? maxResults, BlobContinuationToken currentToken, BlobRequestOptions options, OperationContext operationContext)
        {
            return await cloudBlobDirectory.ListBlobsAsync(useFlatBlobListing, blobListingDetails, maxResults, currentToken, options, operationContext, CancellationToken.None);
        }
        public static async Task<List<IListBlobItem>> ListBlobsAsync(this CloudBlobDirectory cloudBlobDirectory, BlobContinuationToken currentToken)
        {
            return await cloudBlobDirectory.ListBlobsAsync(false, BlobListingDetails.None, null, currentToken, null, null, CancellationToken.None);
        }
    }
}
