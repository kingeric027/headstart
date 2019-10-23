using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Common.Commands;
using Marketplace.Common.Helpers;
using Marketplace.Common.Models;

namespace Marketplace.Common.Controllers
{
    public class OrchestrationController : BaseController
    {
        private readonly IOrchestrationCommand _command;

        public OrchestrationController(IAppSettings settings, IOrchestrationCommand command) : base(settings)
        {
            _command = command;
        }

        [HttpPost, Route("product"), MarketplaceUserAuth()]
        public async Task<OrchestrationProduct> PostProduct([FromBody] OrchestrationProduct obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext);
        }

        [HttpPost, Route("productfacet"), MarketplaceUserAuth()]
        public async Task<OrchestrationProductFacet> PostProductFacet([FromBody] OrchestrationProductFacet obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext);
        }

        [HttpPost, Route("priceschedule"), MarketplaceUserAuth()]
        public async Task<OrchestrationPriceSchedule> PostPriceSchedule([FromBody] OrchestrationPriceSchedule obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext);
        }

        [HttpPost, Route("productassignment"), MarketplaceUserAuth()]
        public async Task<OrchestrationProductAssignment> PostProductAssignment([FromBody] OrchestrationProductAssignment obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext);
        }

        [HttpPost, Route("spec"), MarketplaceUserAuth()]
        public async Task<OrchestrationSpec> PostSpec([FromBody] OrchestrationSpec obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext);
        }

        [HttpPost, Route("specoption"), MarketplaceUserAuth()]
        public async Task<OrchestrationSpecOption> PostSpecOption([FromBody] OrchestrationSpecOption obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext);
        }

        [HttpPost, Route("specproductassignment"), MarketplaceUserAuth()]
        public async Task<OrchestrationSpecProductAssignment> PostSpecProductAssignment([FromBody] OrchestrationSpecProductAssignment obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext);
        }
    }
}
