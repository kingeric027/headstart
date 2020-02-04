using System;
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
using Marketplace.Common.Commands.CardConnect;
using Marketplace.Common.Commands.Crud;
using Marketplace.Common.Helpers;
using Marketplace.Common.Services.DevCenter;
using Marketplace.Common.Services;
using Marketplace.Common.Services.CardConnect;
using Marketplace.Helpers;
using OrderCloud.SDK;

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
                var sdk = new OrderCloudClient(new OrderCloudClientConfig
                {
					// TODO: move these to AppSettings
                    ApiUrl = "https://api.ordercloud.io",
                    AuthUrl = "https://auth.ordercloud.io",
                    ClientId = "2234C6E1-8FA5-41A2-8A7F-A560C6BA44D8",
                    ClientSecret = "z08ibzgsb337ln8EzJx5efI1VKxqdqeBW0IB7p1SJaygloJ4J9uZOtPu1Aql",
                    Roles = new[]
                    {
                        ApiRole.FullAccess
                    }
                });
				services
					.ConfigureWebApiServices(_settings, "v1", "Marketplace API")
                    .Inject<IAppSettings>()
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
                    .AddTransient<IOrderCloudClient>(s => sdk)
                    .Inject<ICardConnectService>()
                    .Inject<ICreditCardCommand>()
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
