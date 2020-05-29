#if NETCOREAPP2_2
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace ordercloud.integrations.library
{
    public static class OrderCloudIntegrationsConfigureWebExtensions
    {
        public static IApplicationBuilder OrderCloudIntegrationsConfigureWebApp(this IApplicationBuilder app, IHostingEnvironment env, string v1)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<GlobalExceptionHandler>();
            app.UseHttpsRedirection();
            app.UseMvc();
            return app;
        }
    }
}
#endif