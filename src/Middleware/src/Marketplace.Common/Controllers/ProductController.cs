using System;
using System.Threading.Tasks;
using Marketplace.Common.Commands.Crud;
using Marketplace.Helpers;
using Marketplace.Helpers.Models;
using Marketplace.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;

namespace Marketplace.Common.Controllers
{
    [Route("product")]
    public class ProductController : BaseController
    {
        private readonly IMarketplaceProductCommand _command;
        public ProductController(AppSettings settings, IMarketplaceProductCommand command) : base(settings)
        {
            _command = command;
        }

        [HttpGet, Route("{id}"), MarketplaceUserAuth(ApiRole.ProductAdmin, ApiRole.ProductReader)]
        public async Task<MarketplaceProduct> Get(string id)
        {
            return await _command.Get(id, VerifiedUserContext);
        }

        [HttpGet, MarketplaceUserAuth(ApiRole.ProductAdmin, ApiRole.ProductReader)]
        public async Task<ListPage<MarketplaceProduct>> List(MarketplaceListArgs<MarketplaceProduct> args)
        {
            return await _command.List(args, VerifiedUserContext);
        }

        [HttpPost, MarketplaceUserAuth(ApiRole.ProductAdmin)]
        public async Task<MarketplaceProduct> Post([FromBody] MarketplaceProduct obj)
        {
            return await _command.Post(obj, this.VerifiedUserContext);
        }

        [HttpPut, Route("{id}"), MarketplaceUserAuth(ApiRole.ProductAdmin)]
        public async Task<MarketplaceProduct> Put([FromBody] MarketplaceProduct obj, string id)
        {
            return await _command.Put(id, obj, this.VerifiedUserContext);
        }

        [HttpPatch, Route("{id}"), MarketplaceUserAuth(ApiRole.ProductAdmin)]
        public async Task<PartialMarketplaceProduct> Patch([FromBody] PartialMarketplaceProduct obj, string id)
        {
            return await _command.Patch(obj, id, this.VerifiedUserContext);
        }

        [HttpDelete, Route("{id}"), MarketplaceUserAuth(ApiRole.ProductAdmin)]
        public async Task Delete(string id)
        {
            await _command.Delete(id, VerifiedUserContext);
        }
    }
}
