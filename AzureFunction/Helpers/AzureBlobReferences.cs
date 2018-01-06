using AzureFunction.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureFunction.Helpers
{
    public class AzureBlobReferences
    {
        public static CloudBlockBlob DestinationBlob(BlobDetails details)
        {
            var destConnectionString = CloudConfigurationManager.GetSetting(details.Destination);
            var destStorageAccount =
                CloudStorageAccount.Parse(destConnectionString);
            var blobClient = destStorageAccount.CreateCloudBlobClient();
            var containerReference = blobClient.GetContainerReference(details.ContainerName);

            return containerReference.GetBlockBlobReference(details.BlobName);
        }

        public static CloudBlockBlob SourceBlob(BlobDetails details)
        {
            var sourceConnectionString = CloudConfigurationManager.GetSetting(details.Source);
            var sourceStorageAccount =
                CloudStorageAccount.Parse(sourceConnectionString);
            var blobClient = sourceStorageAccount.CreateCloudBlobClient();
            var containerReference = blobClient.GetContainerReference(details.ContainerName);

            return containerReference.GetBlockBlobReference(details.BlobName);
        }
    }
}
