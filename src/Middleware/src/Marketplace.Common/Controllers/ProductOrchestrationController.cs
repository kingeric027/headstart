using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Common.Commands;
using Marketplace.Helpers.Models;
using Marketplace.Helpers;

namespace Marketplace.Common.Controllers
{
    [Route("orchestration/{clientId}")]
    public class ProductOrchestrationController : BaseController
    {
        private readonly IOrchestrationCommand _command;

        public ProductOrchestrationController(AppSettings settings, IOrchestrationCommand command) : base(settings)
        {
            _command = command;
        }

        [HttpPost, Route("catalog"), MarketplaceUserAuth()]
        public async Task<MarketplaceCatalog> PostCatalog([FromBody] MarketplaceCatalog obj, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID, clientId);
        }

        [HttpPost, Route("product"), MarketplaceUserAuth()]
        public async Task<MarketplaceProduct> PostProduct([FromBody] MarketplaceProduct obj, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID, clientId);
        }

        [HttpPost, Route("productfacet"), MarketplaceUserAuth()]
        public async Task<MarketplaceProductFacet> PostProductFacet([FromBody] MarketplaceProductFacet obj, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID, clientId);
        }

        [HttpPost, Route("priceschedule"), MarketplaceUserAuth()]
        public async Task<MarketplacePriceSchedule> PostPriceSchedule([FromBody] MarketplacePriceSchedule obj, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID, clientId);
        }

        [HttpPost, Route("productassignment"), MarketplaceUserAuth()]
        public async Task<MarketplaceProductAssignment> PostProductAssignment([FromBody] MarketplaceProductAssignment obj, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID, clientId);
        }

        [HttpPost, Route("spec"), MarketplaceUserAuth()]
        public async Task<MarketplaceSpec> PostSpec([FromBody] MarketplaceSpec obj, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID, clientId);
        }

        [HttpPost, Route("specoption"), MarketplaceUserAuth()]
        public async Task<MarketplaceSpecOption> PostSpecOption([FromBody] MarketplaceSpecOption obj, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID, clientId);
        }

        [HttpPost, Route("specproductassignment"), MarketplaceUserAuth()]
        public async Task<MarketplaceSpecProductAssignment> PostSpecProductAssignment([FromBody] MarketplaceSpecProductAssignment obj, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID, clientId);
        }

        [HttpPost, Route("catalogproductassignment"), MarketplaceUserAuth()]
        public async Task<MarketplaceCatalogAssignment> PostCatalogProductAssignment([FromBody] MarketplaceCatalogAssignment obj, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID, clientId);
        }
    }
}
