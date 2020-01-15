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
		public IEnumerable<ShippingSelection> ShippingSelections { get; set; }
		public string AvalaraTaxTransactionCode { get; set; }
	}
}
