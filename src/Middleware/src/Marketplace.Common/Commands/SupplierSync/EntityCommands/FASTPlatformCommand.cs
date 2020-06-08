using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.SupplierSync
{
    public class FASTPlatformCommand : ISupplierSyncCommand
    {
        private readonly IOrderCloudClient _oc;

        public FASTPlatformCommand(AppSettings settings, IOrderCloudClient oc)
        {
            _oc = oc;
        }

        public async Task<JObject> GetOrderAsync(string ID, VerifiedUserContext user)
        {
            var order = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, ID, user.AccessToken);
            return JObject.FromObject(order);
        }

        public Task<List<TemplateHydratedProduct>> ParseProductTemplate(IFormFile file, VerifiedUserContext user)
        {
            throw new System.NotImplementedException();
        }
    }
}
