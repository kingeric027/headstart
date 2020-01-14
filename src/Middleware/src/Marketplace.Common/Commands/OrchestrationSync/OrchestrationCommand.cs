﻿using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Marketplace.Common.Exceptions;
using Marketplace.Common.Extensions;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Marketplace.Helpers.Models;
using Action = Marketplace.Common.Models.Action;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Exceptions;
using Marketplace.Helpers.Services;
using ErrorCodes = Marketplace.Helpers.Exceptions.ErrorCodes;

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
        Task<T> SaveToQueue<T>(T obj, VerifiedUserContext user, string resourceId) where T : IMarketplaceObject;
    }

    public class OrchestrationCommand : IOrchestrationCommand
    {
        private readonly IBlobService _blobQueue;
        private readonly IBlobService _blobCache;
        private readonly AppSettings _settings;
        private readonly LogQuery _log;

        public OrchestrationCommand(AppSettings settings, LogQuery log)
        {
            _settings = settings;
            _blobQueue = new BlobService(new BlobServiceConfig()
            {
                ConnectionString = settings.BlobSettings.ConnectionString,
                Container = settings.BlobSettings.QueueName
            });

            _blobCache = new BlobService(new BlobServiceConfig()
            {
                ConnectionString = settings.BlobSettings.ConnectionString,
                Container = settings.BlobSettings.CacheName
            });
            _log = log;
        }

        //public OrchestrationCommand(AppSettings settings, LogQuery log, IBlobService queueBlob, IBlobService cacheBlob)
        //{
        //    _settings = settings;
        //    _blobQueue = queueBlob;
        //    _blobCache = cacheBlob;
        //    _log = log;
        //}

        public async Task<T> SaveToQueue<T>(T obj, VerifiedUserContext user, string resourceId) where T : IMarketplaceObject
        {
            try
            {
                var orch = new OrchestrationObject<T>
                {
                    Token = user.AccessToken,
                    ClientId = user.ClientID,
                    Model = obj
                };
                await _blobQueue.Save(orch.BuildPath(resourceId), JsonConvert.SerializeObject(obj));
                return await Task.FromResult(obj);
            }
            catch (ApiErrorException ex)
            {
                throw new ApiErrorException(ex.ApiError);
            }
            catch (Exception ex)
            {
                await _log.Save(new OrchestrationLog()
                {
                    Level = LogLevel.Error,
                    Message = $"Failed to save blob to queue from API: {user.SupplierID} - {typeof(T)}:  {ex.Message}",
                    Current = JObject.FromObject(obj)
                });
                throw new ApiErrorException(ErrorCodes.All["WriteFailure"], obj);
            }
        }

        public async Task CleanupQueue(string path)
        {
            try
            {
                await _blobQueue.Delete(path);
            }
            catch (Exception ex)
            {
                await _log.Save(new OrchestrationLog()
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
                await _blobCache.Save($"{wi.ResourceId.ToLower()}/{wi.RecordType.ToString().ToLower()}/{wi.RecordId}", wi.Cache);
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
                var blob = await _blobQueue.Get<JObject>(path);
                return blob.SelectToken("Model") as JObject;
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
                var blob = await _blobCache.Get<JObject>($"{path}");
                return blob.SelectToken("Model") as JObject;
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
                    return await Task.FromResult((wi.RecordType == RecordType.SpecProductAssignment || wi.RecordType == RecordType.UserGroupAssignment || wi.RecordType == RecordType.CatalogAssignment) ? Action.Ignore : Action.Get);

                // special case for SpecAssignment because there is no ID for the OC object
                // but we want one in orchestration to handle caching
                // in further retrospect I don't think we need to handle updating objects when only the ID is being changed
                // maybe in the future a true business case will be necessary to do this
                if ((wi.RecordType == RecordType.SpecProductAssignment || wi.RecordType == RecordType.UserGroupAssignment || wi.RecordType == RecordType.CatalogAssignment) && wi.Diff.Count == 1 && wi.Diff.SelectToken("ID").Path == "ID")
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
