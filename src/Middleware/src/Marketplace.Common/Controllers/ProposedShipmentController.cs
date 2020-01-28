using Marketplace.Common.Commands;
using Marketplace.Common.Models;
using Marketplace.Common.Services.ShippingIntegration;
using Marketplace.Helpers;
using Marketplace.Helpers.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;

namespace Marketplace.Common.Controllers
{
	[Route("proposedshipment")]
	public class ProposedShipmentController: BaseController
	{
		private readonly IProposedShipmentCommand _shippingCommand;
		public ProposedShipmentController(AppSettings settings, IProposedShipmentCommand shippingCommand) : base(settings) 
		{
			_shippingCommand = shippingCommand;
		}

		// investigate why ApiRole.Shopper auth is failing for shoppers
		[HttpGet, Route("{orderId}"), MarketplaceUserAuth(ApiRole.SupplierReader)]
		public async Task<MarketplaceListPage<ProposedShipment>> List(string orderId)
		{
			var shippingRateResponse = await _shippingCommand.ListProposedShipments(orderId, VerifiedUserContext);
			return shippingRateResponse;
		}

		[HttpPut, Route("{orderId}/select"), MarketplaceUserAuth(ApiRole.SupplierReader)]
		public async Task<MarketplaceOrder> Select(string orderId, [FromBody] ProposedShipmentSelection proposedShipmentSelection)
		{
			var marketplaceOrder = await _shippingCommand.SetShippingSelectionAsync(orderId, proposedShipmentSelection);
			return marketplaceOrder;
		}
	}
}
