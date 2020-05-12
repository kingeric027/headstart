using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Common.Commands;
using Marketplace.Common.Models;
using Marketplace.Models.Attributes;
using ordercloud.integrations.extensions;
using ordercloud.integrations.openapispec;
using OrderCloud.SDK;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Orchestration Logs\" represents logs of orchestration activities")]
    [MarketplaceSection.Orchestration(ListOrder = 3)]
    [Route("orchestration/logs")]
    public class OrchestrationLogController : BaseController
    {
        private readonly IOrchestrationLogCommand _command;
        public OrchestrationLogController(AppSettings settings, IOrchestrationLogCommand command) : base(settings)
        {
            _command = command;
        }

        [DocName("GET Orchestration Logs")]
        [HttpGet]
        public async Task<ListPage<OrchestrationLog>> List(ListArgs<OrchestrationLog> marketplaceListArgs)
        {
            return await _command.List(marketplaceListArgs);
        }
    }
}
