using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Orchestration.ProductOrchestration;

[assembly: WebJobsStartup(typeof(Startup))]
namespace Orchestration.ProductOrchestration
{
    public static class ProductQueueTrigger
    {
        [FunctionName("ProductQueueTrigger")]
        public static async void Run([BlobTrigger("orchestration-queue/{name}", Connection = "AzureWebJobsStorage")]CloudBlockBlob blob, string name, [OrchestrationClient]DurableOrchestrationClient client, ILogger log)
        {
            log.LogInformation($"Orchestration item queued: {blob.Name}");
            await client.StartNewAsync("ProductOrchestrationWorkflow", blob.Name);
        }
    }
}
