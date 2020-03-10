using System;
using Microsoft.AspNetCore.Hosting;
using Marketplace.Common;

namespace Marketplace.API
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			// Links to an Azure App Configuration resource that holds the app settings.
			// Set this in your visual studio Env Variables.
			var connectionString = Environment.GetEnvironmentVariable("APP_CONFIG_CONNECTION"); 

			Marketplace.Helpers.WebHostBuilder
				.CreateWebHostBuilder<Startup, AppSettings>(args, connectionString)
				.Build()
				.Run();
		}
	}
}
