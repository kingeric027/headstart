using System;
using System.Collections.Generic;
using System.Text;

namespace ordercloud.integrations.easypost
{
	public static class EasyPostMappers
	{
		public static EasyPostAddress MapAddress(OrderCloud.SDK.Address address)
		{
			return new EasyPostAddress()
			{
				street1 = address.Street1,
				street2 = address.Street2,
				city = address.City,
				state = address.State,
				zip = address.Zip,
				country = address.Country,
			};
		}

		public static EasyPostParcel MapParcel(OrderCloud.SDK.LineItem li)
		{
			return new EasyPostParcel()
			{
				weight = (double)(li.Product.ShipWeight ?? 1), // ship weight is required and cannot be 0.
				height = (double)(li.Product.ShipHeight), // optional, so null is fine
				width = (double)(li.Product.ShipWidth),
				length = (double)(li.Product.ShipLength)
			};
		}

		// deprecated
		public static EasyPostParcel MapParcel(double weight) 
		{
			return new EasyPostParcel()
			{
				weight = weight
			};
		}
	}
}
