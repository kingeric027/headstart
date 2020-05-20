using System;
using System.Collections.Generic;
using System.Text;

namespace Integrations.Avalara.Models
{
	public class TaxCodesListArgs
	{
		public int Top { get; set; }
		public int Skip { get; set; }
		public string Filter { get; set; }
		public string OrderBy { get; set; }
		public string CodeCategory { get; set; }
	}
}
