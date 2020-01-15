using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Marketplace.Common.Commands.Crud;
using Marketplace.Helpers;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;

namespace Marketplace.Common.Controllers
{
    [Route("product")]
    public class ProductController : BaseController
    {
        private readonly IMarketplaceProductCrudCommand _command;
        public ProductController(AppSettings settings, IMarketplaceProductCrudCommand command) : base(settings)
        {
            _command = command;
        }

        [HttpPost]
        public async Task<MarketplaceProduct> Post([FromBody] MarketplaceProduct obj)
        {
            var model = await _command.Post(obj, this.VerifiedUserContext);
            return model;
        }

        [HttpPatch, Route("{id}"), MarketplaceUserAuth(ApiRole.ProductAdmin)]
        public async Task<MarketplaceProduct> Patch([FromBody] Partial<MarketplaceProduct> obj, string id)
        {
            var p = await _command.Patch(obj, id, this.VerifiedUserContext);
            return await Task.FromResult(p);
        }

        [HttpGet, Route("{id}"), MarketplaceUserAuth(ApiRole.ProductAdmin, ApiRole.ProductReader)]
        public async Task<MarketplaceProduct> Get(string id)
        {
            return await Task.FromResult(new MarketplaceProduct());
        }
    }
}
