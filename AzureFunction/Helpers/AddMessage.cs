using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AzureFunction.Helpers
{
    public class AddMessage
    {
        public static void Send(string message, string queue)
        {
            var queueConnectionString = CloudConfigurationManager.GetSetting("AzureWebJobsStorage");
            var queueStorageAccount =
                CloudStorageAccount.Parse(queueConnectionString);

            var queueClient = queueStorageAccount.CreateCloudQueueClient();
            var queueReference = queueClient.GetQueueReference(queue);
            queueReference.CreateIfNotExists();

            var msg = new CloudQueueMessage(message);
            queueReference.AddMessage(msg);
        }
    }
}
