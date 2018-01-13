using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AzureFunction.Helpers;
using AzureFunction.Models;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace AzureFunction.Fuctions
{
    public static class GetBlobsFunction
    {
        [FunctionName("GetBlobsFunction")]
        public static void Run([QueueTrigger("retrieve-blob")]string containerDetails, TraceWriter log)
        {
            var telemetry = AppInsightClient.Telemetry;

            var details = JsonConvert.DeserializeObject<ContainerDetails>(containerDetails);

            try
            {
                telemetry.TrackTrace($"Retrieve Blobs for Container: {details.ContainerName}");

                var blobs = RetrieveBlobs(details);

                foreach (var blobDetails in blobs)
                {
                    AddMessage.Send(blobDetails, "copy-blob");
                }
            }
            catch (Exception e)
            {
                telemetry.TrackTrace($"ERROR: Cannot retrieve blobs for container: {details.ContainerName}, exception: {e}");
            }
        }

        private static List<string> RetrieveBlobs(ContainerDetails details)
        {
            var sourceConnectionString = CloudConfigurationManager.GetSetting(details.Source);
            var sourceStorageAccount =
                CloudStorageAccount.Parse(sourceConnectionString);
            var blobClient = sourceStorageAccount.CreateCloudBlobClient();
            var containerReference = blobClient.GetContainerReference(details.ContainerName);
            var blobs = containerReference.ListBlobs().Cast<CloudBlob>().Select(x => JsonConvert.SerializeObject(new BlobDetails
            {
                Source = details.Source,
                Destination = details.Destination,
                ContainerName = details.ContainerName,
                BlobName = x.Name
            })).ToList();

            blobs.Add(JsonConvert.SerializeObject(new BlobDetails
            {
                Source = details.Source,
                Destination = details.Destination,
                ContainerName = details.ContainerName,
                BlobName = "blob404"
            }));

            return blobs;

        }
    }
}
