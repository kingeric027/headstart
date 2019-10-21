using System;
using Microsoft.AspNetCore.Hosting;
using Orchestration.Common;

namespace Orchestration.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IoC.CreateWebHostBuilder<Startup>(args).Build().Run();
        }
    }
}
