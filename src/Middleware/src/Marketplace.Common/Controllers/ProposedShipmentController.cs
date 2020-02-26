using Marketplace.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Helpers.SwaggerTools;
using Marketplace.Models;
using Marketplace.Models.Attributes;
using Marketplace.Models.Extended;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Proposed Shipments\" represents proposed shipments for line items")]
    [MarketplaceSection.OrdersAndFulfillment(ListOrder = 1)]
    [Route("proposedshipment")]
	public class ProposedShipmentController: BaseController
	{
		private readonly IProposedShipmentCommand _shippingCommand;
		public ProposedShipmentController(AppSettings settings, IProposedShipmentCommand shippingCommand) : base(settings) 
		{
			_shippingCommand = shippingCommand;
		}

        // investigate why ApiRole.Shopper auth is failing for shoppers
        [DocName("LIST ProposedShipments")]
        [HttpGet, Route("{orderId}"), MarketplaceUserAuth(ApiRole.SupplierReader)]
		public async Task<ListPage<ProposedShipment>> List(string orderId)
		{
			var shippingRateResponse = await _shippingCommand.ListProposedShipments(orderId, VerifiedUserContext);
			return shippingRateResponse;
		}

        [DocName("PUT ProposedShipment")]
        [HttpPut, Route("{orderId}/select"), MarketplaceUserAuth(ApiRole.SupplierReader)]
		public async Task<MarketplaceOrder> Select(string orderId, [FromBody] ProposedShipmentSelection proposedShipmentSelection)
		{
			var marketplaceOrder = await _shippingCommand.SetShippingSelectionAsync(orderId, proposedShipmentSelection);
			return marketplaceOrder;
		}
	}
}
