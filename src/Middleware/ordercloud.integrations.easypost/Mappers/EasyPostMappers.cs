using OrderCloud.SDK;
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
		public static EasyPostParcel MapParcel(IList<LineItem> lineItems)
		{
			var aggregateHeight = (double)Math.Min(MINIMUM_SHIP_DIMENSION, lineItems.Max(li => li.Product.ShipHeight ?? MINIMUM_SHIP_DIMENSION));
			var aggregateWidth = (double)Math.Min(MINIMUM_SHIP_DIMENSION, lineItems.Max(li => li.Product.ShipWidth ?? MINIMUM_SHIP_DIMENSION));
			var aggregateLength = (double)Math.Min(MINIMUM_SHIP_DIMENSION, lineItems.Max(li => li.Product.ShipLength ?? MINIMUM_SHIP_DIMENSION));
			var totalWeight = lineItems.Aggregate(0.0, (sum, lineItem) => 
			{
				var productShipWeight = lineItem.Product.ShipWeight ?? 1;
				return sum += ((double)productShipWeight * lineItem.Quantity);
			});
			return new EasyPostParcel() { 
				weight = totalWeight,
				height = aggregateHeight,
				width = aggregateWidth,
				length = aggregateLength
			};
		}

        private static List<EasyPostCustomsItem> MapCustomsItem(IGrouping<AddressPair, LineItem> lineitems, EasyPostShippingProfile profile)
        {
            return lineitems.Select(lineItem => new EasyPostCustomsItem()
                {
                    description = lineItem.Product.Name,
                    hs_tariff_number = profile.HS_Tariff_Number,
                    origin_country = lineItem.ShipFromAddress.Country,
                    value = decimal.ToDouble(lineItem.LineSubtotal),
                    quantity = lineItem.Quantity,
                    weight = (double)Convert.ChangeType(lineItem.Product.ShipWeight, typeof(double))
                })
                .ToList();
        }

		public static ShipMethod MapRate(EasyPostRate rate)
		{
			return new ShipMethod()
			{
				ID = rate.id,
				Name = rate.service,
				Cost = decimal.Parse(rate.rate),
				EstimatedTransitDays = rate.delivery_days ?? rate.est_delivery_days ?? 10,
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

		public static EasyPostShipment MapShipment(IGrouping<AddressPair, LineItem> groupedLineItems, EasyPostShippingProfiles profiles)
		{
			var shipment =  new EasyPostShipment()
			{
				from_address = MapAddress(groupedLineItems.Key.ShipFrom),
				to_address = MapAddress(groupedLineItems.Key.ShipTo),
				parcel = MapParcel(groupedLineItems.Select(g => g).ToList()), // All line items with the same shipFrom and shipTo are grouped into 1 "parcel"
				carrier_accounts = profiles.ShippingProfiles.Select(id => new EasyPostCarrierAccount() { id = id.CarrierAccountID }).ToList()
			};

			// add customs info for international shipments
            if (groupedLineItems.Key.ShipTo.Country != "US")
            {
                var line_item = groupedLineItems.First(g => g.SupplierID != null);

				var profile = profiles.FirstOrDefault(line_item.SupplierID);
                shipment.customs_info = new EasyPostCustomsInfo()
                {
					contents_type = "merchandise",
					restriction_type = profile.Restriction_Type,
					eel_pfc = profile.EEL_PFC,
					customs_certify = profile.Customs_Certify,
					customs_signer = profile.Customs_Signer,
					customs_items = MapCustomsItem(groupedLineItems, profile)
                };
            }
            return shipment;
        }
    }
}
