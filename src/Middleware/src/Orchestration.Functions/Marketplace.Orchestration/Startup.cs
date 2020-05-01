using System;
using Flurl.Http;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Marketplace.Common;
using Marketplace.Common.Commands;
using Marketplace.Common.Extensions;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Models;
using Marketplace.Orchestration;
using OrderCloud.SDK;
using Marketplace.Common.Services.FreightPop;

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
                .Inject<IFlurlClient>()
                .Inject<IFreightPopService>()
                .Inject<IOrderCloudClient>()
                .Inject<IOrchestrationCommand>()
                .Inject<IOrderOrchestrationCommand>()
                .Inject<IMarketplaceBuyerLocationCommand>()
                .Inject<ISyncCommand>()
                .InjectCosmosStore<LogQuery, OrchestrationLog>(new CosmosConfig(
                        settings.CosmosSettings.DatabaseName, 
                        settings.CosmosSettings.EndpointUri, 
                        settings.CosmosSettings.PrimaryKey));
        }
    }
}
