﻿using OrderCloud.SDK;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ordercloud.integrations.easypost
{
	public static class EasyPostMappers
	{
		public static int MINIMUM_SHIP_DIMENSION = 22; // inches 

		public static EasyPostAddress MapAddress(Address address)
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

		// To use this method all the LineItems should have the same ShipTo and ShipFrom
		// TODO - does this need to be more intelligient?
		public static EasyPostParcel MapParcel(IEnumerable<LineItem> lineItems)
		{
			var argregateHight = (double) Math.Max(MINIMUM_SHIP_DIMENSION, lineItems.Select(li => li.Product.ShipHeight ?? 0).Max());
			var argregateWidth = (double) Math.Max(MINIMUM_SHIP_DIMENSION, lineItems.Select(li => li.Product.ShipWidth ?? 0).Max());
			var argregateLength = (double) Math.Max(MINIMUM_SHIP_DIMENSION, lineItems.Select(li => li.Product.ShipLength ?? 0).Max());
			var totalWeight = lineItems.Aggregate(0.0, (sum, lineItem) => 
			{
				var productShipWeight = lineItem.Product.ShipWeight ?? 1;
				return sum += ((double)productShipWeight * lineItem.Quantity);
			});
			return new EasyPostParcel() { 
				weight = totalWeight,
				height = argregateHight,
				width = argregateWidth,
				length = argregateLength
			};
		}

		public static ShipMethod MapRate(EasyPostRate rate)
		{
			return new ShipMethod()
			{
				ID = rate.id,
				Name = rate.service,
				Cost = decimal.Parse(rate.rate),
				EstimatedTransitDays = (int)rate.delivery_days,
				xp =
				{
					Carrier = rate.carrier,
					CarrierAccountID = rate.carrier_account_id,
					ListRate = decimal.Parse(rate.list_rate),
					Guaranteed = rate.delivery_date_guaranteed,
					OriginalCost = decimal.Parse(rate.rate)
				}
			};
		}

		public static IList<ShipMethod> MapRates(IEnumerable<EasyPostRate> rates) => rates.Select(MapRate).ToList();

		public static EasyPostShipment MapShipment(IGrouping<AddressPair, LineItem> groupedLineItems, IEnumerable<string> accounts)
		{
			return new EasyPostShipment()
			{
				from_address = MapAddress(groupedLineItems.Key.ShipFrom),
				to_address = MapAddress(groupedLineItems.Key.ShipTo),
				parcel = MapParcel(groupedLineItems), // All line items with the same shipFrom and shipTo are grouped into 1 "parcel"
				carrier_accounts = accounts.Select(id => new EasyPostCarrierAccount() { id = id }).ToList()
			};
		}
	}
}
