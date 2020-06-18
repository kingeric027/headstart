using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Marketplace.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npoi.Mapper;
using ordercloud.integrations.cms;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.SupplierSync
{
    public interface ISupplierSyncCommand
    {
        Task<JObject> GetOrderAsync(string ID, VerifiedUserContext user);
    }

    public class SupplierSyncCommand : ISupplierSyncCommand
    {
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

                var type = Assembly.GetExecutingAssembly().GetTypeByAttribute<SupplierSyncAttribute>(attribute => attribute.SupplierID == user.SupplierID); 
                if (type == null) throw new MissingMethodException($"Command for {user.SupplierID} is unavailable");

                var command = (ISupplierSyncCommand) Activator.CreateInstance(type, _settings, oc);
                var method = command.GetType().GetMethod($"GetOrderAsync", BindingFlags.Public | BindingFlags.Instance);
                if (method == null)
                    throw new MissingMethodException($"Get Order Method for {user.SupplierID} is unavailable");

                return await (Task<JObject>) method.Invoke(command, new object[] {ID, user});
            }
            catch (MissingMethodException mex)
            {
                throw new Exception(JsonConvert.SerializeObject(new ApiError()
                {
                    Data = new {user, OrderID = ID},
                    ErrorCode = mex.Message,
                    Message = $"Missing Method for: {user.SupplierID ?? "Invalid Supplier"}"
                }));
            }
        }
    }
}
