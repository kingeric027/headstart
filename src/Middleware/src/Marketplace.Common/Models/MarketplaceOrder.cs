using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marketplace.Common.Models;
using Newtonsoft.Json;
using OrderCloud.SDK;


namespace Marketplace.Common.Models
{
	public class MarketplaceOrder : Order<OrderXp, dynamic, dynamic> { }

	public class OrderXp
	{
		// TODO - this should serialize to an array of ShippingSelection objects. 
		//  Dictionary key is ShipFromAddressID. This should enforce uniqueness. 
		public IDictionary<string, ShippingSelection> ShippingSelections { get; set; }
		public string AvalaraTaxTransactionCode { get; set; }
	}
}
