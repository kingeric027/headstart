using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Marketplace.Common;
using Marketplace.Helpers;
using Marketplace.Helpers.Attributes;

namespace Marketplace.API
{
    public class Startup
    {
        private IConfiguration _config;
        private readonly AppSettings _settings;
        public Startup(IConfiguration configuration, AppSettings settings)
        {
            _config = configuration;
            _settings = settings;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.RegisterApiServices(_settings);
            services.AddMvc(options => { options.Filters.Add(typeof(ValidateModelAttribute)); })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(o =>
                {
                    o.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    o.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });
            
            services.AddAuthentication()
                .AddScheme<MarketplaceUserAuthOptions, MarketplaceUserAuthHandler>("MarketplaceUser", null);

            services.AddCors(o => o.AddPolicy("marketplacecors",
                builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            app.UseMiddleware<GlobalExceptionHandler>();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
