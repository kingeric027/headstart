using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Marketplace.Common.Commands;
using Marketplace.Common.Exceptions;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Action = Marketplace.Common.Models.Action;

namespace Orchestration.ProductOrchestration
{
    public class ProductOrchestrationWorkflow
    {
        private readonly IOrchestrationCommand _orch;
        private readonly ISyncCommand _sync;
        private readonly LogQuery _log;

        public ProductOrchestrationWorkflow(IOrchestrationCommand orch, ISyncCommand sync, LogQuery log)
        {
            _orch = orch;
            _sync = sync;
            _log = log;
        }

        [FunctionName("ProductOrchestrationWorkflow")]
        public async Task RunOrchestrator([OrchestrationTrigger] DurableOrchestrationContext context, ILogger log)
        {
            try
            {
                var path = context.GetInput<string>();

                var wi = new WorkItem(path)
                {
                    Cache = await context.CallActivityAsync<JObject>("GetCachedItem", path),
                    Current = await context.CallActivityAsync<JObject>("GetQueuedItem", path)
                };
                wi.Token = wi.Current["Token"].ToString();
                wi.ClientId = wi.Current["ClientId"].ToString();
                wi.Diff = await context.CallActivityAsync<JObject>("CalculateDiff", wi);
                wi.Action = await context.CallActivityAsync<Action>("DetermineAction", wi);

                switch (wi.Action)
                {
                    case Action.Ignore:
                        wi.Cache = null;
                        break;
                    case Action.Get:
                        wi.Cache = await context.CallActivityAsync<JObject>("RefreshCache", wi);
                        break;
                    case Action.Create:
                    case Action.Delete:
                    case Action.Patch:
                    case Action.Update:
                    default:
                        wi.Cache = await context.CallActivityAsync<JObject>("OrderCloudAction", wi);
                        break;
                }
                
                await context.CallActivityAsync("UpdateCache", wi);

                log.LogInformation($"{wi.RecordId}: {wi.Action.ToString()} successfully");
                await context.CallActivityAsync<Newtonsoft.Json.Linq.JObject>("LogEvent", new OrchestrationLog(wi));
            }
            catch (OrchestrationException oex)
            {
                log.LogError($"{oex.Error.Type}: {oex.Message}", oex.Error.Data);
            }
            catch (FunctionFailedException fex)
            {
                log.LogError(fex.Message);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }
            finally
            {
                await context.CallActivityAsync("CleanupQueue", context.GetInput<string>());
            }
        }

        [FunctionName("LogEvent")]
        public async Task LogEvent([ActivityTrigger] OrchestrationLog log) => await _log.Upsert(log);

        [FunctionName("RefreshCache")]
        public async Task<Newtonsoft.Json.Linq.JObject> RefreshCache([ActivityTrigger] WorkItem wi) => await _sync.Dispatch(wi);

        [FunctionName("CleanupQueue")]
        public async Task CleanupQueue([ActivityTrigger] string path) => await _orch.CleanupQueue(path);

        [FunctionName("UpdateCache")]
        public async Task UpdateCache([ActivityTrigger] WorkItem wi) => await _orch.SaveToCache(wi);

        [FunctionName("OrderCloudAction")]
        public async Task<Newtonsoft.Json.Linq.JObject> OrderCloudAction([ActivityTrigger] WorkItem wi) => await _sync.Dispatch(wi);

        [FunctionName("DetermineAction")]
        public async Task<Action> DetermineAction([ActivityTrigger] WorkItem wi) => await _orch.DetermineAction(wi);

        [FunctionName("CalculateDiff")]
        public async Task<Newtonsoft.Json.Linq.JObject> CalculateDiff([ActivityTrigger] WorkItem wi) => await _orch.CalculateDiff(wi);

        [FunctionName("GetQueuedItem")]
        public async Task<Newtonsoft.Json.Linq.JObject> GetQueuedItem([ActivityTrigger] string path) => await _orch.GetQueuedItem(path);

        [FunctionName("GetCachedItem")]
        public async Task<Newtonsoft.Json.Linq.JObject> GetCachedItem([ActivityTrigger] string path) => await _orch.GetCachedItem(path);
    }
}