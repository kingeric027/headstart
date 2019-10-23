using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmonaut;
using Cosmonaut.Extensions;
using Cosmonaut.Response;
using Marketplace.Common.Extensions;
using Marketplace.Common.Helpers;
using Marketplace.Common.Models;

namespace Marketplace.Common.Queries
{
    public class SupplierQuery
    {
        private readonly ICosmosStore<OrchestrationSupplier> _store;

        public SupplierQuery(ICosmosStore<OrchestrationSupplier> store)
        {
            _store = store;
        }

        public async Task<Models.ListPage<OrchestrationSupplier>> List(IListArgs args)
        {
            var query = _store.Query()
                .Filter(args)
                .Search(args)
                .Sort(args);

            var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
            var count = await query.CountAsync();
            return list.ToListPage(args.Page, args.PageSize, count);
        }

        public async Task<OrchestrationSupplier> Get(string id)
        {
            var item = await _store.FindAsync(id);
            return item;
        }

        public async Task<OrchestrationSupplier> GetByUserID(string id)
        {
            var list = await _store.Query().Where(s => s.UserID == id).ToListAsync();
            return list.FirstOrDefault();
        }

        public async Task<CosmosResponse<OrchestrationSupplier>> Upsert(OrchestrationSupplier log)
        {
            log.timeStamp = DateTime.Now;
            var result = await _store.UpsertAsync(log);
            return result;
        }

        public async Task<CosmosMultipleResponse<OrchestrationSupplier>> UpsertList(List<OrchestrationSupplier> logs)
        {
            var result = await _store.UpsertRangeAsync(logs);
            return result;
        }

        public async Task<CosmosResponse<OrchestrationSupplier>> Delete(string id)
        {
            var result = await _store.RemoveByIdAsync(id);
            return result;
        }
    }
}
