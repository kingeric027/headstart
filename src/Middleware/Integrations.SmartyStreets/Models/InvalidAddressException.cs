using ordercloud.integrations.extensions;
using OrderCloud.SDK;

namespace Integrations.SmartyStreets
{
	public class InvalidAddressException : OrderCloudIntegrationException
	{
		public InvalidAddressException(AddressValidation validation) : base("InvalidAddress", "Address not valid", validation) { }
	}

	public class InvalidBuyerAddressException : OrderCloudIntegrationException
	{
		public InvalidBuyerAddressException(BuyerAddressValidation validation) : base("InvalidAddress", "Address not valid", validation) { }
	}
}
