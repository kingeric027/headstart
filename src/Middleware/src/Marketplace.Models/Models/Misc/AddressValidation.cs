using System.Collections.Generic;
using OrderCloud.SDK;

namespace Marketplace.Models.Misc
{
	public class AddressValidation
	{
		public Address RawAddress { get; set; }
		public bool IsRawAddressValid { get; set; }
		public bool AreSuggestionsValid { get; set; } // suggestions may be incomplete if they come from the autocomplete api
		public List<Address> SuggestedAddresses { get; set; }
	}
}
