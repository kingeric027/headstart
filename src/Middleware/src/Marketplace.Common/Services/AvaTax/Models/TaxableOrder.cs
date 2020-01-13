using Marketplace.Common.Services.FreightPop;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Models
{
	public class TaxableOrder
	{
		public MarketplaceOrder Order { get; set; }
		public IEnumerable<LineItem> Lines { get; set; }
		public IEnumerable<ShippingRate> ShippingRates { get; set; }
	}
}
