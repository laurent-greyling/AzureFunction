using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace AzureFunction
{
    public static class RetrieveBlobs
    {
        [FunctionName("RetrieveBlobs")]
        public static void Run([QueueTrigger("retrieve-blob")]string myQueueItem, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
