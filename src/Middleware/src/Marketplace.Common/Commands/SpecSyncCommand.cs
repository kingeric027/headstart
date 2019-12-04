﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Marketplace.Common.Exceptions;
using Marketplace.Common.Extensions;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands
{
    public class SpecSyncCommand : SyncCommand, IWorkItemCommand
    {
        private readonly IOrderCloudClient _oc;
        public SpecSyncCommand(IAppSettings settings, LogQuery log, IOrderCloudClient oc) : base(settings, log)
        {
            _oc = oc;
        }

        public async Task<JObject> CreateAsync(WorkItem wi)
        {
            var obj = wi.Current.ToObject<Spec>();
            try
            {
                obj.ID = wi.RecordId;
                var response = await _oc.Specs.CreateAsync(obj, wi.Token);
                return JObject.FromObject(response);
            }
            catch (OrderCloudException exId) when (IdExists(exId))
            {
                // handle 409 errors by refreshing cache
                await _log.Upsert(new OrchestrationLog(wi)
                {
                    ErrorType = OrchestrationErrorType.CreateExistsError,
                    Message = exId.Message,
                    Level = LogLevel.Error
                });
                return await GetAsync(wi);
            }
            catch (OrderCloudException ex)
            {
                await _log.Upsert(new OrchestrationLog(wi)
                {
                    ErrorType = OrchestrationErrorType.CreateGeneralError,
                    Message = ex.Message,
                    Level = LogLevel.Error
                });
                throw new Exception(OrchestrationErrorType.CreateGeneralError.ToString(), ex);
            }
            catch (Exception e)
            {
                await _log.Upsert(new OrchestrationLog(wi)
                {
                    ErrorType = OrchestrationErrorType.CreateGeneralError,
                    Message = e.Message,
                    Level = LogLevel.Error
                });
                throw new Exception(OrchestrationErrorType.CreateGeneralError.ToString(), e);
            }
        }

        public async Task<JObject> UpdateAsync(WorkItem wi)
        {
            var obj = JObject.FromObject(wi.Current).ToObject<Spec>();
            try
            {
                if (obj.ID == null) obj.ID = wi.RecordId;
                var response = await _oc.Specs.SaveAsync<Spec>(wi.RecordId, obj);
                return JObject.FromObject(response);
            }
            catch (OrderCloudException ex)
            {
                await _log.Upsert(new OrchestrationLog(wi)
                {
                    ErrorType = OrchestrationErrorType.UpdateGeneralError,
                    Message = ex.Message,
                    Level = LogLevel.Error
                });
                throw new Exception(OrchestrationErrorType.UpdateGeneralError.ToString(), ex);
            }
        }

        public async Task<JObject> PatchAsync(WorkItem wi)
        {
            var obj = JObject.FromObject(wi.Diff).ToObject<PartialSpec<OrchestrationSpecXp>>();
            try
            {
                var response = await _oc.Specs.PatchAsync(wi.RecordId, obj);
                return JObject.FromObject(response);
            }
            catch (OrderCloudException ex)
            {
                await _log.Upsert(new OrchestrationLog(wi)
                {
                    ErrorType = OrchestrationErrorType.PatchGeneralError,
                    Message = ex.Message,
                    Level = LogLevel.Error
                });
                throw new Exception(OrchestrationErrorType.PatchGeneralError.ToString(), ex);
            }
        }

        public Task<JObject> DeleteAsync(WorkItem wi)
        {
            throw new NotImplementedException();
        }

        public async Task<JObject> GetAsync(WorkItem wi)
        {
            try
            {
                var response = await _oc.Specs.GetAsync(wi.RecordId);
                return JObject.FromObject(response);
            }
            catch (OrderCloudException ex)
            {
                await _log.Upsert(new OrchestrationLog(wi)
                {
                    ErrorType = OrchestrationErrorType.GetGeneralError,
                    Message = ex.Message,
                    Level = LogLevel.Error
                });
                throw new Exception(OrchestrationErrorType.GetGeneralError.ToString(), ex);
            }
        }
    }
}