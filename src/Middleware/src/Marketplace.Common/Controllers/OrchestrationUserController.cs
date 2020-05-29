﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Common.Commands;
using Marketplace.Models;
using Marketplace.Models.Attributes;
using ordercloud.integrations.library;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Orchestration\" represents Organization objects exposed for orchestration control")]
    [MarketplaceSection.Orchestration(ListOrder = 2)]
    [Route("orchestration/{clientId}")]
    public class OrchestrationUserController : BaseController
    {
        private readonly IOrchestrationCommand _command;

        public OrchestrationUserController(AppSettings settings, IOrchestrationCommand command) : base(settings)
        {
            _command = command;
        }

        [DocName("POST Buyer")]
        [HttpPost, Route("buyer"), OrderCloudIntegrationsAuth()]
        public async Task<MarketplaceBuyer> PostBuyer([FromBody] MarketplaceBuyer obj, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, obj.ID, clientId);
        }

        [DocName("POST User")]
        [HttpPost, Route("{buyerId}/user"), OrderCloudIntegrationsAuth()]
        public async Task<MarketplaceUser> PostUser([FromBody] MarketplaceUser obj, string buyerId, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, buyerId, clientId);
        }

        [DocName("POST UserGroup")]
        [HttpPost, Route("{buyerId}/usergroup"), OrderCloudIntegrationsAuth()]
        public async Task<MarketplaceUserGroup> PostUserGroup([FromBody] MarketplaceUserGroup obj, string buyerId, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, buyerId, clientId);
        }

        [DocName("POST UserGroupAssignment")]
        [HttpPost, Route("{buyerId}/usergroupassignment"), OrderCloudIntegrationsAuth()]
        public async Task<MarketplaceUserGroupAssignment> PostUserGroupAssignment([FromBody] MarketplaceUserGroupAssignment obj, string buyerId, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, buyerId, clientId);
        }

        [DocName("POST Address")]
        [HttpPost, Route("{buyerId}/address"), OrderCloudIntegrationsAuth()]
        public async Task<MarketplaceAddressBuyer> PostAddress([FromBody] MarketplaceAddressBuyer obj, string buyerId, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, buyerId, clientId);
        }

        [DocName("POST AddressAssignment")]
        [HttpPost, Route("{buyerId}/addressassignment"), OrderCloudIntegrationsAuth()]
        public async Task<MarketplaceAddressAssignment> PostAddressAssignment([FromBody] MarketplaceAddressAssignment obj, string buyerId, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, buyerId, clientId);
        }

        [DocName("POST CostCenter")]
        [HttpPost, Route("{buyerId}/costcenter"), OrderCloudIntegrationsAuth()]
        public async Task<MarketplaceCostCenter> PostCostCenter([FromBody] MarketplaceCostCenter obj, string buyerId, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, buyerId, clientId);
        }
    }
}
