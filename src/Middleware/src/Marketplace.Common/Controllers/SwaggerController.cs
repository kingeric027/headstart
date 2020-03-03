using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Marketplace.Helpers.Models;
using Marketplace.Helpers.SwaggerTools;
using Marketplace.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Marketplace.Common.Controllers
{
    [Route("swagger")]
    public class SwaggerController : BaseController
    {
        public SwaggerController(AppSettings settings) : base(settings)
        {
        }

        [HttpGet]
        public async Task<JObject> Get()
        {
            var reference = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var swagger = SwaggerGenerator.GenerateSwaggerSpec<BaseController, MarketplaceUserAuthAttribute, IMarketplaceObject>(
                Path.Combine(reference, "reference.md"), new SwaggerConfig()
                {
                    Name = "Marketplace",
                    ContactEmail = "oheywood@four51.com",
                    Description = "Marketplace API",
                    Host = "https://marketplace-api-qa.azurewebsites.net",
                    Title = "Marketplace API",
                    Url = "https://ordercloud.io",
                    Version = "1.0"
                }, ErrorCodes.All);
            return await Task.FromResult(swagger.Item1);
        }
    }
}
