using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Marketplace.Common.Mappers.Crud;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.Crud
{
    public interface IMarketplaceProductCrudCommand
    {
        Task<MarketplaceProduct> Post(MarketplaceProduct product, VerifiedUserContext user);
        Task<MarketplaceProduct> Patch(Partial<MarketplaceProduct> product, string id, VerifiedUserContext user);
    }

    public class MarketplaceProductCrudCommand : IMarketplaceProductCrudCommand
    {
        private readonly IOrderCloudClient _oc;
        public MarketplaceProductCrudCommand(AppSettings settings, IOrderCloudClient oc)
        {
            _oc = new OrderCloudClient(new OrderCloudClientConfig()
            {
                ApiUrl = "https://api.ordercloud.io",
                AuthUrl = "https://auth.ordercloud.io",
            });
        }

        public async Task<MarketplaceProduct> Post(MarketplaceProduct product, VerifiedUserContext user)
        {
            var model = MarketplaceProductMapper.Map(product);
            var p = await _oc.Products.CreateAsync(model, user.AccessToken);
            var result = MarketplaceProductMapper.Map(p);
            return result;
        }

        public async Task<MarketplaceProduct> Patch(Partial<MarketplaceProduct> product, string id, VerifiedUserContext user)
        {
            var model = MarketplaceProductMapper.Map(product);
            var p = await _oc.Products.PatchAsync(id, model, user.AccessToken);
            var result = MarketplaceProductMapper.Map(p);
            return result;
        }
    }
}
