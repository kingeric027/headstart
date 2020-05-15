﻿using System;
using Avalara.AvaTax.RestClient;
using Flurl.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Integrations.Avalara;
using Marketplace.CMS.Models;
using Marketplace.Common;
using Marketplace.Common.Commands;
using Marketplace.Common.Commands.Crud;
using Marketplace.Common.Commands.Zoho;
using Marketplace.Common.Extensions;
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
using Marketplace.Models.Extended;
using ordercloud.integrations.cardconnect;
using ordercloud.integrations.CMS;
using ordercloud.integrations.CMS.Models;
using ordercloud.integrations.CMS.Queries;
using ordercloud.integrations.CMS.Storage;
using ordercloud.integrations.cosmos;
using OrderCloud.SDK;
using ordercloud.integrations.extensions;
using Swashbuckle.AspNetCore.Swagger;

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
                .OrderCloudIntegrationsConfigureWebApiServices(_settings, "marketplacecors")
                .InjectCosmosStore<LogQuery, OrchestrationLog>(cosmosConfig)
                .InjectCosmosStore<SupplierCategoryConfigQuery, SupplierCategoryConfig>(cosmosConfig)
                .InjectCosmosStore<AssetQuery, Asset>(cosmosConfig)
                .InjectCosmosStore<AssetContainerQuery, AssetContainer>(cosmosConfig)
                .InjectCosmosStore<AssetedResourceQuery, AssetedResource>(cosmosConfig).Inject<AppSettings>()
                .Inject<IDevCenterService>()
				.Inject<IFlurlClient>()
				.Inject<IZohoClient>()
				.Inject<IZohoCommand>()
				.Inject<ISyncCommand>()
				.Inject<IValidatedAddressCommand>()
				.Inject<IFreightPopService>()
				.Inject<IOrchestrationCommand>()
				.Inject<IOrchestrationLogCommand>()
				.Inject<IOCShippingIntegration>()
				.Inject<IShipmentCommand>()
                .Inject<IEnvironmentSeedCommand>()
				.Inject<IOrderCloudSandboxService>()
				.Inject<IMarketplaceProductCommand>()
				.Inject<ISendgridService>()
				.Inject<IAssetQuery>()
				.Inject<ISupplierCategoryConfigQuery>()
                .Inject<IMarketplaceSupplierCommand>()
                .Inject<IOrderCloudIntegrationsCardConnectCommand>()
                .AddSingleton<IAvalaraCommand>(x => new AvalaraCommand(avalaraConfig))
                .AddSingleton<IBlobStorage>(x => new BlobStorage(cmsConfig))
                .AddSingleton<ISmartyStreetsCommand>(x => new SmartyStreetsCommand(_settings.SmartyStreetSettings))
                .AddSingleton<IOrderCloudIntegrationsCardConnectService>(x => new OrderCloudIntegrationsCardConnectService(_settings.CardConnectSettings))
                .AddAuthenticationScheme<DevCenterUserAuthOptions, DevCenterUserAuthHandler>("DevCenterUser")
                .AddAuthenticationScheme<OrderCloudIntegrationsAuthOptions, OrderCloudIntegrationsAuthHandler>("MarketplaceUser")
                .AddAuthenticationScheme<OrderCloudWebhookAuthOptions, OrderCloudWebhookAuthHandler>("OrderCloudWebhook", opts => opts.HashKey = _settings.OrderCloudSettings.WebhookHashKey)
                .AddTransient<IOrderCloudClient>(provider => new OrderCloudClient(new OrderCloudClientConfig
                {
                    ApiUrl = _settings.OrderCloudSettings.ApiUrl,
                    AuthUrl = _settings.OrderCloudSettings.AuthUrl,
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
