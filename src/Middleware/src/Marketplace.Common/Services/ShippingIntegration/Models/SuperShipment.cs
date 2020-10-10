using System;
using System.Collections.Generic;
using Marketplace.Common.Services.ShippingIntegration.Models;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Models.Misc
{
	[SwaggerModel]
	public class SuperShipment
	{
		public MarketplaceShipment Shipment { get; set; }
		public List<ShipmentItem> ShipmentItems { get; set; }
	}
}
