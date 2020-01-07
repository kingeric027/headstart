using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Services.FreightPop
{
	public class GetRatesData
	{
		public IEnumerable<ShippingRate> Rates { get; set;} 
		public IEnumerable<string> ErrorMessages { get; set; }
	}
}
