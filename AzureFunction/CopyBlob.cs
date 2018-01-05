using System.IO;
using System.Net;
using AzureFunction.Models;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;

namespace AzureFunction
{
    public static class CopyBlob
    {
        [FunctionName("CopyBlob")]
        public static void Run([QueueTrigger("copy-blob")]string blobItem, TraceWriter log)
        {
            var details = JsonConvert.DeserializeObject<BlobDetails>(blobItem);

            MoveBlobs(details);
        }

        private static void MoveBlobs(BlobDetails details)
        {
            var backupConnectionString = CloudConfigurationManager.GetSetting(details.Backup);
            var backupStorageAccount =
                CloudStorageAccount.Parse(backupConnectionString);
            var blobClient = backupStorageAccount.CreateCloudBlobClient();
            var containerReference = blobClient.GetContainerReference(details.ContainerName);

            var blockBlob = containerReference.GetBlockBlobReference(details.BlobName);

            var webClient = new WebClient();
            using (var stream = new MemoryStream(webClient.DownloadData(details.BlobUri)))
            {
                blockBlob.UploadFromStream(stream);
            }
        }
    }
}
