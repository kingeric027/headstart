using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Common.Commands;
using Marketplace.Helpers.Models;
using Marketplace.Helpers;

namespace Marketplace.Common.Controllers
{
    public class UserOrchestrationController : BaseController
    {
        private readonly IOrchestrationCommand _command;

        public UserOrchestrationController(AppSettings settings, IOrchestrationCommand command) : base(settings)
        {
            _command = command;
        }

        [HttpPost, Route("buyer"), MarketplaceUserAuth()]
        public async Task<MarketplaceBuyer> PostBuyer([FromBody] MarketplaceBuyer obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, obj.ID);
        }

        [HttpPost, Route("{buyerId}/user"), MarketplaceUserAuth()]
        public async Task<MarketplaceUser> PostUser([FromBody] MarketplaceUser obj, string buyerId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, buyerId);
        }

        [HttpPost, Route("{buyerId}/usergroup"), MarketplaceUserAuth()]
        public async Task<MarketplaceUserGroup> PostUserGroup([FromBody] MarketplaceUserGroup obj, string buyerId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, buyerId);
        }

        [HttpPost, Route("{buyerId}/usergroupassignment"), MarketplaceUserAuth()]
        public async Task<MarketplaceUserGroupAssignment> PostUserGroupAssignment([FromBody] MarketplaceUserGroupAssignment obj, string buyerId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, buyerId);
        }

        [HttpPost, Route("{buyerId}/address"), MarketplaceUserAuth()]
        public async Task<MarketplaceAddress> PostAddress([FromBody] MarketplaceAddress obj, string buyerId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, buyerId);
        }

        [HttpPost, Route("{buyerId}/addressassignment"), MarketplaceUserAuth()]
        public async Task<MarketplaceAddressAssignment> PostAddressAssignment([FromBody] MarketplaceAddressAssignment obj, string buyerId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, buyerId);
        }

        [HttpPost, Route("{buyerId}/costcenter"), MarketplaceUserAuth()]
        public async Task<MarketplaceCostCenter> PostCostCenter([FromBody] MarketplaceCostCenter obj, string buyerId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, buyerId);
        }
    }
}
