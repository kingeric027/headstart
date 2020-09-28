using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using Cosmonaut;
using Cosmonaut.Extensions.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

#if NETCOREAPP3_1
using System.Text.Json.Serialization;
#endif

namespace ordercloud.integrations.library
{
    public static class FunctionHostBuilderExtensions
    {
#if NETCOREAPP2_2
    public static IFunctionsHostBuilder InjectAzureFunctionSettings<T>(this IFunctionsHostBuilder host,
        string appSettingsConnectionString, string section = "AppSettings") where T : class, new()
    {
        var settings = new T();
        var builder = new ConfigurationBuilder();
        if (host.Services.FirstOrDefault(d => d.ServiceType == typeof(IConfiguration))?.ImplementationInstance is
            IConfiguration configRoot)
            builder.AddConfiguration(configRoot);
        var config = builder
            .AddAzureAppConfiguration(appSettingsConnectionString)
            .Build();
        config.GetSection(section).Bind(settings);

        host.Services
            .Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), config))
            .BuildServiceProvider()
            .GetService<IConfiguration>();
        host.Services.AddMvcCore().AddJsonFormatters(f =>
        {
            f.ContractResolver = new DefaultContractResolver();
            f.Converters.Add(new StringEnumConverter());
        }).AddJsonOptions(o => o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore);
        host.Services.AddSingleton(settings);
        return host;
    }

    public static T BindSettings<T>(this IFunctionsHostBuilder builder, string section = "AppSettings")
        where T : class, new()
    {
        var settings = new T();
        builder.Services.BuildServiceProvider().GetService<IConfiguration>()
            .GetSection(section)
            .Bind(settings);
        return settings;
    }
#endif

#if NETCOREAPP3_1
        public static IFunctionsHostBuilder InjectAzureFunctionSettings<T>(this IFunctionsHostBuilder host, string section
 = null, string connection_string = null)
            where T : class, new()
        {
            var settings = new T();

            var config = new ConfigurationBuilder()
                .AddAzureAppConfiguration(connection_string ?? Environment.GetEnvironmentVariable("APP_CONFIG_CONNECTION"))
                .Build();

            config.GetSection(section ?? Environment.GetEnvironmentVariable("APP_SETTINGS_SECTION_NAME")).Bind(settings);

            host
                .Services.AddMvcCore().AddJsonOptions(opt =>
                {
					opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
					opt.JsonSerializerOptions.IgnoreNullValues = true;
                    opt.JsonSerializerOptions.PropertyNamingPolicy = null;
                })
                .Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), config))
                .BuildServiceProvider()
                .GetService<IConfiguration>();

            host.Services.AddSingleton(settings);
            return host;
        }

        public static IFunctionsHostBuilder InjectDependencies(this IFunctionsHostBuilder builder, IEnumerable<Type> services)
        {
            foreach (var service in services)
            {
                builder.Services.Inject(service);
            }

            return builder;
        }

        public static IHostBuilder InjectCosmosStores<TQuery, TModel>(this IHostBuilder builder, CosmosConfig config) where TQuery : class where TModel : class
        {
            return builder.ConfigureServices((context, collection) =>
            {
                var cs = new CosmosStoreSettings(config.DatabaseName, config.EndpointUri, config.PrimaryKey, new ConnectionPolicy
                {
                    ConnectionProtocol = Protocol.Tcp,
                    ConnectionMode = ConnectionMode.Direct
                }, defaultCollectionThroughput: 400)
                {
                    UniqueKeyPolicy = new UniqueKeyPolicy()
                    {
                        UniqueKeys = (Collection<UniqueKey>)typeof(TModel).GetMethod("GetUniqueKeys")?.Invoke(null, null) ?? new Collection<UniqueKey>()
                    }
                };
                collection.AddSingleton(typeof(TQuery), typeof(TQuery));
                collection.AddCosmosStore<TModel>(cs);
            });
        }
#endif
    }
}
