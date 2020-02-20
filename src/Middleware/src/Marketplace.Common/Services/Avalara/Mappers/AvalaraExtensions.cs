using Avalara.AvaTax.RestClient;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Models.Models.Marketplace;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Extensions
{
	public static class AvalaraExtensions
	{
		private static TransactionBuilder WithLineShipTo(this TransactionBuilder trans, Address address)
		{
			return trans.WithLineAddress(TransactionAddressType.ShipTo, address.Street1, address.Street2, null, address.City, address.State, address.Zip, address.Country);
		}

		private static TransactionBuilder WithLineShipFrom(this TransactionBuilder trans, Address address)
		{
			return trans.WithLineAddress(TransactionAddressType.ShipFrom, address.Street1, address.Street2, null, address.City, address.State, address.Zip, address.Country);
		}

		private static TransactionBuilder WithLine(this TransactionBuilder trans, TransactionLineModel line)
		{
			return trans.WithLine(line.lineAmount ?? 0, 1, line.taxCode, null, line.itemCode, line.customerUsageType, line.lineNumber);
		}

		private static TransactionBuilder WithLine(this TransactionBuilder trans, TransactionLineModel line, Address shipFrom, Address shipTo)
		{
			return trans.WithLine(line).WithLineShipFrom(shipFrom).WithLineShipTo(shipTo);
		}

		public static TransactionBuilder WithLineItem(this TransactionBuilder trans, MarketplaceLineItem lineItem)
		{
			var line = new TransactionLineModel()
			{
				lineAmount = lineItem.LineTotal,
				taxCode = lineItem.Product.xp.Tax.Code,
				itemCode = lineItem.ProductID,
				customerUsageType = null,
				lineNumber = lineItem.ID
			};
			return trans.WithLine(line, lineItem.ShipFromAddress, lineItem.ShippingAddress);
		}

		public static TransactionBuilder WithShippingRate(this TransactionBuilder trans, decimal rate, Address shipFrom, Address shipTo)
		{
			var shipping = new TransactionLineModel()
			{
				lineAmount = rate,
				taxCode = "FR",
				// replace this itemCode with the proposedshipment ID when this is being generated from the platform
				itemCode = shipFrom.ID,
				customerUsageType = null,
				lineNumber = null
			};
			return trans.WithLine(shipping, shipFrom, shipTo);

		}

	}
}
