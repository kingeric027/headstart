using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration.Models
{
	public class LineItemOverride
	{
		public string LineItemID { get; set; }
		public decimal UnitPrice { get; set; }
	}
	public class OrderWorksheet
	{
		public Order Order { get; set; }
		public List<LineItem> LineItems { get; set; }
		public ShipmentEstimateResponse ShipmentEstimateResponse { get; set; }
		public OrderCalculateResponse OrderCalculateResponse { get; set; }
	}
	public class OrderCalculateResponse
	{
		public List<LineItemOverride> LineItemOverrides { get; set; }
		public decimal? ShippingTotal { get; set; }
		public decimal? TaxTotal { get; set; }
		public JObject xp { get; set; }
	}
	public class ShipmentEstimateResponse
	{
		public List<ShipmentEstimate> ShipmentEstimates { get; set; }
	}
	public class ShipmentEstimateItem
	{
		//unique to this proposedshipment
		public string LineItemID { get; set; }
		public int Quantity { get; set; }
	}
	public class ShipmentMethod
	{
		//unique to this proposedshipment
		public string ID { get; set; }
		public string Name { get; set; }
		public decimal Cost { get; set; }
		public int EstimatedTransitDays { get; set; }
		public JObject xp { get; set; }
	}
	public class ShipmentEstimate
	{
		public string ID { get; set; }
		public JObject xp { get; set; }
		public string SelectedShipMethodID { get; set; }
		public List<ShipmentEstimateItem> ShipmentEstimateItems { get; set; }
		public List<ShipmentMethod> ShipmentMethods { get; set; }
	}
	//model that's passed from the front end to set selections
	public class OrderShipmethodSelection
	{
		public ShipmethodSelection[] ShipmethodSelections { get; set; }
	}
	public class ShipmethodSelection
	{
		public string ShipmentEstimateID { get; set; }
		public string ShipmentMethodID { get; set; }
	}
	public class OrderCalculatePayload
	{
		public OrderWorksheet OrderCalculation { get; set; }
		public JRaw ConfigData { get; set; }
	}
}
