﻿using Marketplace.Helpers.Exceptions;
using Marketplace.Models.Misc;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Services.SmartyStreets.Models
{
	public class InvalidAddressException : ApiErrorException
	{
		public InvalidAddressException(AddressValidation<Address> validation) : base("InvalidAddress", 400, "Address not valid", validation) { }
	}

	public class InvalidBuyerAddressException : ApiErrorException
	{
		public InvalidBuyerAddressException(AddressValidation<BuyerAddress> validation) : base("InvalidAddress", 400, "Address not valid", validation) { }
	}
}
