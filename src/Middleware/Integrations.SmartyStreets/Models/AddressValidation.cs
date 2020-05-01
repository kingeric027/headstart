using System.Collections.Generic;

namespace Marketplace.Models.Misc
{
	public class AddressValidation<TAddress>
	{
		public AddressValidation(TAddress raw)
		{
			RawAddress = raw;
		}

		public TAddress RawAddress { get; set; }
		public TAddress ValidAddress { get; set; }
		public bool ValidAddressFound => ValidAddress != null;
		public List<TAddress> SuggestedAddresses { get; set; } = new List<TAddress>() { };
		// https://smartystreets.com/docs/cloud/us-street-api#footnotes
		public string GapBeteenRawAndValid { get; set; }
	}
}
