using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using Marketplace.Helpers.Attributes;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class MarketplaceAddressBuyer : Address<BuyerAddressXP>, IMarketplaceObject
    {
    }
    [SwaggerModel]
    public class MarketplaceAddressMeBuyer : BuyerAddress<BuyerAddressXP>, IMarketplaceObject
    {
    }

	[SwaggerModel]
	public class BuyerAddressXP
	{
		public List<DestinationAddressAccessorial> Accessorials { get; set; }
		public string Email { get; set; }
		public Coordinates Coordinates;
		public int AvalaraCertificateID { get; set; } // null if no certificate
		public DateTimeOffset AvalaraCertificateExpiration { get; set; } // null if no certificate 
	}

    public enum DestinationAddressAccessorial
    {
        DestinationInsideDelivery = 3,
        DestinationLiftGate = 4,
        LimitedAccessDelivery = 9,
        ResidentialDelivery = 15,
    }

    [SwaggerModel]
	public class Coordinates
	{
		public double Latitude { get; set; }
		public double Longitude { get; set; }
	}
}
