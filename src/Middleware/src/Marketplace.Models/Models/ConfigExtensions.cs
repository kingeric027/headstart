using System;
using Microsoft.AspNetCore.Authentication;
using Marketplace.Models;

namespace Marketplace.Models
{
	public static class ConfigExtensions
	{
		/// <summary>
		/// Call inside of services.AddAuthorization(...) (typically in Startup.ConfigureServices) to enable validation of incoming webhooks.
		/// </summary>
		public static AuthenticationBuilder AddOrderCloudWebhooks(this AuthenticationBuilder builder, Action<OrderCloudWebhookAuthOptions> configureOptions)
		{
			return builder.AddScheme<OrderCloudWebhookAuthOptions, OrderCloudWebhookAuthHandler>("OrderCloudWebhook", null, configureOptions);
		}
	}
}