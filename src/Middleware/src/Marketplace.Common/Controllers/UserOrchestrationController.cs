using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Common.Commands;
using Marketplace.Common.Helpers;
using Marketplace.Common.Models;
using Marketplace.Helpers.Models;

namespace Marketplace.Common.Controllers
{
    public class UserOrchestrationController : BaseController
    {
        private readonly IOrchestrationCommand _command;

        public UserOrchestrationController(IAppSettings settings, IOrchestrationCommand command) : base(settings)
        {
            _command = command;
        }

        [HttpPost, Route("buyer"), MarketplaceUserAuth()]
        public async Task<OrchestrationBuyer> PostBuyer([FromBody] OrchestrationBuyer obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, obj.ID);
        }

        [HttpPost, Route("{buyerId}/user"), MarketplaceUserAuth()]
        public async Task<OrchestrationUser> PostUser([FromBody] OrchestrationUser obj, string buyerId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, buyerId);
        }

        [HttpPost, Route("{buyerId}/usergroup"), MarketplaceUserAuth()]
        public async Task<OrchestrationUserGroup> PostUserGroup([FromBody] OrchestrationUserGroup obj, string buyerId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, buyerId);
        }

        [HttpPost, Route("{buyerId}/usergroupassignment"), MarketplaceUserAuth()]
        public async Task<OrchestrationUserGroupAssignment> PostUserGroupAssignment([FromBody] OrchestrationUserGroupAssignment obj, string buyerId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, buyerId);
        }

        [HttpPost, Route("{buyerId}/address"), MarketplaceUserAuth()]
        public async Task<OrchestrationAddress> PostAddress([FromBody] OrchestrationAddress obj, string buyerId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, buyerId);
        }

        [HttpPost, Route("{buyerId}/addressassignment"), MarketplaceUserAuth()]
        public async Task<OrchestrationAddressAssignment> PostAddressAssignment([FromBody] OrchestrationAddressAssignment obj, string buyerId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, buyerId);
        }

        [HttpPost, Route("{buyerId}/costcenter"), MarketplaceUserAuth()]
        public async Task<OrchestrationCostCenter> PostCostCenter([FromBody] OrchestrationCostCenter obj, string buyerId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, buyerId);
        }
    }
}
