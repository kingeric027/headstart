using Flurl.Http;
using Marketplace.Common;
using Marketplace.Common.Commands;
using Marketplace.Common.Commands.Crud;
using Marketplace.Common.Commands.Zoho;
using Marketplace.Common.Helpers;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Marketplace.Common.Services;
using Marketplace.Common.Services.Avalara;
using Marketplace.Common.Services.CardConnect;
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
            services
                .ConfigureWebApiServices(_settings)
				.ConfigureOpenApiSpec("v1", "Marketplace API")
                .Inject<IAppSettings>()
                .Inject<IDevCenterService>()
                .Inject<IFlurlClient>()
                .Inject<IZohoClient>()
                .Inject<IZohoCommand>()
                .Inject<ISyncCommand>()
                .Inject<IAvalaraService>()
                .Inject<IFreightPopService>()
                .Inject<ISmartyStreetsService>()
                .InjectCosmosStore<LogQuery, OrchestrationLog>(cosmosConfig)
                .InjectCosmosStore<SupplierCategoryConfigQuery, SupplierCategoryConfig>(cosmosConfig)
                .Inject<IOrchestrationCommand>()
                .Inject<IOrchestrationLogCommand>()
                .Inject<IOCShippingIntegration>()
                .Inject<IShipmentCommand>()
                .Inject<IValidatedAddressCommand>()
                .Inject<IEnvironmentSeedCommand>()
                .Inject<IOrderCloudSandboxService>()
                .Inject<IMarketplaceProductCommand>()
                .Inject<ISupplierCategoryConfigQuery>()
                .Inject<ISendgridService>()
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
                .Inject<ICardConnectService>()
                .Inject<ICreditCardCommand>()
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
