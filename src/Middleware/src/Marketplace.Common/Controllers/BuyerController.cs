using Headstart.Common.Commands;
using Headstart.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Headstart.Models.Attributes;
using ordercloud.integrations.library;

namespace Headstart.Common.Controllers
{
    [DocComments("\"Buyers\" represents Buyers for Marketplace")]
    [MarketplaceSection.Marketplace(ListOrder = 1)]
    [Route("buyer")]
    public class BuyerController : BaseController
    {
        
        private readonly IHSBuyerCommand _command;
        private readonly IOrderCloudClient _oc;
        public BuyerController(IHSBuyerCommand command, IOrderCloudClient oc, AppSettings settings) : base(settings)
        {
            _command = command;
            _oc = oc;
        }

        [DocName("POST Marketplace Buyer")]
        [HttpPost, OrderCloudIntegrationsAuth(ApiRole.BuyerAdmin)]
        public async Task<SuperHSBuyer> Create([FromBody] SuperHSBuyer buyer)
        {
            return await _command.Create(buyer, VerifiedUserContext);
        }

        [DocName("PUT Marketplace Buyer")]
        [HttpPut, Route("{buyerID}"), OrderCloudIntegrationsAuth(ApiRole.BuyerAdmin)]
        public async Task<SuperHSBuyer> Put([FromBody] SuperHSBuyer superBuyer, string buyerID)
        {
            return await _command.Update(buyerID, superBuyer, VerifiedUserContext.AccessToken);
        }

        [DocName("GET Marketplace Buyer")]
        [HttpGet, Route("{buyerID}"), OrderCloudIntegrationsAuth(ApiRole.BuyerAdmin)]
        public async Task<SuperHSBuyer> Get(string buyerID)
        {
            return await _command.Get(buyerID, VerifiedUserContext.AccessToken);
        }
    }
}
