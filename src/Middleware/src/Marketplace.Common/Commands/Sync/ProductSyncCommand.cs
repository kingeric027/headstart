using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Marketplace.Common.Exceptions;
using Marketplace.Common.Models;
using Newtonsoft.Json.Linq;
using Marketplace.Common.Queries;
using OrderCloud.SDK;
using Marketplace.Models;
using Marketplace.Helpers;
using Marketplace.Helpers.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Marketplace.Common.Commands
{
    
    public class PartialConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject.FromObject(((OrchestrationModel)value).Props, serializer).WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
            throw new NotImplementedException();

        public override bool CanConvert(Type type)
        {
            var t = typeof(OrchestrationModel).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) &&
                typeof(IPartial).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
            return t;
        }
    }

    public class OrchestrationSerializer<T> : DefaultContractResolver
    {
        public static readonly OrchestrationSerializer<T> Instance = new OrchestrationSerializer<T>();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (member.GetCustomAttribute(typeof(ApiNoUpdateAttribute)) != null)
                property.ShouldSerialize = o => false;
            
            return property;
        }
    }
    public class ProductSyncCommand : SyncCommand, IWorkItemCommand
    {
        private readonly IOrderCloudClient _oc;
        public ProductSyncCommand(AppSettings settings, LogQuery log, IOrderCloudClient oc) : base(settings, log)
        {
            _oc = oc;
        }

        public async Task<JObject> CreateAsync(WorkItem wi)
        {
            var obj = wi.Current.ToObject<MarketplaceProduct>();
            try
            {
                obj.ID = wi.RecordId;
                var response = await _oc.Products.CreateAsync(obj, wi.Token);
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
            var obj = JObject.FromObject(wi.Current).ToObject<MarketplaceProduct>();
            try
            {
                if (obj.ID == null) obj.ID = wi.RecordId;
                var response = await _oc.Products.SaveAsync<Product>(wi.RecordId, obj, wi.Token);
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
            var obj = JObject.FromObject(wi.Diff).ToObject<PartialMarketplaceProduct>(JsonSerializer.Create(
                new JsonSerializerSettings()
                {
                    ContractResolver = OrchestrationSerializer<PartialMarketplaceProduct>.Instance
                }));
            try
            {
                var response = await _oc.Products.PatchAsync(wi.RecordId, obj, wi.Token);
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
                var response = await _oc.Products.GetAsync(wi.RecordId, wi.Token);
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
