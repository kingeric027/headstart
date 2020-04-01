using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OrderCloud.SDK;

namespace Marketplace.Models.Misc
{
	public class SuperShipment
	{
		public Shipment Shipment { get; set; }
		public List<ShipmentItem> ShipmentItems { get; set; }
	}

	public class ShipmentCreateResponse
	{
		public Shipment Shipment { get; set; }
		public List<ShipmentItem> ShipmentItems { get; set; }

	}
}
