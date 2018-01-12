using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace AzureFunction.Fuctions
{
    public class TriggerData
    {
        public string ResourceGroup { get; set; }
        public string PairName { get; set; }
        public string BackupConnection { get; set; }
        public string ProductionConnection { get; set; }
        public string StartTime { get; set; }
    }

    public static class GetPowerShellTrigger
    {
        [FunctionName("GetPowerShellTrigger")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "GetPowerShellTrigger")]HttpRequestMessage req, TraceWriter log)
        {

            var data = await req.Content.ReadAsStringAsync();

            var queueData = JsonConvert.DeserializeObject<TriggerData>(data);

            // Fetching the name from the path parameter in the request URL
            return req.CreateResponse(HttpStatusCode.OK, "Hello ");
        }
    }
}
