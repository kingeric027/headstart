using Avalara.AvaTax.RestClient;
using Marketplace.Common.Services.FreightPop;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marketplace.Common.Mapping
{
	public static class TransactionLineMapper
	{
		public static IEnumerable<TransactionLineModel> MapFrom(IEnumerable<LineItem> lineItems)
		{
			return lineItems.Select(li => MapFrom(li));
		}

		public static TransactionLineModel MapFrom(LineItem lineItem)
		{
			return new TransactionLineModel()
			{
				lineAmount = lineItem.LineTotal,
				taxCode = null,
				itemCode = lineItem.ProductID,
				customerUsageType = null,
				lineNumber = lineItem.ID
			};
		}

		public static IEnumerable<TransactionLineModel> MapFrom(IEnumerable<ShippingRate> shippingRate)
		{
			return shippingRate.Select(li => MapFrom(li));
		}

		public static TransactionLineModel MapFrom(ShippingRate shippingRate)
		{
			return new TransactionLineModel()
			{
				lineAmount = shippingRate.TotalCost,
				taxCode = null,
				itemCode = shippingRate.Id,
				customerUsageType = null,
				lineNumber = null
			};
		}
	}
}
