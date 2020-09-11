using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ordercloud.integrations.library;
using ErrorCodes = Marketplace.Models.ErrorCodes;

namespace Marketplace.Common.Controllers
{
    [Route("swagger")]
    public class SwaggerController : BaseController
    {
        private readonly AppSettings _settings;

        public SwaggerController(AppSettings settings) : base(settings)
        {
            _settings = settings;
        }

        [HttpGet]
        public async Task<JObject> Get()
        {
            var reference = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var g = new OpenApiGenerator<BaseController, OrderCloudIntegrationsAuthAttribute>()
                .CollectMetaData(Path.Combine(reference, "reference.md"), ErrorCodes.All)
                .DefineSpec(new SwaggerConfig()
                {
                    Name = "Marketplace",
                    ContactEmail = "oheywood@four51.com",
                    Description = "Marketplace API",
                    Host = _settings.EnvironmentSettings.BaseUrl,
                    Title = "Marketplace API",
                    Url = "https://ordercloud.io",
                    Version = "1.0"
                });
            return await Task.FromResult(g.Specification());
        }
    }
}
