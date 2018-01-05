using System;
using System.Collections.Generic;
using System.Linq;
using AzureFunction.Helpers;
using AzureFunction.Models;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace AzureFunction
{
    public static class GetBlobs
    {
        [FunctionName("GetBlobs")]
        public static void Run([QueueTrigger("retrieve-blob", Connection = "")]string containerDetails, TraceWriter log)
        {
            var details = JsonConvert.DeserializeObject<ContainerDetails>(containerDetails);

            var blobs = RetrieveBlobs(details);

            foreach (var blobDetails in blobs)
            {
                AddMessage.Send(blobDetails, "copy-blob");
            }
        }

        private static List<string> RetrieveBlobs(ContainerDetails details)
        {
            var productionConnectionString = CloudConfigurationManager.GetSetting(details.Production);
            var productionStorageAccount =
                CloudStorageAccount.Parse(productionConnectionString);
            var blobClient = productionStorageAccount.CreateCloudBlobClient();
            var containerReference = blobClient.GetContainerReference(details.ContainerName);

            return  containerReference.ListBlobs().Cast<CloudBlob>().Select(x => JsonConvert.SerializeObject(new BlobDetails
            {
                Production = details.Production,
                Backup = details.Backup,
                ContainerName = details.ContainerName,
                BlobName = x.Name,
                BlobUri = x.Uri
            })).ToList();
        }
    }
}
