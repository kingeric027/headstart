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
	}
}
