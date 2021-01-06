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
using Marketplace.Common.Helpers;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Marketplace.Common.Services;
using Marketplace.Common.Services.DevCenter;
using Marketplace.Common.Services.ShippingIntegration;
using Marketplace.Common.Services.Zoho;
using OrderCloud.SDK;
using Swashbuckle.AspNetCore.Swagger;
using ordercloud.integrations.smartystreets;
using ordercloud.integrations.easypost;
using ordercloud.integrations.avalara;
using ordercloud.integrations.cardconnect;
using ordercloud.integrations.exchangerates;
using ordercloud.integrations.library;
using ordercloud.integrations.tecra;
using ordercloud.integrations.tecra.Storage;
using System.Runtime.InteropServices;
using LazyCache;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Flurl.Http.Configuration;
using System.Net;
using Microsoft.ApplicationInsights;
using Microsoft.WindowsAzure.Storage.Blob;
using SendGrid;
using SmartyStreets;
using SmartyStreets.USStreetApi;
using Marketplace.Common.Services.CMS;
using Common.Services.AnytimeDashboard;
using Marketplace.Common.Services.WazingDashboard;

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
			var cosmosConfig = new CosmosConfig(
                _settings.CosmosSettings.DatabaseName,
                _settings.CosmosSettings.EndpointUri,
                _settings.CosmosSettings.PrimaryKey,
                _settings.CosmosSettings.RequestTimeoutInSeconds,
                _settings.CosmosSettings.MaxConnectionLimit,
                _settings.CosmosSettings.IdleTcpConnectionTimeoutInMinutes,
                _settings.CosmosSettings.OpenTcpConnectionTimeoutInSeconds,
                _settings.CosmosSettings.MaxTcpConnectionsPerEndpoint,
                _settings.CosmosSettings.MaxRequestsPerTcpConnection,
                _settings.CosmosSettings.EnableTcpConnectionEndpointRediscovery
            );

            var avalaraConfig = new AvalaraConfig()
			{
				Env = _settings.Env == AppEnvironment.Prod ? AvaTaxEnvironment.Production : AvaTaxEnvironment.Sandbox,
				AccountID = _settings.AvalaraSettings.AccountID,
				LicenseKey = _settings.AvalaraSettings.LicenseKey,
				CompanyCode = _settings.AvalaraSettings.CompanyCode,
                CompanyID = _settings.AvalaraSettings.CompanyID
			};
            var tecraConfig = _settings.TecraSettings;
            tecraConfig.BlobStorageHostUrl = _settings.BlobSettings.HostUrl;
            tecraConfig.BlobStorageConnectionString = _settings.BlobSettings.ConnectionString;

            var currencyConfig = new BlobServiceConfig()
            {
                ConnectionString = _settings.ExchangeRatesSettings.ConnectionString,
                Container = _settings.ExchangeRatesSettings.Container
            };
            var middlewareErrorsConfig = new BlobServiceConfig()
            {
                ConnectionString = _settings.BlobSettings.ConnectionString,
                Container = "unhandled-errors-log"
            };

            var flurlClientFactory = new PerBaseUrlFlurlClientFactory();
            var smartyStreetsUsClient = new ClientBuilder(_settings.SmartyStreetSettings.AuthID, _settings.SmartyStreetSettings.AuthToken).BuildUsStreetApiClient();

            services
                .AddLazyCache()
                .OrderCloudIntegrationsConfigureWebApiServices(_settings, middlewareErrorsConfig, "marketplacecors")
                .InjectCosmosStore<LogQuery, OrchestrationLog>(cosmosConfig)
                .InjectCosmosStore<ChiliPublishConfigQuery, ChiliConfig>(cosmosConfig)
                .InjectCosmosStore<ReportTemplateQuery, ReportTemplate>(cosmosConfig)
                .InjectCosmosStore<ResourceHistoryQuery<ProductHistory>, ProductHistory>(cosmosConfig)
                .InjectCosmosStore<ResourceHistoryQuery<PriceScheduleHistory>, PriceScheduleHistory>(cosmosConfig)
                .Inject<ISSOLoginCommand>()
                .Inject<IAnytimeWebhookCommand>()
                .Inject<IAnytimeDashboardClient>()
                .Inject<IWaxDashboardClient>()
                .AddTransient<ISyncOrgCommand, SyncOrgCommand>()
                .Inject<IDevCenterService>()
                .Inject<ISyncCommand>()
                .Inject<ISmartyStreetsCommand>()
                .Inject<IOrchestrationCommand>()
                .Inject<IOrchestrationLogCommand>()
                .Inject<ICheckoutIntegrationCommand>()
                .Inject<IShipmentCommand>()
                .Inject<IOrderCommand>()
                .Inject<IPaymentCommand>()
                .Inject<IOrderSubmitCommand>()
                .Inject<IEnvironmentSeedCommand>()
                .Inject<IMarketplaceProductCommand>()
                .Inject<ILineItemCommand>()
                .Inject<IMeProductCommand>()
                .Inject<IMarketplaceCatalogCommand>()
                .Inject<ISendgridService>()
                .Inject<IMarketplaceSupplierCommand>()
                .Inject<ICreditCardCommand>()
                .Inject<ISupportAlertService>()
                .Inject<IOrderCalcService>()
                .Inject<IOrderCloudIntegrationsTecraCommand>()
                .Inject<IChiliTemplateCommand>()
                .Inject<IChiliConfigCommand>()
                .Inject<IOrderCloudIntegrationsTecraCommand>()
                .AddSingleton<IChiliBlobStorage>(x => new ChiliBlobStorage(tecraConfig, new OrderCloudIntegrationsBlobService(new BlobServiceConfig()
                {
                    ConnectionString = tecraConfig.BlobStorageConnectionString,
                    Container = "chili-assets",
                    AccessType = BlobContainerPublicAccessType.Container
                })))
                .Inject<ISupplierApiClientHelper>()
                .AddSingleton<ICMSClient>(new CMSClient(new CMSClientConfig() { BaseUrl = _settings.CMSSettings.BaseUrl }))
                .AddSingleton<OrderCloudTecraConfig>(x => _settings.TecraSettings)
                .Inject<IOrderCloudIntegrationsTecraService>()
                .AddSingleton<ISendGridClient>(x => new SendGridClient(_settings.SendgridSettings.ApiKey))
                .AddSingleton<IFlurlClientFactory>(x => flurlClientFactory)
                .AddSingleton<DownloadReportCommand>()
                .Inject<IZohoClient>()
                .AddSingleton<IZohoCommand>(z => new ZohoCommand(new ZohoClient(
                    new ZohoClientConfig() {
                        ApiUrl = "https://books.zoho.com/api/v3",
                        AccessToken = _settings.ZohoSettings.AccessToken,
                        ClientId = _settings.ZohoSettings.ClientId,
                        ClientSecret = _settings.ZohoSettings.ClientSecret,
                        OrganizationID = _settings.ZohoSettings.OrgID
                    }, flurlClientFactory), 
                    new OrderCloudClient(new OrderCloudClientConfig
                    {
                        ApiUrl = _settings.OrderCloudSettings.ApiUrl,
                        AuthUrl = _settings.OrderCloudSettings.ApiUrl,
                        ClientId = _settings.OrderCloudSettings.ClientID,
                        ClientSecret = _settings.OrderCloudSettings.ClientSecret,
                        Roles = new[] { ApiRole.FullAccess }
                })))
                .AddSingleton<IOrderCloudIntegrationsExchangeRatesClient, OrderCloudIntegrationsExchangeRatesClient>()
                .AddSingleton<IExchangeRatesCommand>(provider => new ExchangeRatesCommand(currencyConfig, flurlClientFactory, provider.GetService<IAppCache>()))
                .AddSingleton<IAvalaraCommand>(x => new AvalaraCommand(avalaraConfig, 
                        new AvaTaxClient("four51 marketplace", "v1", "machine_name", avalaraConfig.Env)
                            .WithSecurity(_settings.AvalaraSettings.AccountID, _settings.AvalaraSettings.LicenseKey)))
                .AddSingleton<IEasyPostShippingService>(x => new EasyPostShippingService(new EasyPostConfig() { APIKey = _settings.EasyPostSettings.APIKey }))
                .AddSingleton<ISmartyStreetsService>(x => new SmartyStreetsService(_settings.SmartyStreetSettings, smartyStreetsUsClient))
                .AddSingleton<IOrderCloudIntegrationsCardConnectService>(x => new OrderCloudIntegrationsCardConnectService(_settings.CardConnectSettings, flurlClientFactory))
                .AddSingleton<IOrderCloudClient>(provider => new OrderCloudClient(new OrderCloudClientConfig
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

            var serviceProvider = services.BuildServiceProvider();
            services
                .AddAuthenticationScheme<DevCenterUserAuthOptions, DevCenterUserAuthHandler>("DevCenterUser")
                .AddAuthenticationScheme<OrderCloudIntegrationsAuthOptions, OrderCloudIntegrationsAuthHandler>("OrderCloudIntegrations", opts => opts.OrderCloudClient = serviceProvider.GetService<IOrderCloudClient>())
                .AddAuthenticationScheme<OrderCloudWebhookAuthOptions, OrderCloudWebhookAuthHandler>("OrderCloudWebhook", opts => opts.HashKey = _settings.OrderCloudSettings.WebhookHashKey)
                .AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions {
                    EnableAdaptiveSampling = false, // retain all data
                    InstrumentationKey = _settings.ApplicationInsightsSettings.InstrumentationKey
                });


            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            FlurlHttp.Configure(settings => settings.Timeout = TimeSpan.FromSeconds(_settings.FlurlSettings.TimeoutInSeconds));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.OrderCloudIntegrationsConfigureWebApp(env, "v1")
                .UseSwaggerUI(c => {
                    c.SwaggerEndpoint($"/swagger", $"API v1");
                    c.RoutePrefix = string.Empty;
                });
        }
    }
}
