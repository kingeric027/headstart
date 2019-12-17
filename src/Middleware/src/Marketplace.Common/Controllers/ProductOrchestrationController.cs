using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Common.Commands;
using Marketplace.Common.Helpers;
using Marketplace.Common.Models;
using Marketplace.Helpers.Models;

namespace Marketplace.Common.Controllers
{
    public class ProductOrchestrationController : BaseController
    {
        private readonly IOrchestrationCommand _command;

        public ProductOrchestrationController(IAppSettings settings, IOrchestrationCommand command) : base(settings)
        {
            _command = command;
        }

        [HttpPost, Route("product"), MarketplaceUserAuth()]
        public async Task<OrchestrationProduct> PostProduct([FromBody] OrchestrationProduct obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID);
        }

        [HttpPost, Route("productfacet"), MarketplaceUserAuth()]
        public async Task<OrchestrationProductFacet> PostProductFacet([FromBody] OrchestrationProductFacet obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID);
        }

        [HttpPost, Route("priceschedule"), MarketplaceUserAuth()]
        public async Task<OrchestrationPriceSchedule> PostPriceSchedule([FromBody] OrchestrationPriceSchedule obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID);
        }

        [HttpPost, Route("productassignment"), MarketplaceUserAuth()]
        public async Task<OrchestrationProductAssignment> PostProductAssignment([FromBody] OrchestrationProductAssignment obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID);
        }

        [HttpPost, Route("spec"), MarketplaceUserAuth()]
        public async Task<OrchestrationSpec> PostSpec([FromBody] OrchestrationSpec obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID);
        }

        [HttpPost, Route("specoption"), MarketplaceUserAuth()]
        public async Task<OrchestrationSpecOption> PostSpecOption([FromBody] OrchestrationSpecOption obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID);
        }

        [HttpPost, Route("specproductassignment"), MarketplaceUserAuth()]
        public async Task<OrchestrationSpecProductAssignment> PostSpecProductAssignment([FromBody] OrchestrationSpecProductAssignment obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID);
        }

        [HttpPost, Route("catalogproductassignment"), MarketplaceUserAuth()]
        public async Task<OrchestrationCatalogAssignment> PostCatalogProductAssignment([FromBody] OrchestrationCatalogAssignment obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID);
        }
    }
}
