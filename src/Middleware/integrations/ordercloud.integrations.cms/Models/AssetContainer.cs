using System;
using System.Collections.Generic;
using System.Text;

namespace ordercloud.integrations.cms
{
	public class AssetContainer
	{
		public string id { get; set; }
		public Customer Customer { get; set; }
		public string BuyerID { get; set; }
		public string SupplierID { get; set; }
	}
}
