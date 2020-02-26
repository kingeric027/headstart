using System.Threading.Tasks;
using Marketplace.Common.Commands.Crud;
using Marketplace.Helpers;
using Marketplace.Helpers.SwaggerTools;
using Marketplace.Models;
using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Products\" represents products for Marketplace")]
    [MarketplaceSection.ProductCatalogs(ListOrder = 1)]
    [Route("products")]
    public class ProductController : BaseController
    {
        private readonly IMarketplaceProductCommand _command;
        public ProductController(AppSettings settings, IMarketplaceProductCommand command) : base(settings)
        {
            _command = command;
        }

        [DocName("GET Product")]
        [HttpGet, Route("{id}"), MarketplaceUserAuth(ApiRole.ProductAdmin, ApiRole.ProductReader)]
        public async Task<SuperMarketplaceProduct> Get(string id)
        {
            return await _command.Get(id, VerifiedUserContext);
        }

        [DocName("LIST Product")]
        [HttpGet, MarketplaceUserAuth(ApiRole.ProductAdmin, ApiRole.ProductReader)]
        public async Task<ListPage<SuperMarketplaceProduct>> List(ListArgs<MarketplaceProduct> args)
        {
            return await _command.List(args, VerifiedUserContext);
        }

        [DocName("POST Product")]
        [HttpPost, MarketplaceUserAuth(ApiRole.ProductAdmin)]
        public async Task<SuperMarketplaceProduct> Post([FromBody] SuperMarketplaceProduct obj)
        {
            return await _command.Post(obj, this.VerifiedUserContext);
        }

        [DocName("PUT Product")]
        [HttpPut, Route("{id}"), MarketplaceUserAuth(ApiRole.ProductAdmin)]
        public async Task<SuperMarketplaceProduct> Put([FromBody] SuperMarketplaceProduct obj, string id)
        {
            return await _command.Put(id, obj, this.VerifiedUserContext);
        }

        [DocName("DELETE Product")]
        [HttpDelete, Route("{id}"), MarketplaceUserAuth(ApiRole.ProductAdmin)]
        public async Task Delete(string id)
        {
            await _command.Delete(id, VerifiedUserContext);
        }
    }
}
