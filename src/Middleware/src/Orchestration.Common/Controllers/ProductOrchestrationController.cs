using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orchestration.Common.Commands;
using Orchestration.Common.Helpers;
using Orchestration.Common.Models;

namespace Orchestration.Common.Controllers
{
    public class OrchestrationController : BaseController
    {
        private readonly IOrchestrationCommand _command;

        public OrchestrationController(IAppSettings settings, IOrchestrationCommand command) : base(settings)
        {
            _command = command;
        }

        [HttpPost, Route("product"), OrchestrationUserAuth()]
        public async Task<OrchestrationProduct> PostProduct([FromBody] OrchestrationProduct obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext.SupplierID);
        }

        [HttpPost, Route("productfacet"), OrchestrationUserAuth()]
        public async Task<OrchestrationProductFacet> PostProductFacet([FromBody] OrchestrationProductFacet obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext.SupplierID);
        }

        [HttpPost, Route("priceschedule"), OrchestrationUserAuth()]
        public async Task<OrchestrationPriceSchedule> PostPriceSchedule([FromBody] OrchestrationPriceSchedule obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext.SupplierID);
        }

        [HttpPost, Route("productassignment"), OrchestrationUserAuth()]
        public async Task<OrchestrationProductAssignment> PostProductAssignment([FromBody] OrchestrationProductAssignment obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext.SupplierID);
        }

        [HttpPost, Route("spec"), OrchestrationUserAuth()]
        public async Task<OrchestrationSpec> PostSpec([FromBody] OrchestrationSpec obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext.SupplierID);
        }

        [HttpPost, Route("specoption"), OrchestrationUserAuth()]
        public async Task<OrchestrationSpecOption> PostSpecOption([FromBody] OrchestrationSpecOption obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext.SupplierID);
        }

        [HttpPost, Route("specproductassignment"), OrchestrationUserAuth()]
        public async Task<OrchestrationSpecProductAssignment> PostSpecProductAssignment([FromBody] OrchestrationSpecProductAssignment obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext.SupplierID);
        }
    }
}
