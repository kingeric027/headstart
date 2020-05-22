using Newtonsoft.Json.Linq;
using ordercloud.integrations.extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Marketplace.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.ETL.EntityCommands
{
    public class ExampleCommand : IOrderOrchestrationCommand
    {
        private IOrderCloudClient _oc;
        public ExampleCommand(AppSettings settings, IOrderCloudClient oc)
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
