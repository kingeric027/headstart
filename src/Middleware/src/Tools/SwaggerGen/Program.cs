using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Humanizer;
using Marketplace.Common.Controllers;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.SwaggerTools;
using Marketplace.Models;
using Newtonsoft.Json;

namespace SwaggerGen
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //SwaggerGenerator.Write(@"..\..\..\..\..\..\resources\swagger.json");

            //var a = Assembly.GetAssembly(typeof(Marketplace.Models.IMarketplaceObject));
            //var controller = Assembly.GetAssembly(typeof(BaseController)).GetExportedTypes();

            var path = GetSolutionFolder("docs");
            var swagger = SwaggerGenerator.GenerateSwaggerSpec<BaseController, MarketplaceUserAuthAttribute, IMarketplaceObject>(Path.Combine(path, "reference.md"), "v1", ErrorCodes.All);
            using var writer = File.CreateText(Path.Combine(path, "swagger.json"));
            new JsonSerializer { Formatting = Formatting.Indented }.Serialize(writer, swagger);
        }

        private static string GetSolutionFolder(string folder)
        {
            var parent = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.Parent?.Parent?.Parent?.Parent?.FullName;
            
            return Path.Combine(parent, folder);
        }
    }
}
