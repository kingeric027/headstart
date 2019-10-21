using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Orchestration.Common.Commands;
using Orchestration.Common.Exceptions;
using Orchestration.Common.Models;
using Orchestration.Common.Queries;
using Action = Orchestration.Common.Models.Action;
using ApiRole = OrderCloud.SDK.ApiRole;

namespace Orchestration.ProductOrchestration
{
    public class ProductOrchestrationWorkflow
    {
        private readonly IOrchestrationCommand _orch;
        private readonly ISyncCommand _sync;
        private readonly LogQuery _log;
        private readonly SupplierQuery _supplier;

        public ProductOrchestrationWorkflow(IOrchestrationCommand orch, ISyncCommand sync, LogQuery log, SupplierQuery supplier)
        {
            _orch = orch;
            _sync = sync;
            _log = log;
            _supplier = supplier;
        }

        [FunctionName("ProductOrchestrationWorkflow")]
        public async Task RunOrchestrator([OrchestrationTrigger] DurableOrchestrationContext context, ILogger log)
        {
            try
            {
                var path = context.GetInput<string>();

                var wi = new WorkItem(path)
                {
                    Supplier = await context.CallActivityAsync<OrchestrationSupplier>("GetSupplier", path),
                    Cache = await context.CallActivityAsync<JObject>("GetCachedItem", path),
                    Current = await context.CallActivityAsync<JObject>("GetQueuedItem", path)
                };
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
                await context.CallActivityAsync<JObject>("LogEvent", new OrchestrationLog(wi));
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
        public async Task<JObject> RefreshCache([ActivityTrigger] WorkItem wi) => await _sync.Dispatch(wi);

        [FunctionName("GetSupplier")]
        public async Task<OrchestrationSupplier> GetSupplier([ActivityTrigger] string path) => await _orch.GetSupplier(path);

        [FunctionName("CleanupQueue")]
        public async Task CleanupQueue([ActivityTrigger] string path) => await _orch.CleanupQueue(path);

        [FunctionName("UpdateCache")]
        public async Task UpdateCache([ActivityTrigger] WorkItem wi) => await _orch.SaveToCache(wi);

        [FunctionName("OrderCloudAction")]
        public async Task<JObject> OrderCloudAction([ActivityTrigger] WorkItem wi) => await _sync.Dispatch(wi);

        [FunctionName("DetermineAction")]
        public async Task<Action> DetermineAction([ActivityTrigger] WorkItem wi) => await _orch.DetermineAction(wi);

        [FunctionName("CalculateDiff")]
        public async Task<JObject> CalculateDiff([ActivityTrigger] WorkItem wi) => await _orch.CalculateDiff(wi);

        [FunctionName("GetQueuedItem")]
        public async Task<JObject> GetQueuedItem([ActivityTrigger] string path) => await _orch.GetQueuedItem(path);

        [FunctionName("GetCachedItem")]
        public async Task<JObject> GetCachedItem([ActivityTrigger] string path) => await _orch.GetCachedItem(path);
    }
}