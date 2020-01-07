using System;
using System.Threading.Tasks;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Marketplace.Helpers;
using Marketplace.Helpers.Models;

namespace Marketplace.Common.Commands
{
    public interface IOrchestrationLogCommand
    {
        Task<ListPage<OrchestrationLog>> GetProductLogs(ListArgs<OrchestrationLog> listArgs);
    }

    public class OrchestrationLogCommand : IOrchestrationLogCommand
    {
        private readonly AppSettings _settings;
        private readonly LogQuery _log;

        public OrchestrationLogCommand(AppSettings settings, LogQuery log)
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
