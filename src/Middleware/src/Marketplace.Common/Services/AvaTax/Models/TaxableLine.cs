using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Services.AvaTax.Models
{
	public class TaxableLine
	{
		public decimal Amount { get; set; }
		public string TaxCode { get; set; } // how to use?
		public string ItemCode { get; set; } // productID 
		public string CustomerUsageType { get; set; } // how to use?
		public string LineNumber { get; set; } // lineItemID
	}
}
