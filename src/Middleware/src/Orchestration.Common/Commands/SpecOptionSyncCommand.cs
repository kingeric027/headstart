using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Orchestration.Common.Exceptions;
using Orchestration.Common.Extensions;
using Orchestration.Common.Models;
using Orchestration.Common.Queries;
using OrderCloud.SDK;

namespace Orchestration.Common.Commands
{
    public class SpecOptionSyncCommand : SyncCommand, IWorkItemCommand
    {
        private readonly IOrderCloudClient _oc;
        public SpecOptionSyncCommand(IAppSettings settings, LogQuery log, IOrderCloudClient oc) : base(settings, log)
        {
            _oc = oc;
        }

        public async Task<JObject> CreateAsync(WorkItem wi)
        {
            var obj = wi.Current.ToObject<SpecOption>();
            try
            {
                obj.ID = wi.RecordId;
                var response = await _oc.Specs.CreateOptionAsync(obj.xp.SpecID, obj);
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
            var obj = JObject.FromObject(wi.Current).ToObject<SpecOption>();
            try
            {
                if (obj.ID == null) obj.ID = wi.RecordId;
                var response = await _oc.Specs.SaveOptionAsync(obj.xp.SpecID, wi.RecordId, obj);
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
            var obj = JObject.FromObject(wi.Diff).ToObject<PartialSpecOption>();
            try
            {
                // must get ID from current or cache in case it's not part of the Diff (and it's not expected to be)
                var cache = JObject.FromObject(wi.Cache).ToObject<PartialSpecOption<OrchestrationSpecOptionXp>>();
                var response = await _oc.Specs.PatchOptionAsync(cache.xp.SpecID, wi.RecordId, obj);
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
                var response = await _oc.Specs.GetOptionAsync(wi.Current.To<SpecOption>().xp.SpecID, wi.RecordId);
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
