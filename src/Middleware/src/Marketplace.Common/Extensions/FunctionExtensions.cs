using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Marketplace.Common.Extensions
{
    public static class FunctionsHostBuilderConfigurationsExtensions
    {
        /// <summary>
        /// Binds your appsettings.json file (or other config source, such as App Settings in the Azure portal) to the AppSettings class
        /// so values can be accessed in a strongly typed manner. Call in your Program.cs off of WebHost.CreateDefaultBuilder(args).
        /// If called before UseStartup, then AppSettings can be injected into your Startup class.
        /// </summary>
        public static IWebHostBuilder UseAppSettings<TAppSettings>(this IWebHostBuilder hostBuilder) where TAppSettings : class, new()
        {
            return hostBuilder.ConfigureServices((ctx, services) => {
                // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options
                services.Configure<TAppSettings>(ctx.Configuration);

                // Breaks from the Options pattern (link above) by allowing AppSettings to be injected directly
                // into services, rather than injecting IOptions<AppSettings>.
                services.AddTransient(sp => sp.GetService<IOptionsSnapshot<TAppSettings>>().Value);
            });
        }

        public static IFunctionsHostBuilder AddConfiguration(this IFunctionsHostBuilder builder, Func<IConfigurationBuilder, IConfiguration> configBuilderFunc)
        {
            var configurationBuilder = builder.GetBaseConfigurationBuilder();
            var configuration = configBuilderFunc(configurationBuilder);
            builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), configuration));
            return builder;
        }

        public static IConfiguration GetCurrentConfiguration(this IFunctionsHostBuilder builder)
        {
            var provider = builder.Services.BuildServiceProvider();
            return provider.GetService<IConfiguration>();
        }

        private static IConfigurationBuilder GetBaseConfigurationBuilder(this IFunctionsHostBuilder builder)
        {
            var configurationBuilder = new ConfigurationBuilder();
            var descriptor = builder.Services.FirstOrDefault(d => d.ServiceType == typeof(IConfiguration));
            if (descriptor?.ImplementationInstance is IConfiguration configRoot)
                configurationBuilder.AddConfiguration(configRoot);
            return configurationBuilder.SetBasePath(GetCurrentDirectory());
        }

        private static string GetCurrentDirectory()
        {
            var currentDirectory = "/home/site/wwwroot";
            var isLocal = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID"));
            if (isLocal)
                currentDirectory = Directory.GetCurrentDirectory();
            return currentDirectory;
        }

        public static IConfigurationBuilder TryAddAzureKeyVault(this IConfigurationBuilder builder, string vaultName)
        {
            // other checks can be added here 
            // for short term assume if null don't add

            //if (!string.IsNullOrEmpty(vaultName))
            //{
            //    var vaultUrl = $"https://{vaultName}.vault.azure.net/";
            //    builder.AddAzureKeyVault(vaultUrl);
            //}

            return builder;
        }
    }
}
