using Marketplace.Common.Services.AvaTax.Models;
using Marketplace.Common.Services.FreightPop;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marketplace.Common.Mapping
{
	public static class TaxableLineMapper
	{
		public static IEnumerable<TaxableLine> MapFrom(IEnumerable<LineItem> lineItems)
		{
			return lineItems.Select(li => MapFrom(li));
		}

		public static TaxableLine MapFrom(LineItem lineItem)
		{
			return new TaxableLine()
			{
				Amount = lineItem.LineTotal,
				TaxCode = null,
				ItemCode = lineItem.ProductID,
				CustomerUsageType = null,
				LineNumber = lineItem.ID
			};
		}

		public static IEnumerable<TaxableLine> MapFrom(IEnumerable<ShippingRate> shippingRate)
		{
			return shippingRate.Select(li => MapFrom(li));
		}

		public static TaxableLine MapFrom(ShippingRate shippingRate)
		{
			return new TaxableLine()
			{
				Amount = shippingRate.TotalCost,
				TaxCode = null,
				ItemCode = shippingRate.Id,
				CustomerUsageType = null,
				LineNumber = null
			};
		}
	}
}
