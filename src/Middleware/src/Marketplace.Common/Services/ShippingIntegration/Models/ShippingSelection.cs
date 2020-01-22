using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration
{
	// tentative model for a shipping selection, this will be a OrderCloud property with a full shipping integration
	public class ShippingSelection
	{
		[Required]
		public string SupplierID { get; set; }
		[Required]
		public string ShipFromAddressID { get; set; }
		[Required]
		public string ShippingRateID { get; set; }
	}
}
