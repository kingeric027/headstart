using Avalara.AvaTax.RestClient;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Extensions
{
	public static class AvalaraExtensions
	{
		public static TransactionBuilder WithAddress(this TransactionBuilder builder, TransactionAddressType type, Address address)
		{
			return builder.WithAddress(type, address.Street1, address.Street2, null, address.City, address.State, address.Zip, address.Country);
		}

		public static TransactionBuilder WithLine(this TransactionBuilder builder, TransactionLineModel line)
		{
			return builder.WithLine(line.lineAmount ?? 0, 1, line.taxCode, null, line.itemCode, line.customerUsageType, line.lineNumber);
		}
	}
}
