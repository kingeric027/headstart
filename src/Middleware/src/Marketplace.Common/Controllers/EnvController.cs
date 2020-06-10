using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Marketplace.Common.Commands.Zoho;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Common.Controllers
{
    [Route("env")]
    public class EnvController : BaseController
    {
        private readonly IZohoCommand _zoho;
        public EnvController(AppSettings settings, IZohoCommand zoho) : base(settings)
        {
            _zoho = zoho;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            var publicSettings = new { env = Settings.Env.ToString(), cosmosdb = Settings.CosmosSettings.DatabaseName };
            return await Task.FromResult(publicSettings);
        }

        [HttpGet, Route("zoho")]
        public async Task<dynamic> GetZoho()
        {
            return await _zoho.ListOrganizations();
        }
    }
}
