using System.Collections.Generic;
using System.Linq;
using AzureFunction.Helpers;
using AzureFunction.Models;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;

namespace AzureFunction
{
    public static class GetContainers
    {
        [FunctionName("GetContainers")]
        public static void Run([QueueTrigger("get-containers")]string scheduledQueueItem, TraceWriter log)
        {
            var pair = new StoragePair().Get(scheduledQueueItem);

            var messages = RetrieveContainers(pair);
            
            foreach (var message in messages)
            {
                AddMessage.Send(message, "create-container");
            }
        }

        private static List<string> RetrieveContainers(StorageAccountPair pair)
        {
            var productionConnectionString = CloudConfigurationManager.GetSetting(pair.Production);
            var productionStorageAccount =
                CloudStorageAccount.Parse(productionConnectionString);
            var blobClient = productionStorageAccount.CreateCloudBlobClient();

            return blobClient.ListContainers().Select(c => JsonConvert.SerializeObject(new ContainerDetails
            {
                Production = pair.Production,
                Backup = pair.Backup,
                ContainerName = c.Name
            })).ToList();
        }
    }
}
