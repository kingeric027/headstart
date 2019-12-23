using System;
using System.Collections.Generic;
using System.Text;
using OrderCloud.SDK;


namespace Marketplace.Common.Models
{
	public class MarketplaceOrder: Order<OrderXp, dynamic, dynamic> {}

	public class OrderXp
	{
		//  Dictionary key is ShipFromAddressID. This should enforce uniqueness. How to serialize to JSON?
		public IDictionary<string, ShippingSelection> ShippingSelections { get; set; }
		public string AvalaraTaxTransactionCode { get; set; }
	}
}
