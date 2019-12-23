using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Common.Controllers
{
    [Route("env")]
    public class EnvController : BaseController
    {
        public EnvController(AppSettings settings) : base(settings)
        {
        }

        [HttpGet]
        public async Task<object> Get()
        {
            var publicSettings = new { env = Settings.Env.ToString(), cosmosdb = Settings.CosmosSettings.DatabaseName };
            return await Task.FromResult(publicSettings);
        }
    }
}
