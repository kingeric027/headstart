using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Marketplace.Common;
using Marketplace.Common.Commands;
using Marketplace.Common.Helpers;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Marketplace.Orchestration;
using Flurl.Http;
using Marketplace.Common.Commands.SupplierSync;
using OrderCloud.SDK;
using ordercloud.integrations.library;
using ordercloud.integrations.freightpop;

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

            builder.Services
                .Inject<IOrderCloudIntegrationsFunctionToken>()
                .Inject<IOrderCloudClient>()
                .Inject<IFlurlClient>()
                .InjectOrderCloud<IOrderCloudClient>(new OrderCloudClientConfig()
                {
                    ApiUrl = settings.OrderCloudSettings.ApiUrl
                })
                .Inject<IOrchestrationCommand>()
                .Inject<ISupplierSyncCommand>()
                .Inject<ISyncCommand>()
                .InjectCosmosStore<LogQuery, OrchestrationLog>(new CosmosConfig(
                        settings.CosmosSettings.DatabaseName, 
                        settings.CosmosSettings.EndpointUri, 
                        settings.CosmosSettings.PrimaryKey));
        }
    }
}
