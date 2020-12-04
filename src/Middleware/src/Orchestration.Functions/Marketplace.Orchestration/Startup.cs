using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Marketplace.Common;
using Marketplace.Common.Commands;
using Marketplace.Common.Helpers;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Marketplace.Orchestration;
using Flurl.Http;
using Microsoft.Extensions.DependencyInjection;
using ordercloud.integrations.cms;
using OrderCloud.SDK;
using ordercloud.integrations.library;
using Marketplace.Common.Services;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Marketplace.Orchestration
{
    public class Startup : FunctionsStartup
    {
        public Startup() { }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var connectionString = Environment.GetEnvironmentVariable("APP_CONFIG_CONNECTION");
            var settings = builder
                .InjectAzureFunctionSettings<AppSettings>(connectionString)
                .BindSettings<AppSettings>();

            var cosmosConfig = new CosmosConfig(settings.CosmosSettings.DatabaseName,
                settings.CosmosSettings.EndpointUri, settings.CosmosSettings.PrimaryKey);
            var cmsConfig = new CMSConfig()
            {
                BaseUrl = settings.EnvironmentSettings.BaseUrl,
                BlobStorageHostUrl = settings.BlobSettings.HostUrl,
            };
            builder.Services
                .InjectCosmosStore<AssetQuery, AssetDO>(cosmosConfig)
                .InjectCosmosStore<AssetedResourceQuery, AssetedResourceDO>(cosmosConfig)
                .InjectCosmosStore<LogQuery, OrchestrationLog>(cosmosConfig)
                .InjectCosmosStore<AssetContainerQuery, AssetContainerDO>(cosmosConfig)
                .InjectCosmosStore<AssetedResourceQuery, AssetedResourceDO>(cosmosConfig)
                .InjectCosmosStore<ResourceHistoryQuery<ProductHistory>, ProductHistory>(cosmosConfig)
                .InjectCosmosStore<ResourceHistoryQuery<PriceScheduleHistory>, PriceScheduleHistory>(cosmosConfig)
                .Inject<IOrderCloudIntegrationsFunctionToken>()
                .Inject<IFlurlClient>()
                .InjectOrderCloud<IOrderCloudClient>(new OrderCloudClientConfig()
                {
                    ApiUrl = settings.OrderCloudSettings.ApiUrl,
                    AuthUrl = settings.OrderCloudSettings.ApiUrl,
                    ClientId = settings.OrderCloudSettings.ClientID,
                    ClientSecret = settings.OrderCloudSettings.ClientSecret,
                    Roles = new[]
                    {
                        ApiRole.FullAccess
                    }
                })
                .AddSingleton<CMSConfig>(x => cmsConfig)
                .Inject<IAssetQuery>()
                .Inject<IAssetedResourceQuery>()
                .Inject<IOrchestrationCommand>()
                .Inject<ISupplierSyncCommand>()
                .Inject<ISyncCommand>()
                .Inject<IProductTemplateCommand>()
                .Inject<ISendgridService>();
        }
    }
}
