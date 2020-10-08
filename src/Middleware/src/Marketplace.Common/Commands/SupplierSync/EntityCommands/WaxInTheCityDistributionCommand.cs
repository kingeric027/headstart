using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents.SystemFunctions;
using Newtonsoft.Json.Linq;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.SupplierSync
{
    [SupplierSync("027"), SupplierSync("093"), SupplierSync("waxinthecitydistribution")]
    public class WaxInTheCityDistributionCommand : ISupplierSyncCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly IOrderCloudClient _ocSeller;
        private AppSettings _settings;

        public WaxInTheCityDistributionCommand(AppSettings settings, IOrderCloudClient oc)
        {
            _oc = oc;
            _settings = settings;

            // investigate potentially injecting this client at startup, didn't have any success when trying
            _ocSeller = new OrderCloudClient(new OrderCloudClientConfig
            {
                ApiUrl = _settings.OrderCloudSettings.ApiUrl,
                AuthUrl = _settings.OrderCloudSettings.ApiUrl,
                ClientId = _settings.OrderCloudSettings.ClientID,
                ClientSecret = _settings.OrderCloudSettings.ClientSecret,
                Roles = new[]
                        {
                    ApiRole.FullAccess
                }
            });
            //7A8002F4-E22F-4A3A-A091-E8B531BB5010
            //dwKGiqiqxBHILnzJSNgvpgMI4mBzVuJqJEt28AIZNCYWxL343st33v46kCL4
        }

        public async Task<JObject> GetOrderAsync(string ID, VerifiedUserContext user)
        {
            var supplierOrder = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, ID, user.AccessToken);
            var supplierLineItems = await _oc.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Incoming, supplierOrder.ID, pageSize: 100, accessToken: user.AccessToken);
            var buyerOrder = await _ocSeller.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, ID.Split('-')[0]);
            var buyerLineItems = await _ocSeller.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Incoming, buyerOrder.ID);
            
            var returnObject = new JObject
            {
                {"SupplierOrder", new JObject {
                    {"Order", JToken.FromObject(supplierOrder)},
                    new JProperty("LineItems", JToken.FromObject(supplierLineItems.Items))
                }},
                {"BuyerOrder", new JObject {
                    {"Order", JToken.FromObject(buyerOrder)},
                    new JProperty("LineItems", JToken.FromObject(buyerLineItems.Items.Where(li => li.SupplierID == supplierOrder.ToCompanyID).Select(li => li)))
                }},
                {"Order", JToken.FromObject(supplierOrder)},
                {"BuyerBillingAddress", JToken.FromObject(buyerOrder.BillingAddress)}
            };
            return JObject.FromObject(returnObject);
        }

        public Task<List<TemplateHydratedProduct>> ParseProductTemplate(IFormFile file, VerifiedUserContext user)
        {
            throw new System.NotImplementedException();
        }
    }
}
