using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Marketplace.Common.Services.FreightPop.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration.Models
{

	public class ProposedShipmentRequest
	{
		public string ID { get; set; }
		public List<ProposedShipmentItem> ProposedShipmentItems { get; set; }
		public RateRequestBody RateRequestBody { get; set; }
		public Task<Response<GetRatesData>> RateResponseTask { get; set; }
	}

	public class ProposedShipmentResponse
	{
		public List<ProposedShipment> ProposedShipments { get; set; }
	}

	// tentative model for a propsed shipments
	public class ProposedShipment : OrderCloudModel
	{
		/// <summary>ID of the proposed shipment.</summary> 
		public string ID { get => GetProp<string>("ID"); set => SetProp<string>("ID", value); }
		/// <summary>Container for extended (custom) properties of the proposed shipment.</summary> 
		public dynamic xp { get => GetProp<dynamic>("xp", new ExpandoObject()); set => SetProp<dynamic>("xp", value); }
		/// <summary>ID of the selected proposed shipment option.</summary> 
		public string SelectedProposedShipmentOptionID { get => GetProp<string>("SelectedProposedShipmentOptionID"); set => SetProp<string>("SelectedProposedShipmentOptionID", value); }
		/// <summary>Proposed shipment items of the proposed shipment.</summary> 
		public IList<ProposedShipmentItem> ProposedShipmentItems { get => GetProp<IList<ProposedShipmentItem>>("ProposedShipmentItems", new List<ProposedShipmentItem>()); set => SetProp<IList<ProposedShipmentItem>>("ProposedShipmentItems", value); }
		/// <summary>Proposed shipment options of the proposed shipment.</summary> 
		public IList<ProposedShipmentOption> ProposedShipmentOptions { get => GetProp<IList<ProposedShipmentOption>>("ProposedShipmentOptions", new List<ProposedShipmentOption>()); set => SetProp<IList<ProposedShipmentOption>>("ProposedShipmentOptions", value); }
	}

	/// <typeparam name="Txp">Type used as a container for extended properties (xp) of the ProposedShipment.</typeparam> 
	public class ProposedShipment<Txp> : ProposedShipment
	{
		public new Txp xp { get; set; }
	}

	public class ProposedShipmentItem : OrderCloudModel
	{
		/// <summary>ID of the line item.</summary> 
		public string LineItemID { get => GetProp<string>("LineItemID"); set => SetProp<string>("LineItemID", value); }
		/// <summary>Quantity of the proposed shipment item.</summary> 
		public int Quantity { get => GetProp<int>("Quantity"); set => SetProp<int>("Quantity", value); }
	}

	public class ProposedShipmentOption : OrderCloudModel
	{
		/// <summary>ID of the proposed shipment option.</summary> 
		public string ID { get => GetProp<string>("ID"); set => SetProp<string>("ID", value); }
		/// <summary>Name of the proposed shipment option.</summary> 
		public string Name { get => GetProp<string>("Name"); set => SetProp<string>("Name", value); }
		/// <summary>Cost of the proposed shipment option.</summary> 
		public decimal Cost { get => GetProp<decimal>("Cost"); set => SetProp<decimal>("Cost", value); }
		/// <summary>Estimated delivery days of the proposed shipment option.</summary> 
		public int EstimatedDeliveryDays { get => GetProp<int>("EstimatedDeliveryDays"); set => SetProp<int>("EstimatedDeliveryDays", value); }
		/// <summary>Container for extended (custom) properties of the proposed shipment option.</summary> 
		public dynamic xp { get => GetProp<dynamic>("xp", new ExpandoObject()); set => SetProp<dynamic>("xp", value); }
	}

	/// <typeparam name="Txp">Type used as a container for extended properties (xp) of the ProposedShipmentOption.</typeparam> 
	public class ProposedShipmentOption<Txp> : ProposedShipmentOption
	{
		public new Txp xp { get; set; }
	}

	public class ShipmentPreference : OrderCloudModel
	{
		/// <summary>ID of the proposed shipment.</summary> 
		public string ProposedShipmentID { get => GetProp<string>("ProposedShipmentID"); set => SetProp<string>("ProposedShipmentID", value); }
		/// <summary>ID of the proposed shipment option.</summary> 
		public string ProposedShipmentOptionID { get => GetProp<string>("ProposedShipmentOptionID"); set => SetProp<string>("ProposedShipmentOptionID", value); }
	}

	public class OrderShipmentPreferences : OrderCloudModel
	{
		/// <summary>Shipment preferences of the order shipment preference.</summary> 
		public IList<ShipmentPreference> ShipmentPreferences { get => GetProp<IList<ShipmentPreference>>("ShipmentPreferences", new List<ShipmentPreference>()); set => SetProp<IList<ShipmentPreference>>("ShipmentPreferences", value); }
	}

	public class LineItemOverride : OrderCloudModel
	{
		/// <summary>ID of the line item.</summary> 
		public string LineItemID { get => GetProp<string>("LineItemID"); set => SetProp<string>("LineItemID", value); }
		/// <summary>Unit price of the line item override.</summary> 
		public decimal UnitPrice { get => GetProp<decimal>("UnitPrice"); set => SetProp<decimal>("UnitPrice", value); }
	}

	public class OrderCalculateResponse : OrderCloudModel
	{
		/// <summary>Line item overrides of the order calculate response.</summary> 
		public IList<LineItemOverride> LineItemOverrides { get => GetProp<IList<LineItemOverride>>("LineItemOverrides", new List<LineItemOverride>()); set => SetProp<IList<LineItemOverride>>("LineItemOverrides", value); }
		/// <summary>Shipping total of the order calculate response.</summary> 
		public decimal ShippingTotal { get => GetProp<decimal>("ShippingTotal"); set => SetProp<decimal>("ShippingTotal", value); }
		/// <summary>Tax total of the order calculate response.</summary> 
		public decimal TaxTotal { get => GetProp<decimal>("TaxTotal"); set => SetProp<decimal>("TaxTotal", value); }
		/// <summary>Container for extended (custom) properties of the order calculate response.</summary> 
		public dynamic xp { get => GetProp<dynamic>("xp", new ExpandoObject()); set => SetProp<dynamic>("xp", value); }
	}

	/// <typeparam name="Txp">Type used as a container for extended properties (xp) of the OrderCalculateResponse.</typeparam> 
	public class OrderCalculateResponse<Txp> : OrderCalculateResponse
	{
		public new Txp xp { get; set; }
	}

	public class OrderCalculationRequest
	{
		public OrderCalculation OrderCalculation { get; set; }
		public dynamic ConfigData { get; set; }
	}

	public class OrderCalculation : OrderCloudModel
	{
		/// <summary>Order of the order calculation.</summary> 
		public Order Order { get => GetProp<Order>("Order"); set => SetProp<Order>("Order", value); }
		/// <summary>Line items of the order calculation.</summary> 
		public IList<LineItem> LineItems { get => GetProp<IList<LineItem>>("LineItems", new List<LineItem>()); set => SetProp<IList<LineItem>>("LineItems", value); }
		/// <summary>Proposed shipments of the order calculation.</summary> 
		public IList<ProposedShipment> ProposedShipmentRatesResponse { get => GetProp<IList<ProposedShipment>>("ProposedShipments", new List<ProposedShipment>()); set => SetProp<IList<ProposedShipment>>("ProposedShipments", value); }
		/// <summary>Order calculate response of the order calculation.</summary> 
		public OrderCalculateResponse OrderCalculateResponse { get => GetProp<OrderCalculateResponse>("OrderCalculateResponse"); set => SetProp<OrderCalculateResponse>("OrderCalculateResponse", value); }
	}
}
