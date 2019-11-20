using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Common.Commands;
using Marketplace.Common.Helpers;
using Marketplace.Common.Models;

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
            return await _command.SaveToQueue(obj, this.VerifiedUserContext);
        }

        [HttpPost, Route("user"), MarketplaceUserAuth()]
        public async Task<OrchestrationUser> PostUser([FromBody] OrchestrationUser obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext);
        }

        [HttpPost, Route("usergroup"), MarketplaceUserAuth()]
        public async Task<OrchestrationUserGroup> PostUserGroup([FromBody] OrchestrationUserGroup obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext);
        }

        [HttpPost, Route("usergroupassignment"), MarketplaceUserAuth()]
        public async Task<OrchestrationUserGroupAssignment> PostProductAssignment([FromBody] OrchestrationUserGroupAssignment obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext);
        }

        [HttpPost, Route("address"), MarketplaceUserAuth()]
        public async Task<OrchestrationAddress> PostSpec([FromBody] OrchestrationAddress obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext);
        }

        [HttpPost, Route("costcenter"), MarketplaceUserAuth()]
        public async Task<OrchestrationCostCenter> PostSpecOption([FromBody] OrchestrationCostCenter obj)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext);
        }
    }
}
