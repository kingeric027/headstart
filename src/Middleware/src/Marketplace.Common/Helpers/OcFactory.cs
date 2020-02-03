using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Helpers
{
	static class OcFactory
	{
		// THIS needs to be totally reworked so it works across Ordercloud Orgs.
		public static IOrderCloudClient GetSEBAdmin()
		{
			return new OrderCloudClient(new OrderCloudClientConfig()
			{
				ClientId = "97349A89-E279-45BE-A072-83DF8F7043F4",
				ClientSecret = "d576450ca8f89967eea0d3477544ea4bee60af051a5c173be09db08c562b",
				Roles = new[] { ApiRole.FullAccess }
			});
		}
	}
}
