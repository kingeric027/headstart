using Marketplace.Models.Misc;
using ordercloud.integrations.extensions;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.SmartyStreets.Models
{
	public class InvalidAddressException : OrderCloudIntegrationException
	{
		public InvalidAddressException(AddressValidation<Address> validation) : base("InvalidAddress", 400, "Address not valid", validation) { }
	}

	public class InvalidBuyerAddressException : OrderCloudIntegrationException
	{
		public InvalidBuyerAddressException(AddressValidation<BuyerAddress> validation) : base("InvalidAddress", 400, "Address not valid", validation) { }
	}
}
