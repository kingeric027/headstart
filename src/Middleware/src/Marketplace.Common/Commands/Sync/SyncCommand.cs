using System;
using System.Reflection;
using System.Threading.Tasks;
using Marketplace.Common.Models;
using Newtonsoft.Json.Linq;
using Marketplace.Common.Queries;
using Marketplace.Models.Misc;
using OrderCloud.SDK;
using Action = Marketplace.Common.Models.Action;

namespace Marketplace.Common.Commands
{
    public interface IWorkItemCommand
    {
        Task<JObject> CreateAsync(WorkItem wi);
        Task<JObject> PatchAsync(WorkItem wi);
        Task<JObject> UpdateAsync(WorkItem wi);
        Task<JObject> DeleteAsync(WorkItem wi);
        Task<JObject> GetAsync(WorkItem wi);
    }

    public interface ISyncCommand
    {
        Task<JObject> Dispatch(WorkItem wi);
    }

    public class SyncCommand : ISyncCommand
    {
        private const string ASSEMBLY = "Marketplace.Common.Commands.";
        protected readonly AppSettings _settings;
        protected readonly LogQuery _log;
        private IOrderCloudClient _oc;
        
        public SyncCommand(AppSettings settings, LogQuery log)
        {
            _settings = settings;
            _log = log;
        }

        public bool IdExists(OrderCloudException ex)
        {
            return ex.Errors[0].ErrorCode == OcError.IdExists;
        }

        public async Task<JObject> Dispatch(WorkItem wi)
        {
            if (wi.Action == Action.Ignore) return null;

            _oc = new OrderCloudClient(new OrderCloudClientConfig()
            {
                ApiUrl = _settings.OrderCloudSettings.ApiUrl,
                AuthUrl = _settings.OrderCloudSettings.AuthUrl,
                ClientId = wi.ClientId
            });

            var type = Type.GetType($"{ASSEMBLY}{wi.RecordType}SyncCommand", true);
            var command = (IWorkItemCommand) Activator.CreateInstance(type, new object[] {_settings, _log, _oc});
            var method = command.GetType()
                .GetMethod($"{wi.Action}Async", BindingFlags.Public | BindingFlags.Instance);
           

            return await (Task<JObject>) method.Invoke(command, new object[] { wi });
        }
    }

    public static class OcError
    {
        public static string IdExists => "IdExists";
        public static string NotFound => "NotFound";
    }
}
