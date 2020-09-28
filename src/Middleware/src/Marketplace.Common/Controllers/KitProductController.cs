using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Models.Attributes;
using ordercloud.integrations.library;
using Marketplace.Models;
using Marketplace.Common.Commands.Crud;
using Marketplace.Common.Models.Marketplace;
using ordercloud.integrations.cms;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Kit Products\" represents Kit Products for Marketplace")]
    [MarketplaceSection.Marketplace(ListOrder = 3)]
    [Route("kitproducts")]
    public class KitProductController : BaseController
    {
        private readonly IMarketplaceKitProductCommand _command;
        public KitProductController(AppSettings settings, IMarketplaceKitProductCommand command) : base(settings)
        {
            _command = command;
        }

        [DocName("GET Kit Product")]
        [HttpGet, Route("{id}"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
        public async Task<MarketplaceKitProduct> Get(string id)
        {
            return await _command.Get(id, VerifiedUserContext);
        }
        [DocName("GET Me Kit Product")]
        [HttpGet, Route("me/{id}"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
        public async Task<MarketplaceMeKitProduct> GetMeKit(string id)
        {
            return await _command.GetMeKit(id, VerifiedUserContext);
        }
        [DocName("POST Kit Product")]
        [HttpPost, OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
        public async Task<MarketplaceKitProduct> Post([FromBody] MarketplaceKitProduct kitProduct)
        {
            return await _command.Post(kitProduct, VerifiedUserContext);
        }
        [DocName("LIST Kit Products")]
        [HttpGet, OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
        public async Task<ListPage<MarketplaceKitProduct>> List(ListArgs<Document<KitProduct>> args)
        {
            return await _command.List(args, VerifiedUserContext);
        }
        [DocName("LIST Me Kit Products")]
        [HttpGet, Route("me"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
        public async Task<ListPage<MarketplaceKitProduct>> ListMeKits(ListArgs<Document<KitProduct>> args)
        {
            return await _command.List(args, VerifiedUserContext);
        }
        [DocName("PUT Kit Product")]
        [HttpPut, Route("{id}"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
        public async Task<MarketplaceKitProduct> Put([FromBody] MarketplaceKitProduct kitProduct, string id)
        {
            return await _command.Put(id, kitProduct, VerifiedUserContext);
        }

        [DocName("DELETE Kit Product")]
        [HttpDelete, Route("{id}"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
        public async Task Delete(string id)
        {
            await _command.Delete(id, VerifiedUserContext);
        }
    }
}
