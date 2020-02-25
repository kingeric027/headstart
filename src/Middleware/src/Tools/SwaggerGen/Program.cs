using System;
using System.IO;
using Marketplace.Common.Controllers;
using Marketplace.Common.Helpers.Tools;
using Marketplace.Models;
using Newtonsoft.Json;

namespace SwaggerGen
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //SwaggerGenerator.Write(@"..\..\..\..\..\..\resources\swagger.json");
           
            var path = GetSolutionFolder("docs");
            var swagger = SwaggerGenerator.GenerateSwaggerSpec<BaseController, MarketplaceUserAuthAttribute>(Path.Combine(path, "reference.md"), "v1");
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
