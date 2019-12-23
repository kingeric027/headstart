using Marketplace.Helpers.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Exceptions
{
	public static class ErrorCodes
	{
		public static IDictionary<string, ErrorCode> All { get; } = new Dictionary<string, ErrorCode>
		{
			{ "Checkout.MissingShippingSelection", new ErrorCode("MissingShippingSelection", 404, "Cannot proceed until all shipping selections have been made.") },
			{ "Checkout.InvalidShipFromAddress", new ErrorCode("InvalidShipFromAddress", 400, "This ShipFromAddressID does not exist.") },
		};

		public static class Checkout
		{
			/// <summary>Cannot proceed until all shipping selections have been made.</summary>
			public static readonly ErrorCode<MissingShippingSelectionError> MissingShippingSelection = All["Chekout.MissingShippingSelection"] as ErrorCode<MissingShippingSelectionError>;
			/// <summary>This ShipFromAddressID does not exist.</summary>
			public static readonly ErrorCode<InvalidShipFromAddressIDError> InvalidShipFromAddress = All["Chekout.MissingShippingSelection"] as ErrorCode<InvalidShipFromAddressIDError>;
		}
	}
}
