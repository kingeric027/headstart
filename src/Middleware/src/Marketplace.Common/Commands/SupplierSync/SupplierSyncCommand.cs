using System;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ordercloud.integrations.extensions;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.SupplierSync
{
    public interface ISupplierSyncCommand
    {
        Task<JObject> GetOrderAsync(string ID, VerifiedUserContext user);
    }

    public class SupplierSyncCommand : ISupplierSyncCommand
    {
        private const string ASSEMBLY = "Marketplace.Common.Commands.SupplierSync.";
        private readonly AppSettings _settings;

        public SupplierSyncCommand(AppSettings settings)
        {
            _settings = settings;
        }

        public async Task<JObject> GetOrderAsync(string ID, VerifiedUserContext user)
        {
            try
            {
                var oc = new OrderCloudClient(new OrderCloudClientConfig()
                {
                    AuthUrl = _settings.OrderCloudSettings.AuthUrl,
                    ApiUrl = _settings.OrderCloudSettings.ApiUrl,
                    ClientId = user.ClientID
                });

                /*
                 * first character in c# class cannot be a number, to handle suppliers 
                 * with IDs starting in a number we are prepending it with 
                 * MPSupplier which is an arbitrary string
                 */
                var type = Type.GetType($"{ASSEMBLY}MPSupplier{user.SupplierID.ToLower()}Command", true, ignoreCase: true);
                var command = (ISupplierSyncCommand) Activator.CreateInstance(type, _settings, oc);
                var method = command.GetType().GetMethod($"GetOrderAsync", BindingFlags.Public | BindingFlags.Instance);
                if (method == null) throw new MissingMethodException($"{user.SupplierID}Command is missing");

                return await (Task<JObject>) method.Invoke(command, new object[] {ID, user});
            }
            catch
            {
                throw new MissingMethodException($"{user.SupplierID}Command is missing");
            }
        }
    }
}
