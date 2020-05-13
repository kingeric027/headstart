using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Marketplace.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configured Swashbuckle swagger html file output. Isolated to this project.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="version"></param>
        /// <param name="docsTitle"></param>
        /// <returns></returns>
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
