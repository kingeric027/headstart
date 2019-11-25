using System;
using Marketplace.Orchestration;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

[assembly: WebJobsStartup(typeof(Startup))]
namespace Marketplace.Orchestration
{
    public static class ObjectQueueTrigger
    {
        [FunctionName("ObjectQueueTrigger")]
        public static async void Run([BlobTrigger("orchestration-queue/{name}", Connection = "AzureWebJobsStorage")]CloudBlockBlob blob, string name, [OrchestrationClient]DurableOrchestrationClient client, ILogger log)
        {
            log.LogInformation($"Orchestration item queued: {blob.Name}");
            await client.StartNewAsync("OrchestrationWorkflow", blob.Name);
        }
    }
}
