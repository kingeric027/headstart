﻿using System;
using Flurl.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Marketplace.Common;
using Marketplace.Helpers.Models;
using Marketplace.Helpers.Extensions;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Marketplace.Common.Commands;
using Marketplace.Common.Commands.Crud;
using Marketplace.Common.Helpers;
using Marketplace.Common.Services.DevCenter;
using Marketplace.Common.Services;
using Marketplace.Helpers;
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

				services
					.ConfigureWebApiServices(_settings, "v1", "Marketplace API")
                    .Inject<IDevCenterService>()
                    .Inject<IFlurlClient>()
                    .Inject<ISyncCommand>()
					.Inject<IAvataxService>()
					.Inject<IFreightPopService>()
					.InjectCosmosStore<LogQuery, OrchestrationLog>(cosmosConfig)
					.InjectCosmosStore<SupplierCategoryConfigQuery, SupplierCategoryConfig>(cosmosConfig)
                    .Inject<IOrchestrationCommand>()
                    .Inject<IOrchestrationLogCommand>()
					.Inject<IOCShippingIntegration>()
					.Inject<IProposedShipmentCommand>()
					.Inject<IEnvironmentSeedCommand>()
                    .Inject<IMarketplaceProductCommand>()
					.Inject<ITaxCommand>()
					.Inject<ISupplierCategoryConfigQuery>()
					.Inject<ISendgridService>()
                    .AddAuthenticationScheme<DevCenterUserAuthOptions, DevCenterUserAuthHandler>("DevCenterUser")
                    .AddAuthenticationScheme<MarketplaceUserAuthOptions, MarketplaceUserAuthHandler>("MarketplaceUser");
            }

			// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
			public static void Configure(IApplicationBuilder app, IHostingEnvironment env)
			{
				app.ConfigureWebApp(env, "v1");
			}
		}
	}        
}
