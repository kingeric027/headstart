using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Marketplace.Helpers.Attributes;
using OrderCloud.SDK;

namespace Marketplace.Models.Misc
{

	[SwaggerModel]
	public class SuperShipment
	{
		public Shipment Shipment { get; set; }
		public List<ShipmentItem> ShipmentItems { get; set; }
	}

	[SwaggerModel]
	public class ShipmentCreateResponse
	{
		public Shipment Shipment { get; set; }
		public List<ShipmentItem> ShipmentItems { get; set; }

	}
}
