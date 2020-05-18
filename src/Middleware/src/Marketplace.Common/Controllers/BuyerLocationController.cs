using Marketplace.Common.Commands;
using Marketplace.Models;
using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using ordercloud.integrations.extensions;
using ordercloud.integrations.openapispec;

namespace Marketplace.Common.Controllers
{
	[DocComments("\"Files\" represents files for Marketplace content management control")]
	[MarketplaceSection.Marketplace(ListOrder = 6)]
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

		[DocName("GET a Buyer Location")]
		[HttpGet, Route("{buyerID}/{buyerLocationID}"), OrderCloudIntegrationsAuth(ApiRole.UserGroupAdmin, ApiRole.AddressAdmin)]
        public async Task<MarketplaceBuyerLocation> Get(string buyerID, string buyerLocationID)
        {
            return await _command.Get(buyerID, buyerLocationID, VerifiedUserContext);
        }

		[DocName("POST a Buyer Location")]
		[HttpPost, Route("{buyerID}"), OrderCloudIntegrationsAuth(ApiRole.UserGroupAdmin, ApiRole.AddressAdmin)]
        public async Task<MarketplaceBuyerLocation> Create(string buyerID, [FromBody] MarketplaceBuyerLocation buyerLocation)
        {
            // ocAuth is the token for the organization that is specified in the AppSettings
            var ocAuth = await _oc.AuthenticateAsync();
            return await _command.Create(buyerID, buyerLocation, VerifiedUserContext, ocAuth.AccessToken);
        }

		[DocName("PUT a Buyer Location")]
		[HttpPut, Route("{buyerID}/{buyerLocationID}"), OrderCloudIntegrationsAuth(ApiRole.UserGroupAdmin, ApiRole.AddressAdmin)]
        public async Task<MarketplaceBuyerLocation> Update(string buyerID, string buyerLocationID, [FromBody] MarketplaceBuyerLocation buyerLocation)
        {
            return await _command.Update(buyerID, buyerLocationID, buyerLocation, VerifiedUserContext);
        }

		[DocName("Delete a Buyer Location")]
		[HttpDelete, Route("{buyerID}/{buyerLocationID}"), OrderCloudIntegrationsAuth(ApiRole.UserGroupAdmin, ApiRole.AddressAdmin)]
        public async Task Delete(string buyerID, string buyerLocationID)
        {
            await _command.Delete(buyerID, buyerLocationID, VerifiedUserContext);
        }
	}
}
