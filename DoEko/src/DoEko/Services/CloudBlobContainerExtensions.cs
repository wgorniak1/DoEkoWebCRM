using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DoEko.Services
{
    public static class CloudBlobContainerExtensions
    {

        public static async Task<List<IListBlobItem>> ListBlobsAsync(this CloudBlobContainer cloudBlobContainer, string prefix, bool useFlatBlobListing, BlobListingDetails blobListingDetails, int? maxResults, BlobContinuationToken currentToken, BlobRequestOptions options, OperationContext operationContext, CancellationToken cancellationToken)
        {
            List<IListBlobItem> results = new List<IListBlobItem>();
            do
            {
                var response = await cloudBlobContainer.ListBlobsSegmentedAsync(prefix, useFlatBlobListing, blobListingDetails, maxResults, currentToken, options, operationContext, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    results.Clear();
                    break;
                }

                currentToken = response.ContinuationToken;

                results.AddRange(response.Results);
                

            }
            while (currentToken != null);

            return results;

        }
        public static async Task<List<IListBlobItem>> ListBlobsAsync(this CloudBlobContainer cloudBlobContainer, string prefix, bool useFlatBlobListing, BlobListingDetails blobListingDetails, int? maxResults, BlobContinuationToken currentToken, BlobRequestOptions options, OperationContext operationContext)
        {
            return await cloudBlobContainer.ListBlobsAsync(prefix, useFlatBlobListing, blobListingDetails, maxResults, currentToken, options, operationContext, CancellationToken.None);
        }
        public static async Task<List<IListBlobItem>> ListBlobsAsync(this CloudBlobContainer cloudBlobContainer, string prefix, BlobContinuationToken currentToken)
        {
            return await cloudBlobContainer.ListBlobsAsync(prefix, false, BlobListingDetails.None, null, currentToken, null, null, CancellationToken.None);

        }
        public static async Task<List<IListBlobItem>> ListBlobsAsync(this CloudBlobContainer cloudBlobContainer, BlobContinuationToken currentToken)
        {

            return await cloudBlobContainer.ListBlobsAsync(string.Empty, false, BlobListingDetails.None, null, currentToken, null, null, CancellationToken.None);

        }

    }
}
