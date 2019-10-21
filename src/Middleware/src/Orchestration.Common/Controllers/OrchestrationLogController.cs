using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orchestration.Common.Commands;
using Orchestration.Common.Helpers;
using Orchestration.Common.Models;

namespace Orchestration.Common.Controllers
{
    [Route("logs")]
    public class OrchestrationLogController : BaseController
    {
        private readonly IOrchestrationLogCommand _command;
        public OrchestrationLogController(IAppSettings settings, IOrchestrationLogCommand command) : base(settings)
        {
            _command = command;
        }

        [HttpGet, Route("product")]
        public async Task<dynamic> GetProductLogs(ListArgs<OrchestrationLog> listArgs)
        {
            return await _command.GetProductLogs(listArgs);
        }
    }
}
