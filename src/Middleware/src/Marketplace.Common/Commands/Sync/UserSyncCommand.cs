using System;
using System.Threading.Tasks;
using Marketplace.Common.Exceptions;
using Marketplace.Common.Helpers;
using Marketplace.Common.Models;
using Newtonsoft.Json.Linq;
using Marketplace.Common.Queries;
using OrderCloud.SDK;
using Marketplace.Models;
using ordercloud.integrations.cms;

namespace Marketplace.Common.Commands
{
    public class UserSyncCommand : SyncCommand, IWorkItemCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly IAssetQuery _assets;
        public UserSyncCommand(AppSettings settings, LogQuery log, IOrderCloudClient oc, IAssetQuery assets) : base(settings, oc, assets, log)
        {
            _oc = oc;
            _assets = assets;
        }

        public async Task<JObject> CreateAsync(WorkItem wi)
        {
            var obj = wi.Current.ToObject<MarketplaceUser>();
            try
            {
                obj.ID = wi.RecordId;
                // odd case where the TermsAccepted property is initialized and the value is invalid. we'll default it to current date/time
                // but the value is not null, and it's not a simple evaluation for a minimum. so i'm using the year = 1 because it works
                if (obj.TermsAccepted != null && obj.TermsAccepted.Value.Year == 1)
                    obj.TermsAccepted = DateTimeOffset.Now;
                var response = await _oc.Users.CreateAsync(wi.ResourceId, obj, wi.Token);
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
            var obj = JObject.FromObject(wi.Current).ToObject<MarketplaceUser>();
            try
            {
                if (obj.ID == null) obj.ID = wi.RecordId;
                // odd case where the TermsAccepted property is initialized and the value is invalid. we'll default it to current date/time
                // but the value is not null, and it's not a simple evaluation for a minimum. so i'm using the year = 1 because it works
                if (obj.TermsAccepted != null && obj.TermsAccepted.Value.Year == 1)
                    obj.TermsAccepted = DateTimeOffset.Now;
                var response = await _oc.Users.SaveAsync<User>(wi.ResourceId, wi.RecordId, obj, wi.Token);
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
            var obj = wi.Diff.ToObject<PartialUser>(OrchestrationSerializer.Serializer);
            try
            {
                // odd case where the TermsAccepted property is initialized and the value is invalid. we'll default it to current date/time
                // but the value is not null, and it's not a simple evaluation for a minimum. so i'm using the year = 1 because it works
                if (obj.TermsAccepted != null && obj.TermsAccepted.Value.Year == 1)
                    obj.TermsAccepted = DateTimeOffset.Now;
                var response = await _oc.Users.PatchAsync(wi.ResourceId, wi.RecordId, obj, wi.Token);
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
                var response = await _oc.Users.GetAsync(wi.ResourceId, wi.RecordId, wi.Token);
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
