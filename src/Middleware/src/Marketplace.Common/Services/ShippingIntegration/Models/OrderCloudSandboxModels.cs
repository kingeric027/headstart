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
	public class LineItemOverride
	{
		public string LineItemID { get; set; }
		public decimal UnitPrice { get; set; }
	}
	public class OrderWorksheet
	{
		public MarketplaceOrder Order { get; set; }
		public IList<MarketplaceLineItem> LineItems { get; set; }
		public ShipEstimateResponse ShipEstimateResponse { get; set; }
		public OrderCalculateResponse OrderCalculateResponse { get; set; }
	}
	public class OrderCalculateResponse
	{
		public List<LineItemOverride> LineItemOverrides { get; set; }
		public decimal? ShippingTotal { get; set; }
		public decimal? TaxTotal { get; set; }
		public JRaw xp { get; set; }
	}
	public class ShipEstimateResponse
	{
		public List<ShipEstimate> ShipEstimates { get; set; }
	}
	public class ShipEstimateItem
	{
		//unique to this proposedshipment
		public string LineItemID { get; set; }
		public int Quantity { get; set; }
	}
	public class ShipMethod
	{
		//unique to this proposedshipment
		public string ID { get; set; }
		public string Name { get; set; }
		public decimal Cost { get; set; }
		public int EstimatedTransitDays { get; set; }
		public JRaw xp { get; set; }
	}
	public class ShipEstimate
	{
		public string ID { get; set; }
		public JRaw xp { get; set; }
		public string SelectedShipMethodID { get; set; }
		public List<ShipEstimateItem> ShipEstimateItems { get; set; }
		public List<ShipMethod> ShipMethods { get; set; }
	}
	//model that's passed from the front end to set selections
	public class OrderShipMethodSelection
	{
		public ShipMethodSelection[] ShipMethodSelections { get; set; }
	}
	public class ShipMethodSelection
	{
		public string ShipEstimateID { get; set; }
		public string ShipMethodID { get; set; }
	}
	public class OrderCalculatePayload
	{
		public OrderWorksheet OrderWorksheet { get; set; }
		public JRaw ConfigData { get; set; }
	}
}
