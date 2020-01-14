using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Exceptions
{
	public class InvalidShipFromAddressIDError
	{
		public InvalidShipFromAddressIDError(string id)
		{
			ShipFromAddressID = id;
		}

		public string ShipFromAddressID { get; set; } 
	}
}
