using System.Collections.Generic;
using Marketplace.Common.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.AvaTax.Models
{
	public class TaxableOrder
	{
		public MarketplaceOrder Order { get; set; }
		public IEnumerable<LineItem> Lines { get; set; }
		// Keys are ShipFromAddressIDs
		public IDictionary<string, decimal> ShippingRates { get; set; }
	}
}
