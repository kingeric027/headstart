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
        Task<MarketplaceListPage<OrchestrationLog>> GetProductLogs(MarketplaceListArgs<OrchestrationLog> marketplaceListArgs);
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

        public async Task<MarketplaceListPage<OrchestrationLog>> GetProductLogs(MarketplaceListArgs<OrchestrationLog> marketplaceListArgs)
        {
            var logs = await _log.List(marketplaceListArgs);
            return logs;
        }
    }
}
