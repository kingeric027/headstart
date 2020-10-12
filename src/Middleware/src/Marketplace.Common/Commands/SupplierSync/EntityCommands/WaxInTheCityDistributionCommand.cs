using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents.SystemFunctions;
using Microsoft.EntityFrameworkCore.Internal;
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
        }

        public async Task<JObject> GetOrderAsync(string ID, VerifiedUserContext user)
        {
            var supplierWorksheet = await _ocSeller.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Outgoing, ID);
            
            var buyerWorksheet = await _ocSeller.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Incoming, ID.Split('-')[0]);
            var buyerLineItems = buyerWorksheet.LineItems.Where(li => li.SupplierID == supplierWorksheet.Order.ToCompanyID).Select(li => li);
            var estimate = buyerWorksheet.ShipEstimateResponse.ShipEstimates.FirstOrDefault(e => e.ShipEstimateItems.Any(i => i.LineItemID == buyerLineItems.FirstOrDefault()?.ID));
            var ship_method = estimate?.ShipMethods.FirstOrDefault(m => m.ID == estimate.SelectedShipMethodID);

            var returnObject = new JObject
            {
                { "SupplierOrder", new JObject {
                    {"Order", JToken.FromObject(supplierWorksheet.Order)},
                    new JProperty("LineItems", JToken.FromObject(supplierWorksheet.LineItems))
                }},
                { "BuyerOrder", new JObject {
                    {"Order", JToken.FromObject(buyerWorksheet.Order)},
                    new JProperty("LineItems", JToken.FromObject(buyerLineItems))
                }},
                { "ShipMethod", JToken.FromObject(ship_method)},
            };
            return JObject.FromObject(returnObject);
        }

        public Task<List<TemplateHydratedProduct>> ParseProductTemplate(IFormFile file, VerifiedUserContext user)
        {
            throw new System.NotImplementedException();
        }
    }
}
