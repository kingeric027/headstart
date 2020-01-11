using Avalara.AvaTax.RestClient;
using Marketplace.Common.Services.FreightPop;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Extensions
{
	public static class AvalaraExtensions
	{
		public static TransactionBuilder WithShipTo(this TransactionBuilder builder, Address address)
		{
			return builder.WithAddress(TransactionAddressType.ShipTo, address.Street1, address.Street2, null, address.City, address.State, address.Zip, address.Country);
		}

		public static TransactionBuilder WithShipFrom(this TransactionBuilder builder, Address address)
		{
			return builder.WithAddress(TransactionAddressType.ShipFrom, address.Street1, address.Street2, null, address.City, address.State, address.Zip, address.Country);
		}

		public static TransactionBuilder WithLine(this TransactionBuilder builder, TransactionLineModel line)
		{
			return builder.WithLine(line.lineAmount ?? 0, 1, line.taxCode, null, line.itemCode, line.customerUsageType, line.lineNumber);
		}

		public static TransactionBuilder WithLineItem(this TransactionBuilder builder, LineItem lineItem)
		{
			return builder.WithLine(new TransactionLineModel()
			{
				lineAmount = lineItem.LineTotal,
				taxCode = null,
				itemCode = lineItem.ProductID,
				customerUsageType = null,
				lineNumber = lineItem.ID
			});
		}

		public static TransactionBuilder WithShippingRate(this TransactionBuilder builder, ShippingRate shipping)
		{
			return builder.WithLine(new TransactionLineModel()
			{
				lineAmount = shipping.TotalCost,
				taxCode = "FR",
				itemCode = shipping.Id,
				customerUsageType = null,
				lineNumber = null
			});
		}
	}
}
