using System.Collections.Generic;
using Marketplace.Helpers.Models;
using Marketplace.Helpers.SwaggerTools;
using Marketplace.Models.Exceptions;
using ErrorCode = Marketplace.Helpers.Models.ErrorCode;

namespace Marketplace.Models
{
    public static class ErrorCodes
    {
        public static IDictionary<string, IErrorCode> All { get; } = new Dictionary<string, IErrorCode>
        {
            { "Checkout.MissingShippingSelection", new  ErrorCode<MissingShippingSelectionError>("MissingShippingSelection", 404, "Cannot proceed until all shipping selections have been made.") },
            { "Checkout.InvalidShipFromAddress", new ErrorCode<InvalidShipFromAddressIDError>("InvalidShipFromAddress", 400, "This ShipFromAddressID does not match any products in the order") },
            { "Checkout.MissingProductDimensions", new ErrorCode<MissingProductDimensionsError>("MissingProductDimensions", 400, "Product dimensions are missing for a product") },
            { "ZohoIntegrationError", new ErrorCode("ZohoIntegrationError", 400, "An error occurred in the Zoho Integration process")}
        };

        public static class Checkout
        {
            /// <summary>Cannot proceed until all shipping selections have been made.</summary>
            public static ErrorCode<MissingShippingSelectionError> MissingShippingSelection => All["Checkout.MissingShippingSelection"] as ErrorCode<MissingShippingSelectionError>;
            /// <summary>This ShipFromAddressID does not exist on the order.</summary>
            public static ErrorCode<InvalidShipFromAddressIDError> InvalidShipFromAddress => All["Checkout.InvalidShipFromAddress"] as ErrorCode<InvalidShipFromAddressIDError>;
            /// <summary>Product dimensions are not set for a product on this order.</summary>
            public static ErrorCode<MissingProductDimensionsError> MissingProductDimensions => All["Checkout.MissingProductDimensions"] as ErrorCode<MissingProductDimensionsError>;
        }

        public static class Integrations
        {
            public static ErrorCode<ZohoIntegrationError> ZohoIntegrationError => All["ZohoIntegrationError"] as ErrorCode<ZohoIntegrationError>;
        }
    }
}
