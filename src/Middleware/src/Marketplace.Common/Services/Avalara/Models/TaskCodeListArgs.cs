using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Services.AvaTax.Models
{
	public class TaxCodeListArgs
	{
		public int Top { get; set; }
		public int Skip { get; set; }
		public string Filter { get; set; }
		public string OrderBy { get; set; }
		public string CodeCategory { get; set; }
	}
}
