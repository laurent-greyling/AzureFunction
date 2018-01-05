using AzureFunction.Helpers;
using AzureFunction.Models;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;

namespace AzureFunction
{
    public static class CreateContainer
    {
        [FunctionName("CreateContainer")]
        public static void Run([QueueTrigger("create-container")]string containers, TraceWriter log)
        {
            var containerDetails = JsonConvert.DeserializeObject<ContainerDetails>(containers);

            CreateContainers(containerDetails);

            AddMessage.Send(containers, "retrieve-blob");
        }

        private static void CreateContainers(ContainerDetails details)
        {
            var backupConnectionString = CloudConfigurationManager.GetSetting(details.Backup);
            var backupStorageAccount =
                CloudStorageAccount.Parse(backupConnectionString);
            var blobClient = backupStorageAccount.CreateCloudBlobClient();
            var containerReference = blobClient.GetContainerReference(details.ContainerName);

            containerReference.CreateIfNotExists();
        }
    }
}
