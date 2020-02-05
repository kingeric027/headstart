using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marketplace.Common.Models;
using Marketplace.Common.Services.ShippingIntegration;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Newtonsoft.Json;
using OrderCloud.SDK;


namespace Marketplace.Common.Models
{
	public class MarketplaceOrder : Order<OrderXp, dynamic, dynamic> { }

	public class OrderXp
	{
		public IEnumerable<ProposedShipmentSelection> ProposedShipmentSelections { get; set; }
		public string AvalaraTaxTransactionCode { get; set; }
	}
}
