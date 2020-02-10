using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Common.Services.FreightPop.Models;

namespace Marketplace.Common.Services.ShippingIntegration.Models
{
	// tentative model for a propsed shipments
	public class ProposedShipmentItem
	{
		public string LineItemID { get; set; }
		public int Quantity { get; set; }
	}

	public class ProposedShipmentOption
	{
		public string ID { get; set; }
		public string Name { get; set; }
		public int DeliveryDays { get; set; }
		public decimal Cost { get; set; }
	}
	public class ProposedShipment
	{
		public List<ProposedShipmentItem> ProposedShipmentItems { get; set; }
		public List<ProposedShipmentOption> ProposedShipmentOptions { get; set; }
	}
	public class ProposedShipmentRequest
	{
		public List<ProposedShipmentItem> ProposedShipmentItems { get; set; }
		public RateRequestBody RateRequestBody { get; set; }
		public Task<Response<GetRatesData>> RateResponseTask { get; set; }
	}
}
