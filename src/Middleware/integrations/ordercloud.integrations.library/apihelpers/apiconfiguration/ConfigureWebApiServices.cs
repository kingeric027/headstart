
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Marketplace.Common.Helpers;
#if NETCOREAPP2_2
using Microsoft.AspNetCore.Authentication;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace ordercloud.integrations.library
{
    public static class OrderCloudIntegrationsConfigureWebApiServicesExtensions
    {
        public static IServiceCollection OrderCloudIntegrationsConfigureWebApiServices<T>(this IServiceCollection services, T settings, string cors_policy = null, Action<MvcJsonOptions> options = null)
            where T : class
        {
            services.AddTransient<GlobalExceptionHandler>();
            services.Inject<IOrderCloudClient>();
            services.AddSingleton(settings);
            services.AddMvc(o => { o.Filters.Add(typeof(ValidateModelAttribute)); })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
				.AddJsonOptions(o =>
				{
					o.SerializerSettings.ContractResolver = new MarketplaceSerializer();
					o.SerializerSettings.Converters.Add(new StringEnumConverter());
				})
                .AddNullableJsonOptions(options);

            services.AddCors(o => o.AddPolicy(cors_policy ?? Environment.GetEnvironmentVariable("CORS_POLICY"),
                builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));
            return services;
        }

        private static IMvcBuilder AddNullableJsonOptions(this IMvcBuilder builder, Action<MvcJsonOptions> options = null)
        {
            return options == null ? builder : builder.AddJsonOptions(options);
        }

        public static IServiceCollection AddAuthenticationScheme<TAuthOptions, TAuthHandler>(this IServiceCollection services, string name)
            where TAuthOptions : AuthenticationSchemeOptions, new() where TAuthHandler : AuthenticationHandler<TAuthOptions>
        {
            services.AddAuthentication().AddScheme<TAuthOptions, TAuthHandler>(name, null);
            return services;
        }

        public static IServiceCollection AddAuthenticationScheme<TAuthOptions, TAuthHandler>(this IServiceCollection services, string name, Action<TAuthOptions> configureOptions)
            where TAuthOptions : AuthenticationSchemeOptions, new() where TAuthHandler : AuthenticationHandler<TAuthOptions>
        {
            services.AddAuthentication().AddScheme<TAuthOptions, TAuthHandler>(name, null, configureOptions);
            return services;
        }
    }
}
#endif
