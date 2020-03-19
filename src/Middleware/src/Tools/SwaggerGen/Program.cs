﻿using System.IO;
using Marketplace.Common.Controllers;
using Marketplace.Helpers.Models;
using Marketplace.Helpers.OpenApiTools;
using Marketplace.Models;
using Newtonsoft.Json;

namespace SwaggerGen
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var path = GetSolutionFolder("docs");

            var g = new OpenApiGeneratorX<BaseController, MarketplaceUserAuthAttribute, IMarketplaceObject>()
                .CollectMetaData(Path.Combine(path, "reference.md"), ErrorCodes.All)
                .DefineSpec(new SwaggerConfig() {
                    Name = "Marketplace",
                    ContactEmail = "oheywood@four51.com",
                    Description = "Marketplace API",
                    Host = "https://marketplace-api-qa.azurewebsites.net",
                    Title = "Marketplace API",
                    Url = "https://ordercloud.io",
                    Version = "1.0"
                });

            using var writer = File.CreateText(Path.Combine(path, "swagger.json"));
            new JsonSerializer { Formatting = Formatting.Indented }.Serialize(writer, g.Specification());
        }

        private static string GetSolutionFolder(string folder)
        {
            var parent = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.Parent?.Parent?.Parent?.Parent?.FullName;
            
            return Path.Combine(parent, folder);
        }
    }
}