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
        Task<List<MarketplaceHydratedProduct>> ParseProductTemplate(IFormFile file, VerifiedUserContext user);
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

        public async Task<List<MarketplaceHydratedProduct>> ParseProductTemplate(IFormFile file, VerifiedUserContext user)
        {
            using var stream = file.OpenReadStream();
            var mapper = new Mapper(stream);
            var products = mapper.Take<TemplateProduct>("Products").ToList();
            var prices = mapper.Take<TemplatePriceSchedule>("PriceSchedules").ToList();
            var specs = mapper.Take<TemplateSpec>("Specs").ToList();
            var specoptions = mapper.Take<TemplateSpecOption>("SpecOptions").ToList();
            var images = mapper.Take<TemplateAsset>("Images").ToList();
            var attachments = mapper.Take<TemplateAsset>("Attachments").ToList();

            var list = products.Select(info => new MarketplaceHydratedProduct()
            {
                Product = info.Value,
                PriceSchedule = prices.FirstOrDefault(row => row.Value.ProductID == info.Value.ID)?.Value,
                Specs = specs.Where(s => s.Value.ProductID == info.Value.ID).Select(s =>
                {
                    s.Value.SpecOptions = specoptions.Where(o => o.Value.SpecID == s.Value.ID).Select(o => o.Value).ToList();
                    return s.Value;
                }).ToList(),
                Images = images.Where(s => s.Value.ProductID == info.Value.ID).Select(o => o.Value).ToList(),
                Attachments = attachments.Where(s => s.Value.ProductID == info.Value.ID).Select(o => o.Value).ToList()
            });
            return await Task.FromResult(list.ToList());
        }
    }

    public class MarketplaceHydratedProduct
    {
        public TemplateProduct Product { get; set; }
        public TemplatePriceSchedule PriceSchedule { get; set; }
        public IList<TemplateSpec> Specs { get; set; }
        public IList<TemplateAsset> Images { get; set; }
        public IList<TemplateAsset> Attachments { get; set; }
    }

    public class TemplateProduct : MarketplaceProduct
    {

    }

    public class TemplatePriceSchedule : MarketplacePriceSchedule
    {
        public string ProductID { get; set; }
    }

    public class TemplateSpec : MarketplaceSpec
    {
        public string ProductID { get; set; }
        public IList<TemplateSpecOption> SpecOptions { get; set; }
    }

    public class TemplateSpecOption : MarketplaceSpecOption
    {
        public string SpecID { get; set; }
    }

    public class TemplateAsset : Asset
    {
        public string ProductID { get; set; }
    }
}
