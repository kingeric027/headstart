using OrderCloud.SDK;
using System.Collections.Generic;

namespace Integrations.SmartyStreets
{
	public class AddressValidation
	{
		public AddressValidation(Address raw)
		{
			RawAddress = raw;
		}

		public Address RawAddress { get; set; }
		public Address ValidAddress { get; set; }
		public bool ValidAddressFound => ValidAddress != null;
		public List<Address> SuggestedAddresses { get; set; } = new List<Address>() { };
		// https://smartystreets.com/docs/cloud/us-street-api#footnotes
		public string GapBeteenRawAndValid { get; set; }
	}

	public class BuyerAddressValidation
	{
		public BuyerAddressValidation(BuyerAddress raw)
		{
			RawAddress = raw;
		}

		public BuyerAddress RawAddress { get; set; }
		public BuyerAddress ValidAddress { get; set; }
		public bool ValidAddressFound => ValidAddress != null;
		public List<BuyerAddress> SuggestedAddresses { get; set; } = new List<BuyerAddress>() { };
		// https://smartystreets.com/docs/cloud/us-street-api#footnotes
		public string GapBeteenRawAndValid { get; set; }
	}
}
