using Avalara.AvaTax.RestClient;
using Marketplace.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marketplace.Common.Mapping.TaxBreakdownMapper.cs
{
	public static class TaxBreakdownMapper
	{
		public static TaxBreakdown MapFrom(TransactionModel source)
		{
			return new TaxBreakdown()
			{
				TotalTax = source.totalTax ?? 0,
				TaxableAmount = source.totalTaxable ?? 0,
				TaxLines = source.summary.Select(taxLine =>
				{
					return new TaxLine()
					{
						Tax = taxLine.tax ?? 0,
						TaxableAmount = taxLine.taxable ?? 0,
						TaxName = taxLine.taxName,
						JurisdictionName = taxLine.jurisName,
						TaxType = taxLine.taxType
					};
				})
			};
		}
	}
}
