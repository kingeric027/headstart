using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Marketplace.Common;
using Marketplace.Common.Extensions;
using Marketplace.Orchestration;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Marketplace.Orchestration
{
    public class Startup : FunctionsStartup
    {
        public Startup() { }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.AddConfiguration((configBuilder) =>
            {
                var configuration = configBuilder
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    //.TryAddAzureKeyVault(Environment.GetEnvironmentVariable("VAULT_NAME"))
                    .AddEnvironmentVariables()
                    .Build();
                
                return configuration;
            });

            IoC.ConfigureFunctionServices(builder.Services, builder.GetCurrentConfiguration());
        }
    }
}
