using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosmonaut;
using Cosmonaut.Extensions;
using Cosmonaut.Response;
using Marketplace.Common.Extensions;
using Marketplace.Common.Models;
using Marketplace.Helpers;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Models;

namespace Marketplace.Common.Queries
{
    public class LogQuery : ICosmosQuery<OrchestrationLog>
    {
        private readonly ICosmosStore<OrchestrationLog> _store;

        public LogQuery(ICosmosStore<OrchestrationLog> store)
        {
            _store = store;
        }

        public async Task<MarketplaceListPage<OrchestrationLog>> List(IMarketplaceListArgs args)
        {
            var query = _store.Query()
                .Search(args)
                .Filter(args)
                .Sort(args);
            var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
            var count = await query.CountAsync();
            return list.ToListPage(args.Page, args.PageSize, count);
        }

        public async Task<OrchestrationLog> Get(string id)
        {
            var item = await _store.FindAsync(id);
            return item;
        }

        public async Task<OrchestrationLog> Save(OrchestrationLog log)
        {
            log.timeStamp = DateTime.Now;
            var result = await _store.UpsertAsync(log);
            return result.Entity;
        }

        public async Task<List<OrchestrationLog>> SaveMany(List<OrchestrationLog> logs)
        {
            var result = await _store.UpsertRangeAsync(logs);
            return result.SuccessfulEntities.Select(e => e.Entity).ToList();
        }

        public async Task Delete(string id)
        {
            await _store.RemoveByIdAsync(id);
        }
    }
}
