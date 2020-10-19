using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using Newtonsoft.Json.Linq;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration.Models
{
	public class MarketplaceOrderWorksheet : OrderWorksheet<MarketplaceOrder, MarketplaceLineItem, ShipEstimateResponse, OrderCalculateResponse, OrderSubmitResponse, OrderSubmitForApprovalResponse, OrderApprovedResponse>
	{
	}

	public class MarketplaceOrderCalculatePayload
	{
		public MarketplaceOrderWorksheet OrderWorksheet { get; set; }
		public CheckoutIntegrationConfiguration ConfigData { get; set; }

	}

	public class CheckoutIntegrationConfiguration
	{
		public bool ExcludePOProductsFromShipping { get; set; }
		public bool ExcludePOProductsFromTax { get; set; }
	}

    public static class MarketplaceOrderWorksheetExtensions
    {
        public static bool IsStandardOrder(this MarketplaceOrderWorksheet sheet)
        {
			return sheet.Order.xp == null || sheet.Order.xp.OrderType != OrderType.Quote;
		}
    }
}
