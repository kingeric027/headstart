using System.IO;
using System.Threading.Tasks;
using Marketplace.Helpers.Models;
using Marketplace.Helpers.OpenApiTools;
using Marketplace.Models;
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
            var g = new OpenApiGenerator<BaseController, MarketplaceUserAuthAttribute, IMarketplaceObject>()
                .CollectMetaData(Path.Combine(reference, "reference.md"), ErrorCodes.All)
                .DefineSpec(new SwaggerConfig()
                {
                    Name = "Marketplace",
                    ContactEmail = "oheywood@four51.com",
                    Description = "Marketplace API",
                    Host = "https://marketplace-api-qa.azurewebsites.net",
                    Title = "Marketplace API",
                    Url = "https://ordercloud.io",
                    Version = "1.0"
                });
            return await Task.FromResult(g.Specification());
        }
    }
}
