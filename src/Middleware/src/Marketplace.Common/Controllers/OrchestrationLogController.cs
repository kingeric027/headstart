using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Common.Commands;
using Marketplace.Common.Models;
using Marketplace.Helpers;

namespace Marketplace.Common.Controllers
{
    [Route("logs")]
    public class OrchestrationLogController : BaseController
    {
        private readonly IOrchestrationLogCommand _command;
        public OrchestrationLogController(AppSettings settings, IOrchestrationLogCommand command) : base(settings)
        {
            _command = command;
        }

        [HttpGet, Route("product")]
        public async Task<dynamic> GetProductLogs(MarketplaceListArgs<OrchestrationLog> marketplaceListArgs)
        {
            return await _command.GetProductLogs(marketplaceListArgs);
        }
    }
}
