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
		public List<Address> SuggestedAddresses { get; set; }
	}
}
