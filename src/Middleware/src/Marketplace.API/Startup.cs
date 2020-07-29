using System;
using Avalara.AvaTax.RestClient;
using Flurl.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Marketplace.Common;
using Marketplace.Common.Commands;
using Marketplace.Common.Commands.Crud;
using Marketplace.Common.Commands.Zoho;
using Marketplace.Common.Controllers;
using Marketplace.Common.Helpers;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Marketplace.Common.Services;
using Marketplace.Common.Services.DevCenter;
using Marketplace.Common.Services.ShippingIntegration;
using Marketplace.Common.Services.Zoho;
using Marketplace.Models.Extended;
using ordercloud.integrations.cms;
using OrderCloud.SDK;
using Swashbuckle.AspNetCore.Swagger;
using ordercloud.integrations.smartystreets;
using ordercloud.integrations.avalara;
using ordercloud.integrations.cardconnect;
using ordercloud.integrations.exchangerates;
using ordercloud.integrations.freightpop;
using ordercloud.integrations.library;
using Document = ordercloud.integrations.cms.Document;

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
				BaseUrl = _settings.EnvironmentSettings.BaseUrl,
				BlobStorageHostUrl = _settings.BlobSettings.HostUrl,
				BlobStorageConnectionString = _settings.BlobSettings.ConnectionString
			};
            var currencyConfig = new BlobServiceConfig()
            {
                ConnectionString = _settings.ExchangeRatesSettings.ConnectionString,
                Container = _settings.ExchangeRatesSettings.Container
            };
            var freightPopConfig = new FreightPopConfig()
            {
                BaseUrl = _settings.FreightPopSettings.BaseUrl,
                Password = _settings.FreightPopSettings.Password,
                Username = _settings.FreightPopSettings.Username
            };

            services
                .OrderCloudIntegrationsConfigureWebApiServices(_settings, "marketplacecors")
                .InjectCosmosStore<LogQuery, OrchestrationLog>(cosmosConfig)
                .InjectCosmosStore<AssetQuery, Asset>(cosmosConfig)
				.InjectCosmosStore<DocumentSchema, DocumentSchema>(cosmosConfig)
				.InjectCosmosStore<Document, Document>(cosmosConfig)
				.InjectCosmosStore<DocumentAssignment, DocumentAssignment>(cosmosConfig)
				.InjectCosmosStore<AssetContainerQuery, AssetContainer>(cosmosConfig)
                .InjectCosmosStore<AssetedResourceQuery, AssetedResource>(cosmosConfig).Inject<AppSettings>()
                .InjectCosmosStore<ChiliPublishConfigQuery, ChiliConfig>(cosmosConfig)
                .Inject<IDevCenterService>()
                .Inject<IFlurlClient>()
                .Inject<IZohoClient>()
                .Inject<ISyncCommand>()
                .Inject<ISmartyStreetsCommand>()
                .Inject<IOrchestrationCommand>()
                .Inject<IOrchestrationLogCommand>()
                .Inject<IOCShippingIntegration>()
                .Inject<IShipmentCommand>()
                .Inject<IEnvironmentSeedCommand>()
                .Inject<IOrderCloudSandboxService>()
                .Inject<IMarketplaceProductCommand>()
                .Inject<IMeProductCommand>()
                .Inject<IMarketplaceCatalogCommand>()
                .Inject<ISendgridService>()
                .Inject<IAssetQuery>()
				.Inject<IDocumentQuery>()
				.Inject<IBlobStorage>()
				.Inject<IDocumentSchemaQuery>()
                .Inject<IMarketplaceSupplierCommand>()
                .Inject<IOrderCloudIntegrationsCardConnectCommand>()
                .Inject<IChiliTemplateCommand>()
                .Inject<IChiliConfigCommand>()
                .AddSingleton<IZohoCommand>(z => new ZohoCommand(new ZohoClientConfig() {
                    ApiUrl = "https://books.zoho.com/api/v3",
                    AccessToken = _settings.ZohoSettings.AccessToken,
                    ClientId = _settings.ZohoSettings.ClientId,
                    ClientSecret = _settings.ZohoSettings.ClientSecret,
                    OrganizationID = _settings.ZohoSettings.OrgID }, 
                    new OrderCloudClientConfig {
                        ApiUrl = _settings.OrderCloudSettings.ApiUrl,
                        AuthUrl = _settings.OrderCloudSettings.ApiUrl,
                        ClientId = _settings.OrderCloudSettings.ClientID,
                        ClientSecret = _settings.OrderCloudSettings.ClientSecret,
                        Roles = new[]
                            {
                                ApiRole.FullAccess
                            }
                    }
                ))
				.AddSingleton<CMSConfig>(x => cmsConfig)
                .AddSingleton<IFreightPopService>(x => new FreightPopService(freightPopConfig))
                .AddSingleton<IExchangeRatesCommand>(x => new ExchangeRatesCommand(currencyConfig))
				.AddSingleton<IAvalaraCommand>(x => new AvalaraCommand(avalaraConfig))
                .AddSingleton<ISmartyStreetsService>(x => new SmartyStreetsService(_settings.SmartyStreetSettings))
                .AddSingleton<IOrderCloudIntegrationsCardConnectService>(x => new OrderCloudIntegrationsCardConnectService(_settings.CardConnectSettings))
                .AddAuthenticationScheme<DevCenterUserAuthOptions, DevCenterUserAuthHandler>("DevCenterUser")
                .AddAuthenticationScheme<OrderCloudIntegrationsAuthOptions, OrderCloudIntegrationsAuthHandler>("OrderCloudIntegrations")
                .AddAuthenticationScheme<OrderCloudWebhookAuthOptions, OrderCloudWebhookAuthHandler>("OrderCloudWebhook", opts => opts.HashKey = _settings.OrderCloudSettings.WebhookHashKey)
                .AddTransient<IOrderCloudClient>(provider => new OrderCloudClient(new OrderCloudClientConfig
                {
                    ApiUrl = _settings.OrderCloudSettings.ApiUrl,
                    AuthUrl = _settings.OrderCloudSettings.ApiUrl,
                    ClientId = _settings.OrderCloudSettings.ClientID,
                    ClientSecret = _settings.OrderCloudSettings.ClientSecret,
                    Roles = new[]
                    {
                        ApiRole.FullAccess
                    }
                }))
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "Marketplace API", Version = "v1" });
                    c.CustomSchemaIds(x => x.FullName);
                })
                .AddAuthentication();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.OrderCloudIntegrationsConfigureWebApp(env, "v1")
                .UseSwagger()
                .UseSwaggerUI(c => {
                    c.SwaggerEndpoint($"/swagger/v1/swagger.json", $"API v1");
                    c.RoutePrefix = string.Empty;
                });
        }
    }
}
