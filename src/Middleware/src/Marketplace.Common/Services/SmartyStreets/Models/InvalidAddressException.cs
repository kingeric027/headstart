using Marketplace.Helpers.Exceptions;
using Marketplace.Models.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Services.SmartyStreets.Models
{
	public class InvalidAddressException : ApiErrorException
	{
		public InvalidAddressException(AddressValidation validation) : base("InvalidAddress", 400, "Address not valid", validation) { }
	}
}
