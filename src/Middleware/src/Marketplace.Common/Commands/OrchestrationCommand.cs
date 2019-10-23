using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Marketplace.Common.Exceptions;
using Marketplace.Common.Extensions;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Marketplace.Common.Services;
using Action = Marketplace.Common.Models.Action;
using ApiRole = OrderCloud.SDK.ApiRole;
using ErrorCodes = Marketplace.Common.Exceptions.ErrorCodes;

namespace Marketplace.Common.Commands
{
    public interface IOrchestrationCommand
    {
        Task CleanupQueue(string path);
        Task SaveToCache(WorkItem wi);
        Task<Action> DetermineAction(WorkItem wi);
        Task<JObject> CalculateDiff(WorkItem wi);
        Task<JObject> GetQueuedItem(string path);
        Task<JObject> GetCachedItem(string path);
        Task<T> SaveToQueue<T>(T obj, string supplierId) where T : IOrchestrationObject;
        Task<OrchestrationSupplier> GetSupplier(string path);
    }

    public class OrchestrationCommand : IOrchestrationCommand
    {
        private readonly IBlobService _blob;
        private readonly IAppSettings _settings;
        private readonly LogQuery _log;
        private readonly SupplierQuery _supplier;

        public OrchestrationCommand(IAppSettings settings, IBlobService blob, LogQuery log, SupplierQuery supplier)
        {
            _settings = settings;
            _blob = blob;
            _log = log;
            _supplier = supplier;
        }

        public async Task<OrchestrationSupplier> GetSupplier(string path)
        {
            var supplier = await _supplier.Get(path.Split("/")[0]);
            return supplier;
        }

        public async Task<T> SaveToQueue<T>(T obj, string supplierId) where T : IOrchestrationObject
        {
            try
            {
                await _blob.Save(_settings.BlobSettings.QueueName, obj.BuildPath(supplierId),
                    JsonConvert.SerializeObject(obj));
                return await Task.FromResult(obj);
            }
            catch (ApiErrorException ex)
            {
                throw new ApiErrorException(ex.ApiError);
            }
            catch (Exception)
            {
                await _log.Upsert(new OrchestrationLog()
                {
                    Level = LogLevel.Error,
                    Message = $"Failed to save blob to queue from API: {supplierId} - {typeof(T)}",
                    Current = JObject.FromObject(obj)
                });
                throw new ApiErrorException(ErrorCodes.All["WriteFailure"], obj);
            }
        }

        public async Task CleanupQueue(string path)
        {
            try
            {
                await _blob.Delete(_settings.BlobSettings.QueueName, path);
            }
            catch (Exception ex)
            {
                await _log.Upsert(new OrchestrationLog()
                {
                    Level = LogLevel.Error,
                    Message = $"Failed to remove blob to queue: {path}",
                    
                });
                throw new OrchestrationException(OrchestrationErrorType.QueueCleanupError, $"{path} | {ex.Message}");
            }
        }
        public async Task SaveToCache(WorkItem wi)
        {
            if (wi.Cache == null) await Task.CompletedTask;
            try
            {
                await _blob.Save(_settings.BlobSettings.CacheName, $"{wi.SupplierId.ToLower()}/{wi.RecordType.ToString().ToLower()}/{wi.RecordId}", wi.Cache);
            }
            catch (Exception ex)
            {
                throw new OrchestrationException(OrchestrationErrorType.CacheUpdateError, wi, ex.Message);
            }
        }

        public async Task<JObject> GetQueuedItem(string path)
        {
            try
            {
                return await _blob.Get<JObject>(_settings.BlobSettings.QueueName, path);
            }
            catch (Exception ex)
            {
                throw new OrchestrationException(OrchestrationErrorType.QueuedGetError, $"{path} | {ex.Message}");
            }
        }

        public async Task<JObject> GetCachedItem(string path)
        {
            try
            {
                return await _blob.Get<JObject>(_settings.BlobSettings.CacheName, $"{path}");
            }
            catch (Exception)
            {
                return null; // we don't want to error here. there is an expectation that new items won't be in cache
            }
        }

        public async Task<JObject> CalculateDiff(WorkItem wi)
        {
            try
            {
                return await Task.FromResult(wi.Current.Diff(wi.Cache));
            }
            catch (Exception ex)
            {
                throw new OrchestrationException(OrchestrationErrorType.DiffCalculationError, wi, ex.Message);
            }
        }

        public async Task<Action> DetermineAction(WorkItem wi)
        {
            // we want to ensure a condition is met before determining an action
            // so that if there is a case not compensated for it flows to an exception
            try
            {
                // first check if there is a cache object, if not it's a CREATE event
                if (wi.Cache == null)
                    return await Task.FromResult(Action.Create);

                // if cache does exists, and there is no difference ignore the action
                if (wi.Cache != null && wi.Diff == null)
                    return await Task.FromResult(wi.RecordType == RecordType.SpecProductAssignment ? Action.Ignore : Action.Get);

                // special case for SpecAssignment because there is no ID for the OC object
                // but we want one in orchestration to handle caching
                // in further retrospect I don't think we need to handle updating objects when only the ID is being changed
                // maybe in the future a true business case will be necessary to do this
                if (wi.RecordType == RecordType.SpecProductAssignment && wi.Diff.Count == 1 && wi.Diff.SelectToken("ID").Path == "ID")
                    return await Task.FromResult(Action.Ignore);

                if (wi.Cache != null && wi.Diff != null)
                {
                    // cache exists, we want to force a PUT when xp has deleted properties because 
                    // it's the only way to delete the properties
                    if (wi.Cache.HasDeletedXp(wi.Current))
                        return await Task.FromResult(Action.Update);
                    // otherwise we want to PATCH the existing object
                    return await Task.FromResult(Action.Patch);
                }
            }
            catch (Exception ex)
            {
                throw new OrchestrationException(OrchestrationErrorType.ActionEvaluationError, wi, ex.Message);
            }
            
            throw new OrchestrationException(OrchestrationErrorType.ActionEvaluationError, wi, "Unable to determine action");
        }
    }
}
