using Avalara.AvaTax.RestClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Models
{
	// TODO - Re-evaluate. What fields are needed? Where should it be stored?
	public class TaxBreakdown
	{
		public decimal TotalTax { get; set; }
		public decimal TaxableAmount { get; set; }
		public IEnumerable<TaxLine> TaxLines { get; set; }
	}

	public class TaxLine
	{
		public decimal Tax { get; set; }
		public decimal TaxableAmount { get; set; }
		public string TaxName { get; set; }
		public TaxType? TaxType { get; set; }
		public string JurisdictionName { get; set; }
	}
}
