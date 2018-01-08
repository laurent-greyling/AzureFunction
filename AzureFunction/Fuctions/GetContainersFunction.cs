using System.Collections.Generic;
using System.Linq;
using AzureFunction.Helpers;
using AzureFunction.Models;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;

namespace AzureFunction.Fuctions
{
    public static class GetContainersFunction
    {
        [FunctionName("GetContainersFunction")]
        public static void Run([QueueTrigger("get-containers")]string scheduledQueueItem, TraceWriter log)
        {
            //deserialise the xml we get from scheduler to get the message.
            var pair = new StoragePair().Get(scheduledQueueItem);

            var messages = RetrieveContainers(pair);
            
            foreach (var message in messages)
            {
                //creates a message and sends to the create-container queue where next function will pick up
                AddMessage.Send(message, "create-container");
            }
        }

        private static List<string> RetrieveContainers(StorageAccountPair pair)
        {
            var sourceConnectionString = CloudConfigurationManager.GetSetting(pair.Source);
            var sourceStorageAccount =
                CloudStorageAccount.Parse(sourceConnectionString);
            var blobClient = sourceStorageAccount.CreateCloudBlobClient();

            return blobClient.ListContainers().Select(c => JsonConvert.SerializeObject(new ContainerDetails
            {
                Source = pair.Source,
                Destination = pair.Destination,
                ContainerName = c.Name
            })).ToList();
        }
    }
}
