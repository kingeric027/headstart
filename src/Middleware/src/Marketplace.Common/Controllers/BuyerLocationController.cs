using Marketplace.Common.Commands;
using Marketplace.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Controllers
{
    [Route("buyerlocations")]
    public class BuyerLocationController : BaseController
    {

        private readonly IMarketplaceBuyerLocationCommand _command;
        private readonly IOrderCloudClient _oc;
        public BuyerLocationController(IMarketplaceBuyerLocationCommand command, IOrderCloudClient oc, AppSettings settings) : base(settings)
        {
            _command = command;
            _oc = oc;
        }

        [HttpGet, Route("{buyerID}/{buyerLocationID}"), MarketplaceUserAuth(ApiRole.UserGroupAdmin, ApiRole.AddressAdmin)]
        public async Task<MarketplaceBuyerLocation> Get(string buyerID, string buyerLocationID)
        {
            return await _command.Get(buyerID, buyerLocationID, VerifiedUserContext);
        }

        [HttpPost, Route("{buyerID}"), MarketplaceUserAuth(ApiRole.UserGroupAdmin, ApiRole.AddressAdmin)]
        public async Task<MarketplaceBuyerLocation> Create([FromBody] MarketplaceBuyerLocation buyerLocation, string buyerID)
        {
            // ocAuth is the token for the organization that is specified in the AppSettings
            var ocAuth = await _oc.AuthenticateAsync();
            return await _command.Create(buyerID, buyerLocation, VerifiedUserContext, ocAuth.AccessToken);
        }
        [HttpPut, Route("{buyerID}/{buyerLocationID}"), MarketplaceUserAuth(ApiRole.UserGroupAdmin, ApiRole.AddressAdmin)]
        public async Task<MarketplaceBuyerLocation> Update([FromBody] MarketplaceBuyerLocation buyerLocation, string buyerLocationID, string buyerID)
        {
            return await _command.Update(buyerID, buyerLocationID, buyerLocation, VerifiedUserContext);
        }
        [HttpDelete, Route("{buyerID}/{buyerLocationID}"), MarketplaceUserAuth(ApiRole.UserGroupAdmin, ApiRole.AddressAdmin)]
        public async Task Delete(string buyerLocationID, string buyerID)
        {
            await _command.Delete(buyerID, buyerLocationID, VerifiedUserContext);
        }
    }
}
