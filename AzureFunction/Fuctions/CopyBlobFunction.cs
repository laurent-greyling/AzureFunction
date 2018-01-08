using AzureFunction.Helpers;
using AzureFunction.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace AzureFunction.Fuctions
{
    public static class CopyBlobFunction
    {
        [FunctionName("CopyBlobFunction")]
        public static void Run([QueueTrigger("copy-blob")]string blobItem, TraceWriter log)
        {
            var details = JsonConvert.DeserializeObject<BlobDetails>(blobItem);

            MoveBlobs(details);
        }

        private static void MoveBlobs(BlobDetails details)
        {
            var sourceBlockBlob = AzureBlobReferences.SourceBlob(details);
            var destBlockBlob = AzureBlobReferences.DestinationBlob(details);

            using (var stream = sourceBlockBlob.OpenRead())
            {
                destBlockBlob.UploadFromStream(stream);
                destBlockBlob.CreateSnapshot();
            }
        }       
    }
}