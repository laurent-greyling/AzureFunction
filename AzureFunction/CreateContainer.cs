using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace AzureFunction
{
    public static class CreateContainer
    {
        [FunctionName("CreateContainer")]
        public static void Run([QueueTrigger("create-container", Connection = "")]string myQueueItem, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
