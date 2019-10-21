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
    public class SpecProductAssignmentSyncCommand : SyncCommand, IWorkItemCommand
    {
        private readonly IOrderCloudClient _oc;
        public SpecProductAssignmentSyncCommand(IAppSettings settings, LogQuery log, IOrderCloudClient oc) : base(settings, log)
        {
            _oc = oc;
        }

        public async Task<JObject> CreateAsync(WorkItem wi)
        {
            var obj = wi.Current.ToObject<SpecProductAssignment>();
            try
            {
                await _oc.Specs.SaveProductAssignmentAsync(obj);
                return JObject.FromObject(obj);
            }
            catch (OrderCloudException exId) when (IdExists(exId))
            {
                // this should never occur
                await _log.Upsert(new OrchestrationLog(wi)
                {
                    ErrorType = OrchestrationErrorType.CreateExistsError
                });
                return JObject.FromObject(obj);
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
            var obj = JObject.FromObject(wi.Current).ToObject<SpecProductAssignment>();
            try
            {
                await _oc.Specs.SaveProductAssignmentAsync(obj);
                return JObject.FromObject(obj);
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
            var obj = JObject.FromObject(wi.Diff).ToObject<SpecProductAssignment>();
            try
            {
                await _oc.Specs.SaveProductAssignmentAsync(obj);
                return JObject.FromObject(obj);
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
                var response = await _oc.Specs.ListProductAssignmentsAsync(wi.RecordId);
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
