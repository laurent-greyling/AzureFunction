using System;
using System.Diagnostics;
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
            var telemetry = AppInsightClient.Telemetry;
            
            var details = JsonConvert.DeserializeObject<BlobDetails>(blobItem);

            try
            {
                telemetry.TrackTrace($"Copy Blob {details.BlobName}");

                MoveBlobs(details);
            }
            catch (Exception e)
            {
                telemetry.TrackTrace($"ERROR: Cannot copy blob: {details.BlobName}, exception: {e}");
            }
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
