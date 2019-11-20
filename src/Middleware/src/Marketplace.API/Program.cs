using System;
using Microsoft.AspNetCore.Hosting;
using Marketplace.Common;

namespace Marketplace.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IoC.CreateWebHostBuilder<Startup>(args).Build().Run();
        }
    }
}
