using Marketplace.Models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Services.SmartyStreets.models
{
	public class AddressValidation
	{
		public Address RawAddress { get; set; }
		public bool IsRawAddressValid { get; set; }
		public bool AreSuggestionsValid { get; set; } // suggestions may be incomplete if they come from the autocomplete api
		public List<Address> SuggestedAddresses { get; set; }
	}
}
