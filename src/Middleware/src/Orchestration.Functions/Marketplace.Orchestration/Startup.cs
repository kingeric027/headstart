using System;
using Cosmonaut;
using Flurl.Http;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Marketplace.Common;
using Marketplace.Common.Commands;
using Marketplace.Common.Extensions;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Marketplace.Common.Services.DevCenter;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Models;
using Marketplace.Orchestration;
using OrderCloud.SDK;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Marketplace.Orchestration
{
    public class Startup : FunctionsStartup
    {
        public Startup() { }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.InjectAzureFunctionSettings<AppSettings>();

            // needs improvement through helper library
            var settings = new AppSettings();
            builder.GetCurrentConfiguration().GetSection("AppSettings").Bind(settings);
            var c = new CosmosConfig(settings.CosmosSettings.DatabaseName, settings.CosmosSettings.EndpointUri, settings.CosmosSettings.PrimaryKey);
            // end needs improvement

            builder.Services.Inject<IDevCenterService>()
                .Inject<IOrchestrationCommand>()
                .Inject<IFlurlClient>()
                .Inject<ISyncCommand>()
                .Inject<IOrderCloudClient>()
                .InjectCosmosStore<LogQuery, OrchestrationLog>(c);
        }
    }
}
