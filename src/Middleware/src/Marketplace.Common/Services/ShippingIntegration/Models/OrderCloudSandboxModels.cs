using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using Newtonsoft.Json.Linq;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration.Models
{
	// should be able to remove with sdk update
	public class MarketplaceOrderWorksheet : OrderWorksheet<MarketplaceOrder, MarketplaceLineItem, ShipEstimateResponse, OrderCalculateResponse, OrderSubmitResponse>
	{
	}

	// should be able to remove with sdk update
	public class OrderCalculatePayload<TOrderWorksheet> : OrderCalculatePayload
	{
		public new TOrderWorksheet OrderWorksheet { get; set; }
	}

	// should be able to remove with sdk update
	public class OrderCalculatePayload
	{
		public OrderWorksheet OrderWorksheet { get; set; }
		public CheckoutIntegrationConfiguration ConfigData { get; set; }
	}

	public class CheckoutIntegrationConfiguration
	{
		public bool ExcludePOProductsFromShipping { get; set; }
		public bool ExcludePOProductsFromTax { get; set; }
	}
}
