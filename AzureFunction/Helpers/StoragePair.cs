using System.IO;
using System.Text;
using System.Xml.Serialization;
using AzureFunction.Models;
using Microsoft.WindowsAzure.Scheduler.Models;
using Newtonsoft.Json;

namespace AzureFunction.Helpers
{
    public class StoragePair
    {
        public StorageAccountPair Get(string myQueueItem)
        {
            var serializer = new XmlSerializer(typeof(StorageQueueMessage));

            using (var xmlstream = new MemoryStream(Encoding.Unicode.GetBytes(myQueueItem)))
            {
                var message = (StorageQueueMessage)serializer.Deserialize(xmlstream);

                return JsonConvert.DeserializeObject<StorageAccountPair>(message.Message);
            }
        }
    }
}
