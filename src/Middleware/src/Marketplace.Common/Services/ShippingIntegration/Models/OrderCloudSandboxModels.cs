using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Common.Services.FreightPop.Models;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using Newtonsoft.Json.Linq;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration.Models
{
	public class ShipmentEstimateRequest
	{
		public string ID { get; set; }
		public List<ShipEstimateItem> ShipEstimateItems { get; set; }
		public RateRequestBody RateRequestBody { get; set; }
		public Task<Response<GetRatesData>> RateResponseTask { get; set; }
	}


	// should be able to remove with sdk update
	public class OrderWorksheet<TOrder, TLineItem> : OrderWorksheet
	{
		public new TOrder Order { get; set; }
		public new List<TLineItem> LineItems { get; set; }
	}

	// should be able to remove with sdk update
	public class MarketplaceOrderWorksheet : OrderWorksheet<MarketplaceOrder, MarketplaceLineItem>
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
		public JRaw ConfigData { get; set; }
	}
}
