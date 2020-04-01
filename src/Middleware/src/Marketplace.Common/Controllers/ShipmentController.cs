using Marketplace.Common.Commands;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Helpers.Attributes;
using Marketplace.Models.Attributes;
using Marketplace.Models.Misc;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Marketplace Shipments\" for making shipments in seller app")]
    [MarketplaceSection.Marketplace(ListOrder = 2)]
    [Route("shipment")]
    public class ShipmentController : BaseController
    {
        
        private readonly IShipmentCommand _command;
        private readonly IOrderCloudClient _oc;
        public ShipmentController(IShipmentCommand command, IOrderCloudClient oc, AppSettings settings) : base(settings)
        {
            _command = command;
        }

        [DocName("POST Marketplace Shipment")]
        // todo update auth
        [HttpPost, MarketplaceUserAuth(ApiRole.SupplierAdmin)]
        public async Task<ShipmentCreateResponse> Create([FromBody] SuperShipment superShipment)
        {
            // ocAuth is the token for the organization that is specified in the AppSettings

            // todo add auth to make sure suppliers are creating shipments for their own orders
            return await _command.CreateShipment(superShipment, VerifiedUserContext.AccessToken);
        } 
    }
}
