using System.Collections.Generic;
using Marketplace.Models.Extended;
using OrderCloud.SDK;

namespace Marketplace.Models
{
	public class MarketplaceOrder : Order<OrderXp, UserXp, AddressXp>
	{
		public IEnumerable<ProposedShipmentSelection> ShippingSelections { get; set; }
		public string AvalaraTaxTransactionCode { get; set; }
	}

    public class OrderXp
    {
        public IEnumerable<ProposedShipmentSelection> ProposedShipmentSelections { get; set; }
        public string AvalaraTaxTransactionCode { get; set; }
    }
}