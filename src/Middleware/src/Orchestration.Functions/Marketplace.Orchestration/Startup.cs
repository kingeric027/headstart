using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Marketplace.Common;
using Marketplace.Common.Commands;
using Marketplace.Common.Helpers;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Marketplace.Orchestration;
using OrderCloud.SDK;
using Marketplace.Common.Services.FreightPop;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ordercloud.integrations.cosmos;
using ordercloud.integrations.extensions;

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
                .Inject<IFlurlClient>()
                .Inject<IFreightPopService>()
                .Inject<IOrderCloudClient>()
                .Inject<IOrchestrationCommand>()
                .Inject<IOrderOrchestrationCommand>()
                .Inject<ISyncCommand>()
                .InjectCosmosStore<LogQuery, OrchestrationLog>(new CosmosConfig(
                        settings.CosmosSettings.DatabaseName, 
                        settings.CosmosSettings.EndpointUri, 
                        settings.CosmosSettings.PrimaryKey));
        }
    }

    
}
