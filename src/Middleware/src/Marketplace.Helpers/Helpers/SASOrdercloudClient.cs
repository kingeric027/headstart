using Marketplace.Helpers.Models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Helpers.Helpers
{
	public class SASOrdercloudClient : OrderCloudClient, IOrderCloudClient
	{
		public SASOrdercloudClient(VerifiedUserContext user) : base(
			new OrderCloudClientConfig()
			{
				ApiUrl = user.ApiUrl,
				AuthUrl = user.AuthUrl,
				ClientId = user.ClientID,
				Roles = new[] { ApiRole.FullAccess }
			}
		) { }
	}
}
