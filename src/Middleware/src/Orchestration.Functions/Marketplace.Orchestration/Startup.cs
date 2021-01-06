using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Headstart.Common;
using Headstart.Common.Commands;
using Headstart.Common.Helpers;
using Headstart.Common.Models;
using Headstart.Common.Queries;
using Headstart.Orchestration;
using Flurl.Http;
using Microsoft.Extensions.DependencyInjection;
using OrderCloud.SDK;
using ordercloud.integrations.library;
using Headstart.Common.Services;
using Flurl.Http.Configuration;
using Headstart.Common.Services.CMS;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Headstart.Orchestration
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

            var cosmosConfig = new CosmosConfig(
                settings.CosmosSettings.DatabaseName,
                settings.CosmosSettings.EndpointUri,
                settings.CosmosSettings.PrimaryKey,
                settings.CosmosSettings.RequestTimeoutInSeconds,
                settings.CosmosSettings.MaxConnectionLimit,
                settings.CosmosSettings.IdleTcpConnectionTimeoutInMinutes,
                settings.CosmosSettings.OpenTcpConnectionTimeoutInSeconds,
                settings.CosmosSettings.MaxTcpConnectionsPerEndpoint,
                settings.CosmosSettings.MaxRequestsPerTcpConnection,
                settings.CosmosSettings.EnableTcpConnectionEndpointRediscovery
            );
            builder.Services
                .AddLazyCache()
                .InjectCosmosStore<LogQuery, OrchestrationLog>(cosmosConfig)
                .InjectCosmosStore<ResourceHistoryQuery<ProductHistory>, ProductHistory>(cosmosConfig)
                .InjectCosmosStore<ResourceHistoryQuery<PriceScheduleHistory>, PriceScheduleHistory>(cosmosConfig)
                .Inject<IOrderCloudIntegrationsFunctionToken>()
                .AddSingleton<ICMSClient>(new CMSClient(new CMSClientConfig() { BaseUrl = settings.CMSSettings.BaseUrl }))
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
                .AddSingleton<IFlurlClientFactory, PerBaseUrlFlurlClientFactory>()
                .Inject<IOrchestrationCommand>()
                .Inject<ISupplierSyncCommand>()
                .Inject<ISyncCommand>()
                .Inject<IProductTemplateCommand>()
                .Inject<ISendgridService>();
        }
    }
}
