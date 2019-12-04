﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Common.Commands;
using Marketplace.Common.Helpers;
using Marketplace.Common.Models;

namespace Marketplace.Common.Controllers
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