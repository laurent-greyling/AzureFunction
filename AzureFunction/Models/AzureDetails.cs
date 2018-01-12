namespace AzureFunction.Models
{
    public class AzureDetails
    {
        public string ResourceGroup { get; set; }
        public string PairName { get; set; }
        public string BackupConnection { get; set; }
        public string ProductionConnection { get; set; }
        public string StartTime { get; set; }
    }
}
