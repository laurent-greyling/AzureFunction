using Microsoft.WindowsAzure.Storage.Table;

namespace AzureFunction.Models
{
    public class StorageAccountPair : TableEntity
    {
        public string Source { get; set; }
        public string Destination { get; set; }
    }
}
