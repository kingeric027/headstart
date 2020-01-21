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
				ClientId = "2234C6E1-8FA5-41A2-8A7F-A560C6BA44D8",
				ClientSecret = "z08ibzgsb337ln8EzJx5efI1VKxqdqeBW0IB7p1SJaygloJ4J9uZOtPu1Aql",
				Roles = new[] { ApiRole.FullAccess }
			});
		}
	}
}
