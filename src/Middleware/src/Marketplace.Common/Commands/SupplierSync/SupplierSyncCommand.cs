using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ganss.Excel;
using Marketplace.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npoi.Mapper;
using NPOI.XSSF.UserModel;
using ordercloud.integrations.cms;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.SupplierSync
{
    public interface ISupplierSyncCommand
    {
        Task<JObject> GetOrderAsync(string ID, VerifiedUserContext user);
        Task<List<SuperMarketplaceProduct>> ParseProductTemplate(IFormFile file, VerifiedUserContext user);
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

        public async Task<List<SuperMarketplaceProduct>> ParseProductTemplate(IFormFile file, VerifiedUserContext user)
        {
            using var stream = file.OpenReadStream();
            var mapper = new Mapper(stream);
            var products = mapper.Take<MarketplaceProduct>("Products").ToList();
            var prices = mapper.Take<MarketplacePriceSchedule>("PriceSchedules").ToList();
            var specs = mapper.Take<MarketplaceSpec>("Specs").ToList();
            var specoptions = mapper.Take<MarketplaceSpecOption>("SpecOptions").ToList();
            var images = mapper.Take<Asset>("Images").ToList();
            var attachments = mapper.Take<Asset>("Attachments").ToList();

            var list = products.Select(info => new SuperMarketplaceProduct()
            {
                Product = info.Value,
                PriceSchedule = prices.FirstOrDefault(row => row.Value.ID == info.Value.DefaultPriceScheduleID)?.Value
            });
            return await Task.FromResult(list.ToList());
        }
    }
}
