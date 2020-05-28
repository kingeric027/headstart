using System.Threading.Tasks;
using Marketplace.Models;
using Newtonsoft.Json.Linq;
using ordercloud.integrations.extensions;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.SupplierSync
{
    // class name will likely need to be changed for production supplier ID
    public class MPSupplier007Command : ISupplierSyncCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly IOrderCloudClient _ocSeller;
        private AppSettings _settings;

        public MPSupplier007Command(AppSettings settings, IOrderCloudClient oc)
        {
            _oc = oc;
            _settings = settings;

            // investigate potentially injecting this client at startup, didn't have any success when trying
            _ocSeller = new OrderCloudClient(new OrderCloudClientConfig
            {
                ApiUrl = _settings.OrderCloudSettings.ApiUrl,
                AuthUrl = _settings.OrderCloudSettings.AuthUrl,
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
            var order = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, ID, user.AccessToken);
            var buyerOrder = await _ocSeller.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, ID.Split('-')[0]);

            var returnObject = new JObject();
            returnObject.Add("Order", JToken.FromObject(order));
            returnObject.Add("BuyerBillingAddress", JToken.FromObject(buyerOrder.BillingAddress));
            return JObject.FromObject(returnObject);
        }
    }
}
