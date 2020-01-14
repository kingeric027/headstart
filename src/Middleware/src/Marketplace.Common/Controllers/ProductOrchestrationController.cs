using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Common.Commands;
using Marketplace.Helpers.Models;
using Marketplace.Helpers;

namespace Marketplace.Common.Controllers
{
    public class ProductOrchestrationController : BaseController
    {
        private readonly IOrchestrationCommand _command;

        public ProductOrchestrationController(AppSettings settings, IOrchestrationCommand command) : base(settings)
        {
            _command = command;
        }

        [HttpPost, Route("catalog"), MarketplaceUserAuth()]
        public async Task<MarketplaceCatalog> PostCatalog([FromBody] MarketplaceCatalog obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID);
        }

        [HttpPost, Route("product"), MarketplaceUserAuth()]
        public async Task<MarketplaceProduct> PostProduct([FromBody] MarketplaceProduct obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID);
        }

        [HttpPost, Route("productfacet"), MarketplaceUserAuth()]
        public async Task<MarketplaceProductFacet> PostProductFacet([FromBody] MarketplaceProductFacet obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID);
        }

        [HttpPost, Route("priceschedule"), MarketplaceUserAuth()]
        public async Task<MarketplacePriceSchedule> PostPriceSchedule([FromBody] MarketplacePriceSchedule obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID);
        }

        [HttpPost, Route("productassignment"), MarketplaceUserAuth()]
        public async Task<MarketplaceProductAssignment> PostProductAssignment([FromBody] MarketplaceProductAssignment obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID);
        }

        [HttpPost, Route("spec"), MarketplaceUserAuth()]
        public async Task<MarketplaceSpec> PostSpec([FromBody] MarketplaceSpec obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID);
        }

        [HttpPost, Route("specoption"), MarketplaceUserAuth()]
        public async Task<MarketplaceSpecOption> PostSpecOption([FromBody] MarketplaceSpecOption obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID);
        }

        [HttpPost, Route("specproductassignment"), MarketplaceUserAuth()]
        public async Task<MarketplaceSpecProductAssignment> PostSpecProductAssignment([FromBody] MarketplaceSpecProductAssignment obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID);
        }

        [HttpPost, Route("catalogproductassignment"), MarketplaceUserAuth()]
        public async Task<MarketplaceCatalogAssignment> PostCatalogProductAssignment([FromBody] MarketplaceCatalogAssignment obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID);
        }
    }
}
