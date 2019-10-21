using System;
using System.IO;
using Cosmonaut;
using Cosmonaut.Extensions.Microsoft.DependencyInjection;
using Flurl.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orchestration.Common.Commands;
using Orchestration.Common.Extensions;
using Orchestration.Common.Helpers;
using Orchestration.Common.Models;
using Orchestration.Common.Queries;
using Orchestration.Common.Services;
using OrderCloud.SDK;

namespace Orchestration.Common
{
    public static class IoC
    {
        public static IWebHostBuilder CreateWebHostBuilder<TStartup>(string[] args) where TStartup : class =>
            WebHost.CreateDefaultBuilder(args)
                .UseDefaultServiceProvider(options =>
                    options.ValidateScopes = false)
                .UseAppSettings<AppSettings>()
                .UseStartup<TStartup>();

        public static IServiceCollection SharedServices(this IServiceCollection services)
        {
            services.AddTransient<IBlobService, BlobService>();
            services.AddTransient<IOrchestrationCommand, OrchestrationCommand>();
            services.AddTransient<IFlurlClient, FlurlClient>();
            services.AddTransient<ISyncCommand, SyncCommand>();
            services.AddSingleton<IOrderCloudClient, OrderCloudClient>();
            return services;
        }

        public static void ConfigureFunctionServices(IServiceCollection services, IConfiguration configuration)
        {
            var settings = new AppSettings();
            configuration.GetSection("AppSettings").Bind(settings);
            services.AddSingleton<IAppSettings>(settings);

            services.SharedServices();

            var cosmosSettings = new CosmosStoreSettings(settings.CosmosSettings.DatabaseName, settings.CosmosSettings.EndpointUri,
                settings.CosmosSettings.PrimaryKey, new ConnectionPolicy
                {
                    ConnectionProtocol = Protocol.Tcp,
                    ConnectionMode = ConnectionMode.Direct
                }, defaultCollectionThroughput: 5000);

            services.AddSingleton(typeof(LogQuery), typeof(LogQuery));
            services.AddCosmosStore<OrchestrationLog>(cosmosSettings);
            services.AddSingleton(typeof(SupplierQuery), typeof(SupplierQuery));
            services.AddCosmosStore<OrchestrationSupplier>(cosmosSettings);
        }

        public static ServiceProvider RegisterApiServices(this IServiceCollection services, IAppSettings settings)
        {
            services.AddTransient<GlobalExceptionHandler>();
            services.AddSingleton(settings);

            services.AddTransient<IOrchestrationLogCommand, OrchestrationLogCommand>();
            services.SharedServices();

            var cosmosSettings = new CosmosStoreSettings(settings.CosmosSettings.DatabaseName, settings.CosmosSettings.EndpointUri,
                settings.CosmosSettings.PrimaryKey, new ConnectionPolicy
                {
                    ConnectionProtocol = Protocol.Tcp,
                    ConnectionMode = ConnectionMode.Direct
                }, defaultCollectionThroughput: 5000);

            services.AddSingleton(typeof(LogQuery), typeof(LogQuery));
            services.AddCosmosStore<OrchestrationLog>(cosmosSettings);
            services.AddSingleton(typeof(SupplierQuery), typeof(SupplierQuery));
            services.AddCosmosStore<OrchestrationSupplier>(cosmosSettings);

            return services.BuildServiceProvider();
        }
    }
   
}
