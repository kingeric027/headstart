﻿using Avalara.AvaTax.RestClient;
using Marketplace.Common.Services.FreightPop;
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

		public static TransactionBuilder WithLineItem(this TransactionBuilder trans, LineItem lineItem)
		{
			var line = new TransactionLineModel()
			{
				lineAmount = lineItem.LineTotal,
				taxCode = null,
				itemCode = lineItem.ProductID,
				customerUsageType = null,
				lineNumber = lineItem.ID
			};
			return trans.WithLine(line, lineItem.ShipFromAddress, lineItem.ShippingAddress);
		}

		public static TransactionBuilder WithShippingRate(this TransactionBuilder trans, ShippingRate rate, Address shipFrom, Address shipTo)
		{
			var shipping = new TransactionLineModel()
			{
				lineAmount = rate.TotalCost,
				taxCode = "FR",
				itemCode = rate.Id,
				customerUsageType = null,
				lineNumber = null
			};
			return trans.WithLine(shipping, shipFrom, shipTo);

		}

	}
}
