using Marketplace.Common.Commands;
using Marketplace.Helpers;
using Marketplace.Helpers.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;

namespace Marketplace.Common.Controllers
{
    [Route("products")]
    public class ProductController : BaseController
    {
        private readonly IOrchestrationCommand _command;
        public ProductController(AppSettings settings, IOrchestrationCommand command) : base(settings)
        {
            _command = command;
        }
        #region CRUD
        [HttpPost, Route(""), MarketplaceUserAuth()]
        public async Task<MarketplaceProduct> PostProduct([FromBody] MarketplaceProduct obj)
        {
            return await Task.FromResult(new MarketplaceProduct());
        }

        [Route("{productID}"), HttpGet, MarketplaceUserAuth(ApiRole.ProductAdmin)]
        public string Get(string productID)
        {
            return $"Successful `GET /product/{productID}`";
        }

        [Route("{productID}"), HttpPut, MarketplaceUserAuth(ApiRole.ProductAdmin)]
        // Need to be changed to Partial
        public string Put(string productID, MarketplaceProduct product)
        {
            return $"Successful `PUT /product/{productID}` with a body of {product}";
        }
        // Need to be changed to Partial
        [Route("{productID}"), HttpPatch, MarketplaceUserAuth(ApiRole.ProductAdmin)]
        public string Patch(string productID, MarketplaceProduct product)
        {
            return $"Successful `PATCH /product/{productID}` with body of {product}";
        }

        [Route("{productID}"), HttpDelete, MarketplaceUserAuth(ApiRole.ProductAdmin)]
        public string Delete(string productID)
        {
            return $"Successful `DELETE /product/{productID}`";
        }
        #endregion
    }
}