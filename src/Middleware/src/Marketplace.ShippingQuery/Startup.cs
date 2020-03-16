using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Marketplace.Common;
using Marketplace.Common.Extensions;
using Marketplace.Helpers.Extensions;
using Marketplace.Orchestration;
using Microsoft.Extensions.DependencyInjection;
using Marketplace.Common.Services.ShippingIntegration;
using OrderCloud.SDK;
using System;
using Marketplace.Common.Services.FreightPop;
using Flurl.Http;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Marketplace.Orchestration
{
    public class Startup : FunctionsStartup
    {
        public Startup() { }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.InjectAzureFunctionSettings<AppSettings>();

            // needs improvement through helper library
            var settings = new AppSettings();
            builder.GetCurrentConfiguration().GetSection("AppSettings").Bind(settings);
            // end needs improvement

            builder.Services
                .Inject<IFlurlClient>()
                .Inject<IFreightPopService>()
                .Inject<IOrderCloudClient>()
                .Inject<IShipmentQuery>();
        }
    }
}