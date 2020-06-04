using Marketplace.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Models.Attributes;
using Marketplace.Models.Misc;
using ordercloud.integrations.library;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Marketplace Shipments\" for making shipments in seller app")]
    [MarketplaceSection.Marketplace(ListOrder = 2)]
    [Route("shipment")]
    public class ShipmentController : BaseController
    {
        
        private readonly IShipmentCommand _command;
        public ShipmentController(IShipmentCommand command, AppSettings settings) : base(settings)
        {
            _command = command;
        }

        [DocName("POST Marketplace Shipment")]
        // todo update auth
        [HttpPost, OrderCloudIntegrationsAuth(ApiRole.ShipmentAdmin)]
        public async Task<ShipmentCreateResponse> Create([FromBody] SuperShipment superShipment)
        {
            // ocAuth is the token for the organization that is specified in the AppSettings

            // todo add auth to make sure suppliers are creating shipments for their own orders
            return await _command.CreateShipment(superShipment, VerifiedUserContext.AccessToken);
        } 
    }
}
