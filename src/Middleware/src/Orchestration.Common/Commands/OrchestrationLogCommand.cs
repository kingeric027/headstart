﻿using System;
using System.Threading.Tasks;
using Orchestration.Common.Helpers;
using Orchestration.Common.Models;
using Orchestration.Common.Queries;

namespace Orchestration.Common.Commands
{
    public interface IOrchestrationLogCommand
    {
        Task<ListPage<OrchestrationLog>> GetProductLogs(ListArgs<OrchestrationLog> listArgs);
    }

    public class OrchestrationLogCommand : IOrchestrationLogCommand
    {
        private readonly IAppSettings _settings;
        private readonly LogQuery _log;

        public OrchestrationLogCommand(IAppSettings settings, LogQuery log)
        {
            _settings = settings;
            _log = log;
        }

        public async Task<ListPage<OrchestrationLog>> GetProductLogs(ListArgs<OrchestrationLog> listArgs)
        {
            var logs = await _log.List(listArgs);
            return logs;
        }
    }
}
