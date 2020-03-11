using System;
using System.IO;
using System.Linq;
using Cosmonaut;
using Cosmonaut.Extensions.Microsoft.DependencyInjection;
using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json.Serialization;
using OrderCloud.SDK;
using Swashbuckle.AspNetCore.Swagger;

namespace Marketplace.Helpers.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Inject<T>(this IServiceCollection services)
        {
            return services.AddServicesByConvention(typeof(T).Assembly, typeof(T).Namespace);
        }

        public static IServiceCollection InjectOrderCloud<T>(this IServiceCollection services, OrderCloudClientConfig config)
        {
            services.AddTransient<IOrderCloudClient>(provider => new OrderCloudClient(config));
            return services;
        }

        public static IServiceCollection InjectCosmosStore<TQuery, TModel>(this IServiceCollection services, CosmosConfig config) 
            where TQuery : class 
            where TModel : class
        {
            var settings = new CosmosStoreSettings(config.DatabaseName, config.EndpointUri, config.PrimaryKey, new ConnectionPolicy
                {
                    ConnectionProtocol = Protocol.Tcp,
                    ConnectionMode = ConnectionMode.Direct
                }, defaultCollectionThroughput: 400);
            services.AddSingleton(typeof(TQuery), typeof(TQuery));
            return services.AddCosmosStore<TModel>(settings);
        }

        public static IServiceCollection ConfigureWebApiServices<T>(this IServiceCollection services, T settings, string version, string docsTitle) 
            where T : class
        {
            services.AddTransient<GlobalExceptionHandler>();
            services.Inject<IOrderCloudClient>();
            services.AddSingleton(settings);
            services.AddMvc(options => { options.Filters.Add(typeof(ValidateModelAttribute)); })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(o =>
                {
                    o.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(version, new Info { Title = docsTitle, Version = version });
                c.CustomSchemaIds(x => x.FullName);
            });

            services.AddCors(o => o.AddPolicy("marketplacecors",
                builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));
            return services;
        }

        public static IApplicationBuilder ConfigureWebApp(this IApplicationBuilder app, IHostingEnvironment env, string version)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"API {version}");
				c.RoutePrefix = string.Empty;
			});

			app.UseMiddleware<GlobalExceptionHandler>();
            app.UseHttpsRedirection();
            app.UseMvc();
            return app;
        }

        public static IServiceCollection AddAuthenticationScheme<TAuthOptions, TAuthHandler>(this IServiceCollection services, string name, Action<TAuthOptions> configureOptions)
            where TAuthOptions : AuthenticationSchemeOptions, new() where TAuthHandler : AuthenticationHandler<TAuthOptions>
        {
            services.AddAuthentication().AddScheme<TAuthOptions, TAuthHandler>(name, null, configureOptions);
            return services;
        }

        public static IServiceCollection AddAuthenticationScheme<TAuthOptions, TAuthHandler>(this IServiceCollection services, string name) 
            where TAuthOptions : AuthenticationSchemeOptions, new() where TAuthHandler : AuthenticationHandler<TAuthOptions>
        {
            services.AddAuthentication().AddScheme<TAuthOptions, TAuthHandler>(name, null);
            return services;
        }

        public static IServiceCollection InjectAzureFunctionSettings<T>(this IFunctionsHostBuilder host, string section = "AppSettings") where T : class, new()
        {
            var settings = new T();
            var builder = new ConfigurationBuilder();

            if (host.Services.FirstOrDefault(d => d.ServiceType == typeof(IConfiguration))?.ImplementationInstance is IConfiguration configRoot)
                builder.AddConfiguration(configRoot).SetBasePath(string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID"))
                    ? Directory.GetCurrentDirectory()
                    : "/home/site/wwwroot");

            var config = builder
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            
            host.Services
                .Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), config))
                .BuildServiceProvider()
                .GetService<IConfiguration>()
                .GetSection(section)
                .Bind(settings);
            return host.Services.AddSingleton(settings);
        }

        public static IServiceCollection InjectConsoleAppSettings<T>(this IServiceCollection services, string folder, string file) where T : class
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(folder)
                .AddEnvironmentVariables()
                .AddJsonFile(file, true).Build();
            var settings = builder.Get<T>();
            services.AddSingleton(settings);
            return services;
        }

        
    }
}
