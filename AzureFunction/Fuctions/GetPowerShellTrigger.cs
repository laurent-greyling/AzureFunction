using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AzureFunction.Helpers;
using AzureFunction.Models;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace AzureFunction.Fuctions
{
    public static class GetPowerShellTrigger
    {
        [FunctionName("GetPowerShellTrigger")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "GetPowerShellTrigger")]HttpRequestMessage req, TraceWriter log)
        {          
            var data = await req.Content.ReadAsStringAsync();
            var azureDetails = JsonConvert.DeserializeObject<AzureDetails>(data);

            SavePair(azureDetails.PairName,azureDetails.ProductionConnection, azureDetails.BackupConnection);

            AddMessage.Send(JsonConvert.SerializeObject(new AzureDetails()
            {
                ResourceGroup = azureDetails.ResourceGroup,
                PairName =azureDetails.PairName,
                StartTime =azureDetails.StartTime
            }),
            "create-scheduler");

            // Fetching the name from the path parameter in the request URL
            return req.CreateResponse(HttpStatusCode.OK, "Process Started");
        }

        private static void SavePair(string pairName, string source, string destination)
        {
            var operationsConnectionString = CloudConfigurationManager.GetSetting("AzureWebJobsStorage");
            var operationsStorageAccount =
                CloudStorageAccount.Parse(operationsConnectionString);

            var tableClient = operationsStorageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("ConnectionPair");

            table.CreateIfNotExists();

            var operation = TableOperation.InsertOrMerge(new StorageAccountPair
            {
                PartitionKey = pairName,
                RowKey = Guid.NewGuid().ToString(),
                Source = source,
                Destination = destination
            });

            table.Execute(operation);
        }
    }
}
