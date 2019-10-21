﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Orchestration.Common.Exceptions;
using Orchestration.Common.Models;
using Orchestration.Common.Queries;
using OrderCloud.SDK;

namespace Orchestration.Common.Commands
{
    public class PriceScheduleSyncCommand : SyncCommand, IWorkItemCommand
    {
        private readonly IOrderCloudClient _oc;
        public PriceScheduleSyncCommand(IAppSettings settings, LogQuery log, IOrderCloudClient oc) : base(settings, log)
        {
            _oc = oc;
        }

        public async Task<JObject> CreateAsync(WorkItem wi)
        {
            var obj = wi.Current.ToObject<PriceSchedule>();
            try
            {
                obj.ID = wi.RecordId;
                var response = await _oc.PriceSchedules.CreateAsync(obj);
                return JObject.FromObject(response);
            }
            catch (OrderCloudException exId) when (IdExists(exId))
            {
                // handle 409 errors by refreshing cache
                await _log.Upsert(new OrchestrationLog(wi)
                {
                    ErrorType = OrchestrationErrorType.CreateExistsError
                });
                return await GetAsync(wi);
            }
            catch (OrderCloudException ex)
            {
                await _log.Upsert(new OrchestrationLog(wi)
                {
                    ErrorType = OrchestrationErrorType.CreateGeneralError
                });
                throw new Exception(OrchestrationErrorType.CreateGeneralError.ToString(), ex);
            }
        }

        public async Task<JObject> UpdateAsync(WorkItem wi)
        {
            var obj = JObject.FromObject(wi.Current).ToObject<PriceSchedule>();
            try
            {
                if (obj.ID == null) obj.ID = wi.RecordId;
                var response = await _oc.PriceSchedules.SaveAsync<PriceSchedule>(wi.RecordId, obj);
                return JObject.FromObject(response);
            }
            catch (OrderCloudException ex)
            {
                await _log.Upsert(new OrchestrationLog(wi)
                {
                    ErrorType = OrchestrationErrorType.UpdateGeneralError
                });
                throw new Exception(OrchestrationErrorType.UpdateGeneralError.ToString(), ex);
            }
        }

        public async Task<JObject> PatchAsync(WorkItem wi)
        {
            var obj = JObject.FromObject(wi.Diff).ToObject<PartialPriceSchedule<OrchestrationPriceScheduleXp>>();
            try
            {
                var response = await _oc.PriceSchedules.PatchAsync(wi.RecordId, obj);
                return JObject.FromObject(response);
            }
            catch (OrderCloudException ex)
            {
                await _log.Upsert(new OrchestrationLog(wi)
                {
                    ErrorType = OrchestrationErrorType.PatchGeneralError
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
                var response = await _oc.PriceSchedules.GetAsync(wi.RecordId);
                return JObject.FromObject(response);
            }
            catch (OrderCloudException ex)
            {
                await _log.Upsert(new OrchestrationLog(wi)
                {
                    ErrorType = OrchestrationErrorType.GetGeneralError
                });
                throw new Exception(OrchestrationErrorType.GetGeneralError.ToString(), ex);
            }
        }
    }
}
