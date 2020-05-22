using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ordercloud.integrations.extensions;

namespace Marketplace.Common.Commands.ETL
{
    public interface IOrderOrchestrationCommand
    {
        Task<JObject> GetAsync(string ID, VerifiedUserContext user);
    }
    public class OrderOrchestrationCommand : IOrderOrchestrationCommand
    {
        private const string ASSEMBLY = "Marketplace.Common.Commands.";
        private readonly AppSettings _settings;

        public OrderOrchestrationCommand(AppSettings settings)
        {
            _settings = settings;
        }

        public async Task<JObject> GetAsync(string ID, VerifiedUserContext user)
        {
            var type = Type.GetType($"{ASSEMBLY}{user.SupplierID}Command", true);
            var command = (IWorkItemCommand)Activator.CreateInstance(type, _settings);
            var method = command.GetType().GetMethod($"GetAsync", BindingFlags.Public | BindingFlags.Instance);
            if (method == null) throw new MissingMethodException($"{user.SupplierID}Command is missing");

            return await (Task<JObject>)method.Invoke(command, new object[] { ID, user });
        }
    }
}
