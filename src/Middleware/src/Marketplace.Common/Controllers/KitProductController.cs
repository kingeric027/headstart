using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Models.Attributes;
using ordercloud.integrations.library;
using Marketplace.Models;
using Marketplace.Common.Commands.Crud;
using Marketplace.Common.Models.Marketplace;

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
        [HttpGet, Route("{id}"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin, ApiRole.ProductReader)]
        public async Task<MarketplaceKitProduct> Get(string id)
        {
            return await _command.Get(id, VerifiedUserContext);
        }
        [DocName("POST Kit Product")]
        [HttpPost, OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
        public async Task<MarketplaceKitProduct> Post([FromBody] MarketplaceKitProductDocument doc, MarketplaceKitProduct kit)
        {
            return await _command.Post(doc, kit, VerifiedUserContext);
        }
        [DocName("LIST Kit Product")]
        [HttpGet, OrderCloudIntegrationsAuth(ApiRole.ProductAdmin, ApiRole.ProductReader)]
        public async Task<ListPage<MarketplaceKitProduct>> List(ListArgs<MarketplaceProduct> args)
        {
            return await _command.List(args, VerifiedUserContext);
        }
        [DocName("PUT Kit Product")]
        [HttpPut, Route("{id}"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
        public async Task<MarketplaceKitProduct> Put([FromBody] MarketplaceKitProductDocument doc, MarketplaceKitProduct obj, string id)
        {
            return await _command.Put(id, doc, obj, VerifiedUserContext);
        }

        [DocName("DELETE Kit Product")]
        [HttpDelete, Route("{id}"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
        public async Task Delete(string id)
        {
            await _command.Delete(id, VerifiedUserContext);
        }
    }
}
