using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Common.Extensions
{
    public static class FunctionsHostBuilderConfigurationsExtensions
    {
        public static T BindSettings<T>(this IFunctionsHostBuilder builder, string section = "AppSettings") where T : class, new()
        {
            var settings = new T();
            builder.GetCurrentConfiguration()
                .GetSection(section)
                .Bind(settings);
            return settings;
        }

        public static IConfiguration GetCurrentConfiguration(this IFunctionsHostBuilder builder)
        {
            var provider = builder.Services.BuildServiceProvider();
            return provider.GetService<IConfiguration>();
        }
    }
}
