using System.Collections.Generic;
using System.Threading.Tasks;
using Dynamitey.DynamicObjects;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using Newtonsoft.Json.Linq;
using ordercloud.integrations.exchangerates;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration.Models
{
	public class MarketplaceOrderWorksheet : OrderWorksheet<MarketplaceOrder, MarketplaceLineItem, MarketplaceShipEstimateResponse, OrderCalculateResponse, OrderSubmitResponse, OrderSubmitForApprovalResponse, OrderApprovedResponse>
	{
	}

	public class MarketplaceOrderCalculatePayload
	{
		public MarketplaceOrderWorksheet OrderWorksheet { get; set; }
		public CheckoutIntegrationConfiguration ConfigData { get; set; }

	}

	public class ShipEstimateResponseXP { }

	[SwaggerModel]
	public class ShipEstimateXP
	{
		public List<MarketplaceShipMethod> AllShipMethods { get; set; }
		public string SupplierID { get; set; }
		public string ShipFromAddressID { get; set; }
	}

	[SwaggerModel]
	public class ShipMethodXP
	{
		public string Carrier { get; set; } // e.g. "Fedex"
		public string CarrierAccountID { get; set; }
		public decimal ListRate { get; set; }
		public bool Guaranteed { get; set; }
		public decimal OriginalCost { get; set; }
		public bool FreeShippingApplied { get; set; }
		public int? FreeShippingThreshold { get; set; }
		public CurrencySymbol? OriginalCurrency { get; set; }
		public CurrencySymbol? OrderCurrency { get; set; }
		public double? ExchangeRate { get; set; }
	}

	[SwaggerModel]
	public class MarketplaceShipMethod : ShipMethod<ShipMethodXP> { }

	[SwaggerModel]
	public class MarketplaceShipEstimate : ShipEstimate<ShipEstimateXP, MarketplaceShipMethod> { }

	public class MarketplaceShipEstimateResponse : ShipEstimateResponse<ShipEstimateResponseXP, MarketplaceShipEstimate> { }

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
