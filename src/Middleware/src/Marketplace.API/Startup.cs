using Avalara.AvaTax.RestClient;
using Flurl.Http;
using Integrations.Avalara;
using Integrations.CMS;
using Integrations.SmartyStreets;
using Marketplace.CMS.Models;
using Marketplace.CMS.Queries;
using Marketplace.CMS.Storage;
using Marketplace.Common;
using Marketplace.Common.Commands;
using Marketplace.Common.Commands.Crud;
using Marketplace.Common.Commands.Zoho;
using Marketplace.Common.Helpers;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Marketplace.Common.Services;
using Marketplace.Common.Services.Avalara;
using Marketplace.Common.Services.DevCenter;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Common.Services.ShippingIntegration;
using Marketplace.Common.Services.SmartyStreets;
using Marketplace.Common.Services.Zoho;
using Marketplace.Helpers;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Models;
using Marketplace.Models;
using Marketplace.Models.Extended;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ordercloud.integrations.cardconnect;
using OrderCloud.SDK;

namespace Marketplace.API
{
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
			var avalaraConfig = new AvalaraConfig()
			{
				Env = _settings.Env == AppEnvironment.Prod ? AvaTaxEnvironment.Production : AvaTaxEnvironment.Sandbox,
				AccountID = _settings.AvalaraSettings.AccountID,
				LicenseKey = _settings.AvalaraSettings.LicenseKey,
				CompanyCode = _settings.AvalaraSettings.CompanyCode,
				HostUrl = _settings.EnvironmentSettings.BaseUrl
			};
			var cmsConfig = new CMSConfig()
			{
				BlobStorageHostUrl = _settings.BlobSettings.HostUrl,
				BlobStorageConnectionString = _settings.BlobSettings.ConnectionString
			};
			services
				.ConfigureWebApiServices(_settings)
				.ConfigureOpenApiSpec("v1", "Marketplace API")
				.Inject<IAppSettings>()
				.Inject<IDevCenterService>()
				.Inject<IFlurlClient>()
				.Inject<IZohoClient>()
				.Inject<IZohoCommand>()
				.Inject<ISyncCommand>()
				.AddSingleton<IAvalaraCommand>(x => new AvalaraCommand(avalaraConfig))
				.Inject<IFreightPopService>()
				.InjectCosmosStore<LogQuery, OrchestrationLog>(cosmosConfig)
				.InjectCosmosStore<SupplierCategoryConfigQuery, SupplierCategoryConfig>(cosmosConfig)
				.InjectCosmosStore<ImageQuery, Image>(cosmosConfig)
				.InjectCosmosStore<ImageProductAssignmentQuery, ImageProductAssignment>(cosmosConfig)
				.InjectCosmosStore<AssetQuery, Asset>(cosmosConfig)
				.InjectCosmosStore<AssetContainerQuery, AssetContainer>(cosmosConfig)
				.InjectCosmosStore<AssetAssignmentQuery, AssetAssignment>(cosmosConfig)
				.AddSingleton<IStorageFactory>(x => new StorageFactory(cmsConfig))
				.Inject<IOrchestrationCommand>()
				.Inject<IOrchestrationLogCommand>()
				.Inject<IOCShippingIntegration>()
				.Inject<IShipmentCommand>()
				.AddSingleton<ISmartyStreetsService>(x => new SmartyStreetsService(_settings.SmartyStreetSettings))
				.Inject<IValidatedAddressCommand>()
				.Inject<IEnvironmentSeedCommand>()
				.Inject<IOrderCloudSandboxService>()
				.Inject<IMarketplaceProductCommand>()
				.Inject<ISendgridService>()
				.Inject<IAssetQuery>()
				.Inject<ISupplierCategoryConfigQuery>()
				.InjectOrderCloud<IOrderCloudClient>(new OrderCloudClientConfig
                {
                    ApiUrl = _settings.OrderCloudSettings.ApiUrl,
                    AuthUrl = _settings.OrderCloudSettings.AuthUrl,
                    ClientId = _settings.OrderCloudSettings.ClientID,
                    ClientSecret = _settings.OrderCloudSettings.ClientSecret,
                    Roles = new[]
                    {
                        ApiRole.FullAccess
                    }
                })
				.AddSingleton<IOrderCloudIntegrationsCardConnectService>(x => new OrderCloudIntegrationsCardConnectService(_settings.CardConnectSettings))
                .Inject<IOrderCloudIntegrationsCardConnectCommand>()
                .Inject<IMarketplaceSupplierCommand>()
                .AddAuthenticationScheme<DevCenterUserAuthOptions, DevCenterUserAuthHandler>("DevCenterUser")
                .AddAuthenticationScheme<MarketplaceUserAuthOptions, MarketplaceUserAuthHandler>("MarketplaceUser")
                .AddAuthenticationScheme<OrderCloudWebhookAuthOptions, OrderCloudWebhookAuthHandler>(
                    "OrderCloudWebhook",
                    opts => opts.HashKey = _settings.OrderCloudSettings.WebhookHashKey)
                .AddAuthentication();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.ConfigureWebApp(env, "v1");
        }
    }
}
