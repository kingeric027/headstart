using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Models.Models.Misc
{
	public class MarketplaceSecurityProfile
	{
		public CustomRole CustomRole { get; set; }
		public ApiRole[] Roles { get; set; }
	}
}
