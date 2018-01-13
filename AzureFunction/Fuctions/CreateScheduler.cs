using System;
using System.Security.Cryptography.X509Certificates;
using AzureFunction.Helpers;
using AzureFunction.Models;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Management.Scheduler;
using Microsoft.WindowsAzure.Management.Scheduler.Models;
using Microsoft.WindowsAzure.Scheduler;
using Microsoft.WindowsAzure.Scheduler.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace AzureFunction.Fuctions
{
    public static class CreateScheduler
    {
        [FunctionName("CreateScheduler")]
        public static void Run([QueueTrigger("create-scheduler")]string schedulerDetails, TraceWriter log)
        {
            var telemetry = AppInsightClient.Telemetry;
            var details = JsonConvert.DeserializeObject<AzureDetails>(schedulerDetails);

            try
            {
                telemetry.TrackTrace("Create Scheduler");
                CreateSchedulerJob(details);
            }
            catch (Exception e)
            {
                telemetry.TrackTrace($"ERROR: {e}");
            }
        }

        private static void CreateSchedulerJob(AzureDetails details)
        {
            // retrieve the windows azure managment certificate from current user
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            var certificate = store.Certificates.Find(X509FindType.FindByThumbprint, "<THUMBPRINT of CERTIFICATE>", false)[0];
            store.Close();

            // create management credencials and cloud service management client
            var credentials = new CertificateCloudCredentials("<Your subscription ID>", certificate);
            var cloudServiceMgmCli = new CloudServiceManagementClient(credentials);

            // create job collection
            var schedulerMgmCli = new SchedulerManagementClient(credentials);
            var jobCollectionIntrinsicSettings = new JobCollectionIntrinsicSettings()
            {
                Plan = JobCollectionPlan.Free,
                Quota = new JobCollectionQuota()
                {
                    MaxJobCount = 5,
                    MaxJobOccurrence = 1,
                    MaxRecurrence = new JobCollectionMaxRecurrence()
                    {
                        Frequency = JobCollectionRecurrenceFrequency.Hour,
                        Interval = 24
                    }
                }
            };

            var jobCollectionCreateParameters = new JobCollectionCreateParameters()
            {
                IntrinsicSettings = jobCollectionIntrinsicSettings,
                Label = "RunBackupJob"
            };

            var jobCollectionCreateResponse = schedulerMgmCli.JobCollections.Create(details.ResourceGroup, "RunBackupJob", jobCollectionCreateParameters);
            //generate sas token
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("AzureWebJobsStorage"));
            
            //Set the expiry time and permissions for the container.
            //In this case no start time is specified, so the shared access signature becomes valid immediately.
            var sasConstraints = new SharedAccessAccountPolicy
            {
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddYears(1),
                Permissions = SharedAccessAccountPermissions.Add
                | SharedAccessAccountPermissions.Create
                | SharedAccessAccountPermissions.Delete
                | SharedAccessAccountPermissions.ProcessMessages
                | SharedAccessAccountPermissions.Read
                | SharedAccessAccountPermissions.Write
            };

            var token = storageAccount.GetSharedAccessSignature(sasConstraints);

            // create job
            var schedulerClient = new SchedulerClient(details.ResourceGroup, "RunBackupJob", credentials);
            var jobAction = new JobAction()
            {
                Type = JobActionType.StorageQueue,
                QueueMessage = new JobQueueMessage
                {
                    Message = details.PairName,
                    QueueName = "get-containers",
                    SasToken = sasConstraints.ToString(),
                    StorageAccountName = "queuefunction"
                }
            };
            var jobRecurrence = new JobRecurrence()
            {
                Frequency = JobRecurrenceFrequency.Hour,
                Interval = 24
            };
            var jobCreateOrUpdateParameters = new JobCreateOrUpdateParameters()
            {
                Action = jobAction,
                Recurrence = jobRecurrence
            };
            var jobCreateResponse = schedulerClient.Jobs.CreateOrUpdate(details.ResourceGroup, jobCreateOrUpdateParameters);
        }
    }
}
