using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Marketplace.Common;
using Marketplace.Common.Commands;
using Marketplace.Common.Services;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Common.Services.ShippingIntegration;
using Marketplace.Helpers;
using Marketplace.Helpers.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Shipping
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        private readonly AppSettings _settings;

        public Startup(AppSettings settings)
        {
            _settings = settings;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .ConfigureWebApiServices(_settings, "v1", "Marketplace API")
                .Inject<IFlurlClient>()
                .Inject<IOCShippingIntegration>()
                .Inject<IFreightPopService>()
                .Inject<IProposedShipmentCommand>()
                .AddAuthenticationScheme<MarketplaceUserAuthOptions, MarketplaceUserAuthHandler>("MarketplaceUser");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.ConfigureWebApp(env, "v1");
        }
    }
}
