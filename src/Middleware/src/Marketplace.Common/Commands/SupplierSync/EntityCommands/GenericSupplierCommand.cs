using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.SupplierSync
{
    [SupplierSync("Generic")]
    public class GenericSupplierCommand : ISupplierSyncCommand
    {
        private readonly IOrderCloudClient _ocSeller;

        public GenericSupplierCommand(AppSettings settings)
        {
            _ocSeller = new OrderCloudClient(new OrderCloudClientConfig
            {
                ApiUrl = settings.OrderCloudSettings.ApiUrl,
                AuthUrl = settings.OrderCloudSettings.ApiUrl,
                ClientId = settings.OrderCloudSettings.ClientID,
                ClientSecret = settings.OrderCloudSettings.ClientSecret,
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
