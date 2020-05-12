
using Microsoft.AspNetCore.Authentication;
#if NETCOREAPP2_2
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Marketplace.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureOpenApiSpec(this IServiceCollection services, string version, string docsTitle)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(version, new Info { Title = docsTitle, Version = version });
                c.CustomSchemaIds(x => x.FullName);
            });
            return services;
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
    }
}
#endif