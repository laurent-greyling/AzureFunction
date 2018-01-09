using System;
using System.Diagnostics;
using AzureFunction.Helpers;
using AzureFunction.Models;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;

namespace AzureFunction.Fuctions
{
    public static class CreateContainerFunction
    {
        [FunctionName("CreateContainerFunction")]
        public static void Run([QueueTrigger("create-container")]string containerDetails, TraceWriter log)
        {
            var telemetry = AppInsightClient.Telemetry;

            var details = JsonConvert.DeserializeObject<ContainerDetails>(containerDetails);

            try
            {
                telemetry.TrackTrace($"Create Container: {details.ContainerName}");

                CreateContainers(details);

                AddMessage.Send(containerDetails, "retrieve-blob");
            }
            catch (Exception e)
            {
                telemetry.TrackTrace($"ERROR Container: {details.ContainerName}, Exception: {e}");
            }
        }

        private static void CreateContainers(ContainerDetails details)
        {
            var destConnectionString = CloudConfigurationManager.GetSetting(details.Destination);
            var destStorageAccount =
                CloudStorageAccount.Parse(destConnectionString);
            var blobClient = destStorageAccount.CreateCloudBlobClient();
            var containerReference = blobClient.GetContainerReference(details.ContainerName);

            containerReference.CreateIfNotExists();
        }
    }
}
