using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ordercloud.integrations.extensions;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.ETL
{
    public interface IOrderSyncCommand
    {
        Task<JObject> GetAsync(string ID, VerifiedUserContext user);
    }

    public class OrderSyncCommand : IOrderSyncCommand
    {
        private const string ASSEMBLY = "Marketplace.Common.Commands.ETL.EntityCommands.";
        private readonly AppSettings _settings;

        public OrderSyncCommand(AppSettings settings)
        {
            _settings = settings;
        }

        public async Task<JObject> GetAsync(string ID, VerifiedUserContext user)
        {
            try
            {
                var oc = new OrderCloudClient(new OrderCloudClientConfig() {ClientId = user.ClientID});
                var type = Type.GetType($"{ASSEMBLY}{user.SupplierID.ToLower()}Command", true, ignoreCase: true);
                var command = (IOrderSyncCommand) Activator.CreateInstance(type, _settings, oc);
                var method = command.GetType().GetMethod($"GetAsync", BindingFlags.Public | BindingFlags.Instance);
                if (method == null) throw new MissingMethodException($"{user.SupplierID}Command is missing");

                return await (Task<JObject>) method.Invoke(command, new object[] {ID, user});
            }
            catch (Exception ex)
            {
                throw new MissingMethodException($"{user.SupplierID}Command is missing");
            }
        }
    }
}
