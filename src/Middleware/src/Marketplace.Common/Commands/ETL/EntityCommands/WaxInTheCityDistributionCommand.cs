using Newtonsoft.Json.Linq;
using ordercloud.integrations.extensions;
using System.Threading.Tasks;
using Marketplace.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.ETL.EntityCommands
{
    public class WaxnTheCityDistributionCommand : IOrderSyncCommand
    {
        private readonly IOrderCloudClient _oc;

        public WaxnTheCityDistributionCommand(AppSettings settings, IOrderCloudClient oc)
        {
            _oc = oc;
        }

        public async Task<JObject> GetAsync(string ID, VerifiedUserContext user)
        {
            var order = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, ID, user.AccessToken);
            return JObject.FromObject(order);
        }
    }
}
