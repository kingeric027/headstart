using System.Threading.Tasks;
using Marketplace.Models;
using Newtonsoft.Json.Linq;
using ordercloud.integrations.extensions;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.SupplierSync
{
    public class MPSupplierFASTPlatformCommand : ISupplierSyncCommand
    {
        private readonly IOrderCloudClient _oc;

        public MPSupplierFASTPlatformCommand(AppSettings settings, IOrderCloudClient oc)
        {
            _oc = oc;
        }

        public async Task<JObject> GetOrderAsync(string ID, VerifiedUserContext user)
        {
            var order = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, ID, user.AccessToken);
            return JObject.FromObject(order);
        }
    }
}
