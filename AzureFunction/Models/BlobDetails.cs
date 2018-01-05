using System;

namespace AzureFunction.Models
{
    public class BlobDetails : ContainerDetails
    {
        public string BlobName { get; set; }
        public Uri BlobUri { get; set; }
    }
}
