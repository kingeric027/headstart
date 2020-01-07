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
			{ "Checkout.MissingShippingSelection", new  ErrorCode<MissingShippingSelectionError>("MissingShippingSelection", 404, "Cannot proceed until all shipping selections have been made.") },
			{ "Checkout.InvalidShipFromAddress", new ErrorCode<InvalidShipFromAddressIDError>("InvalidShipFromAddress", 400, "This ShipFromAddressID does not match any products in the order") },
		};

		public static class Checkout
		{
			/// <summary>Cannot proceed until all shipping selections have been made.</summary>
			public static ErrorCode<MissingShippingSelectionError> MissingShippingSelection => All["Checkout.MissingShippingSelection"] as ErrorCode<MissingShippingSelectionError>;
			/// <summary>This ShipFromAddressID does not exist on the order.</summary>
			public static ErrorCode<InvalidShipFromAddressIDError> InvalidShipFromAddress => All["Checkout.InvalidShipFromAddress"] as ErrorCode<InvalidShipFromAddressIDError>;
		}
	}
}
