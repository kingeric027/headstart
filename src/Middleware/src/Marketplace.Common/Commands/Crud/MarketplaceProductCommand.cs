using System;
using System.Threading.Tasks;
using Marketplace.Common.Mappers;
using Marketplace.Helpers;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.Crud
{
    public interface IMarketplaceProductCommand
    {
        Task<MarketplaceProduct> Get(string id, VerifiedUserContext user);
        Task<MarketplaceListPage<MarketplaceProduct>> List(MarketplaceListArgs<MarketplaceProduct> args, VerifiedUserContext user);
        Task<MarketplaceProduct> Post(MarketplaceProduct product, VerifiedUserContext user);
        Task<MarketplaceProduct> Put(string id, MarketplaceProduct product, VerifiedUserContext user);
        Task<MarketplaceProduct> Patch(Partial<MarketplaceProduct> product, string id, VerifiedUserContext user);
        Task Delete(string id, VerifiedUserContext user);

    }

    public class MarketplaceProductCommand : IMarketplaceProductCommand
    {
        private readonly IOrderCloudClient _oc;
        public MarketplaceProductCommand(AppSettings settings)
        {
            _oc = new OrderCloudClient(new OrderCloudClientConfig()
            {
                ApiUrl = "https://api.ordercloud.io",
                AuthUrl = "https://auth.ordercloud.io",
            });
        }

        public async Task<MarketplaceProduct> Get(string id, VerifiedUserContext user)
        {
            var ocProduct = await _oc.Products.GetAsync(id, user.AccessToken);
            return MarketplaceProductMapper.Map(ocProduct);
        }

        public async Task<MarketplaceListPage<MarketplaceProduct>> List(MarketplaceListArgs<MarketplaceProduct> args, VerifiedUserContext user)
        {
            var list = await _oc.Products.ListAsync(filters: args, accessToken: user.AccessToken);
            return MarketplaceProductMapper.Map(list);
        }

        public async Task<MarketplaceProduct> Post(MarketplaceProduct product, VerifiedUserContext user)
        {
            var model = MarketplaceProductMapper.Map(product);
            var p = await _oc.Products.CreateAsync(model, user.AccessToken);
            return MarketplaceProductMapper.Map(p);
        }

        public async Task<MarketplaceProduct> Put(string id, MarketplaceProduct product, VerifiedUserContext user)
        {
            var model = MarketplaceProductMapper.Map(product);
            var p = await _oc.Products.SaveAsync(id, model, user.AccessToken);
            return MarketplaceProductMapper.Map(p);
        }

        public async Task<MarketplaceProduct> Patch(Partial<MarketplaceProduct> product, string id, VerifiedUserContext user)
        {
            var model = MarketplaceProductMapper.Map(product);
            var p = await _oc.Products.PatchAsync(id, model, user.AccessToken);
            return MarketplaceProductMapper.Map(p);
        }

        public async Task Delete(string id, VerifiedUserContext user)
        {
            await _oc.Products.DeleteAsync(id, user.AccessToken);
        }
    }
}
