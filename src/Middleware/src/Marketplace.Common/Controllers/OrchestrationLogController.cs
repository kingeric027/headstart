using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Common.Commands;
using Marketplace.Common.Models;
using Marketplace.Helpers;
using OrderCloud.SDK;

namespace Marketplace.Common.Controllers
{
    [Route("orchestration/logs")]
    public class OrchestrationLogController : BaseController
    {
        private readonly IOrchestrationLogCommand _command;
        public OrchestrationLogController(AppSettings settings, IOrchestrationLogCommand command) : base(settings)
        {
            _command = command;
        }

        [HttpGet, Route("")]
        public async Task<ListPage<OrchestrationLog>> List(ListArgs<OrchestrationLog> args)
        {
            return await _command.List(args);
        }
    }
}
