using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Marketplace.Shipping
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Marketplace.Helpers.WebHostBuilder
                .CreateWebHostBuilder<Startup, AppSettings>(args)
                .Build()
                .Run();
        }

    }
}
