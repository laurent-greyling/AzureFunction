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
            var details = JsonConvert.DeserializeObject<ContainerDetails>(containerDetails);

            CreateContainers(details);

            AddMessage.Send(containerDetails, "retrieve-blob");
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
