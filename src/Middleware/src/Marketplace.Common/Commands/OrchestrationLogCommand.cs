using System.Threading.Tasks;
using Headstart.Common.Models;
using Headstart.Common.Queries;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Headstart.Common.Commands
{
    public interface IOrchestrationLogCommand
    {
        Task<ListPage<OrchestrationLog>> List(ListArgs<OrchestrationLog> marketplaceListArgs);
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

        public async Task<ListPage<OrchestrationLog>> List(ListArgs<OrchestrationLog> marketplaceListArgs)
        {
            var logs = await _log.List(marketplaceListArgs);
            return logs;
        }
    }
}
