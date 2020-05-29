using Marketplace.Common.Commands;
using Marketplace.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Models.Attributes;
using ordercloud.integrations.library;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Buyers\" represents Buyers for Marketplace")]
    [MarketplaceSection.Marketplace(ListOrder = 1)]
    [Route("buyer")]
    public class BuyerController : BaseController
    {
        
        private readonly IMarketplaceBuyerCommand _command;
        private readonly IOrderCloudClient _oc;
        public BuyerController(IMarketplaceBuyerCommand command, IOrderCloudClient oc, AppSettings settings) : base(settings)
        {
            _command = command;
            _oc = oc;
        }

        [DocName("POST Marketplace Buyer")]
        [HttpPost, OrderCloudIntegrationsAuth(ApiRole.BuyerAdmin)]
        public async Task<MarketplaceBuyer> Create([FromBody] MarketplaceBuyer buyer)
        {
            // ocAuth is the token for the organization that is specified in the AppSettings
            var ocAuth = await _oc.AuthenticateAsync();
            return await _command.Create(buyer, VerifiedUserContext, ocAuth.AccessToken);
        } 
    }
}
