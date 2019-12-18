using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Marketplace.Common;
using Marketplace.Helpers;
using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Models;
using Marketplace.Helpers.Extensions;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Marketplace.Common.Commands;

namespace Marketplace.API
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			Marketplace.Helpers.WebHostBuilder
				.CreateWebHostBuilder<Startup, AppSettings>(args)
				.Build()
				.Run();
		}

		public class Startup
		{
			private readonly AppSettings _settings;

			public Startup(AppSettings settings)
			{
				_settings = settings;
			}

			// This method gets called by the runtime. Use this method to add services to the container.
			// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
			public void ConfigureServices(IServiceCollection services)
			{
				var cosmosConfig = new CosmosConfig(_settings.CosmosSettings.DatabaseName,
					_settings.CosmosSettings.EndpointUri, _settings.CosmosSettings.PrimaryKey);

				services.SharedServices();

				services
					.ConfigureWebApiServices(_settings, "v1", "Marketplace API")
					.Inject<IOrchestrationLogCommand>()
					.Inject<IAppSettings>()
					.Inject<IEnvironmentSeedCommand>()
					.Inject<ISupplierCategoryConfigQuery>()
					.InjectCosmosStore<OrchestrationLog, LogQuery>(cosmosConfig)
					.InjectCosmosStore<SupplierCategoryConfig, SupplierCategoryConfigQuery>(cosmosConfig)
					.AddAuthenticationScheme("MarketplaceUser");
			}

			// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
			public static void Configure(IApplicationBuilder app, IHostingEnvironment env)
			{
				app.ConfigureWebApp(env, "v1");
			}
		}
	}        
}
