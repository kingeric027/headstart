using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Marketplace.Common.Exceptions;
using Marketplace.Common.Extensions;
using Marketplace.Common.Mappers;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using OrderCloud.SDK;
using Marketplace.Helpers.Models;
using Marketplace.Helpers.Extensions;

namespace Marketplace.Common.Commands
{
    public class SpecOptionSyncCommand : SyncCommand, IWorkItemCommand
    {
        private readonly IOrderCloudClient _oc;
        public SpecOptionSyncCommand(AppSettings settings, LogQuery log, IOrderCloudClient oc) : base(settings, log)
        {
            _oc = oc;
        }

        public async Task<JObject> CreateAsync(WorkItem wi)
        {
            var obj = MarketplaceSpecOptionMapper.Map(wi.Current.ToObject<MarketplaceSpecOption>());
            try
            {
                obj.ID = wi.RecordId;
                var response = await _oc.Specs.CreateOptionAsync(obj.xp.SpecID, obj, wi.Token);
                return JObject.FromObject(response);
            }
            catch (OrderCloudException exId) when (IdExists(exId))
            {
                // handle 409 errors by refreshing cache
                await _log.Save(new OrchestrationLog(wi)
                {
                    ErrorType = OrchestrationErrorType.CreateExistsError,
                    Message = exId.Message,
                    Level = LogLevel.Error
                });
                return await GetAsync(wi);
            }
            catch (OrderCloudException ex)
            {
                await _log.Save(new OrchestrationLog(wi)
                {
                    ErrorType = OrchestrationErrorType.CreateGeneralError,
                    Message = ex.Message,
                    Level = LogLevel.Error
                });
                throw new Exception(OrchestrationErrorType.CreateGeneralError.ToString(), ex);
            }
            catch (Exception e)
            {
                await _log.Save(new OrchestrationLog(wi)
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
            var obj = MarketplaceSpecOptionMapper.Map(JObject.FromObject(wi.Current).ToObject<MarketplaceSpecOption>());
            try
            {
                if (obj.ID == null) obj.ID = wi.RecordId;
                var response = await _oc.Specs.SaveOptionAsync(obj.xp.SpecID, wi.RecordId, obj, wi.Token);
                return JObject.FromObject(response);
            }
            catch (OrderCloudException ex)
            {
                await _log.Save(new OrchestrationLog(wi)
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
            var obj = MarketplaceSpecOptionMapper.Map(JObject.FromObject(wi.Diff).ToObject<PartialSpecOption>());
            try
            {
                // must get ID from current or cache in case it's not part of the Diff (and it's not expected to be)
                var cache = JObject.FromObject(wi.Cache).ToObject<PartialSpecOption>();
                var response = await _oc.Specs.PatchOptionAsync(cache.xp.SpecID, wi.RecordId, obj, wi.Token);
                return JObject.FromObject(response);
            }
            catch (OrderCloudException ex)
            {
                await _log.Save(new OrchestrationLog(wi)
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
                var response = await _oc.Specs.GetOptionAsync(wi.Current.To<SpecOption>().xp.SpecID, wi.RecordId, wi.Token);
                return JObject.FromObject(response);
            }
            catch (OrderCloudException ex)
            {
                await _log.Save(new OrchestrationLog(wi)
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
