using System.Threading.Tasks;
using Marketplace.Helpers;
using Marketplace.Helpers.Models;
using Marketplace.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.Crud
{
    public interface IMarketplaceProductCommand
    {
        Task<MarketplaceProduct> Get(string id, VerifiedUserContext user);
        Task<ListPage<MarketplaceProduct>> List(ListArgs<MarketplaceProduct> args, VerifiedUserContext user);
        Task<MarketplaceProduct> Post(MarketplaceProduct product, VerifiedUserContext user);
        Task<MarketplaceProduct> Put(string id, MarketplaceProduct product, VerifiedUserContext user);
        Task<PartialMarketplaceProduct> Patch(PartialMarketplaceProduct product, string id, VerifiedUserContext user);
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
            return await _oc.Products.GetAsync<MarketplaceProduct>(id, user.AccessToken);
        }

        public async Task<ListPage<MarketplaceProduct>> List(ListArgs<MarketplaceProduct> args, VerifiedUserContext user)
        {
            return await _oc.Products.ListAsync<MarketplaceProduct>(filters: args, accessToken: user.AccessToken);
        }

        public async Task<MarketplaceProduct> Post(MarketplaceProduct product, VerifiedUserContext user)
        {
            return await _oc.Products.CreateAsync<MarketplaceProduct>(product, user.AccessToken);
        }

        public async Task<MarketplaceProduct> Put(string id, MarketplaceProduct product, VerifiedUserContext user)
        {
            return await _oc.Products.SaveAsync<MarketplaceProduct>(id, product, user.AccessToken);
        }

        public async Task<PartialMarketplaceProduct> Patch(PartialMarketplaceProduct product, string id, VerifiedUserContext user)
        {
            return await _oc.Products.PatchAsync<PartialMarketplaceProduct>(id, product, user.AccessToken);
        }

        public async Task Delete(string id, VerifiedUserContext user)
        {
            await _oc.Products.DeleteAsync(id, user.AccessToken);
        }
    }
}
