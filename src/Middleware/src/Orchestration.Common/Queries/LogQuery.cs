using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cosmonaut;
using Cosmonaut.Extensions;
using Cosmonaut.Response;
using Orchestration.Common.Extensions;
using Orchestration.Common.Helpers;
using Orchestration.Common.Models;

namespace Orchestration.Common.Queries
{
    public class LogQuery
    {
        private readonly ICosmosStore<OrchestrationLog> _store;

        public LogQuery(ICosmosStore<OrchestrationLog> store)
        {
            _store = store;
        }

        public async Task<Models.ListPage<OrchestrationLog>> List(IListArgs args)
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

        public async Task<CosmosResponse<OrchestrationLog>> Upsert(OrchestrationLog log)
        {
            log.timeStamp = DateTime.Now;
            var result = await _store.UpsertAsync(log);
            return result;
        }

        public async Task<CosmosMultipleResponse<OrchestrationLog>> UpsertList(List<OrchestrationLog> logs)
        {
            var result = await _store.UpsertRangeAsync(logs);
            return result;
        }

        public async Task<CosmosResponse<OrchestrationLog>> Delete(string id)
        {
            var result = await _store.RemoveByIdAsync(id);
            return result;
        }
    }
}
