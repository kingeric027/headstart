using System.Collections.Generic;
using Marketplace.Helpers.Attributes;
using OrderCloud.SDK;

namespace Marketplace.Models.Misc
{
    [SwaggerModel]
	public class AddressValidation
	{
		public AddressValidation(Address raw)
		{
			RawAddress = raw;
		}

		public Address RawAddress { get; set; }
		public bool ValidAddressFound => ValidAddress != null;
		public Address ValidAddress { get; set; } = null;
		public List<Address> SuggestedAddresses { get; set; } = new List<Address>() { };
		// https://smartystreets.com/docs/cloud/us-street-api#footnotes
		public string GapBeteenRawAndValid { get; set; }
	}
}
