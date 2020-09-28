using System;
using System.Reflection;
using System.Threading.Tasks;
using Marketplace.Common.Models;
using Newtonsoft.Json.Linq;
using Marketplace.Common.Queries;
using ordercloud.integrations.cms;
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
        private readonly IOrderCloudClient _client;
        private readonly IAssetQuery _assets;
        private readonly IAssetedResourceQuery _assetAssignment;
        
        public SyncCommand(AppSettings settings, IOrderCloudClient client, IAssetQuery assets, IAssetedResourceQuery assetAssignment, LogQuery log) : this(settings, client, log)
        {
            _assets = assets;
            _assetAssignment = assetAssignment;
        }

        public SyncCommand(AppSettings settings, IOrderCloudClient client, LogQuery log)
        {
            _settings = settings;
            _log = log;
            _client = client;
        }

        public bool IdExists(OrderCloudException ex)
        {
            return ex.Errors[0].ErrorCode == OcError.IdExists;
        }

        public async Task<JObject> Dispatch(WorkItem wi)
        {
            if (wi.Action == Action.Ignore) return null;
            _client.Config.ClientId = wi.ClientId;
            var type = Type.GetType($"{ASSEMBLY}{wi.RecordType}SyncCommand", true);
            var constructors = type.GetConstructors()[0].GetParameters().Length;
            switch (constructors)
            {
                case 3:
                {
                    var command = (IWorkItemCommand)Activator.CreateInstance(type, _settings, _log, _client);
                    var method = command.GetType()
                        .GetMethod($"{wi.Action}Async", BindingFlags.Public | BindingFlags.Instance);
                    if (method == null) throw new MissingMethodException($"{wi.RecordType}SyncCommand is missing");
                    return await (Task<JObject>) method.Invoke(command, new object[] { wi });
                }
                case 5:
                {
                    var command = (IWorkItemCommand)Activator.CreateInstance(type, _settings, _log, _client, _assets, _assetAssignment);
                    var method = command.GetType()
                        .GetMethod($"{wi.Action}Async", BindingFlags.Public | BindingFlags.Instance);
                    if (method == null) throw new MissingMethodException($"{wi.RecordType}SyncCommand is missing");
                    return await (Task<JObject>) method.Invoke(command, new object[] { wi });
                }
                default:
                    throw new MissingMethodException($"{wi.RecordType}SyncCommand is missing");
            }
        }
    }

    public static class OcError
    {
        public static string IdExists => "IdExists";
        public static string NotFound => "NotFound";
    }
}
