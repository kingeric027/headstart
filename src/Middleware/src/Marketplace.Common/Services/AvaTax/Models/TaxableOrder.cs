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
		public IDictionary<string, ShippingRate> SelectedRates { get; set; }
	}
}
