using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosmonaut;
using Cosmonaut.Extensions;
using Marketplace.Common.Commands;
using Microsoft.Azure.Documents.Client;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Common.Queries
{
    public class ChiliPublishConfigQuery : IPartitionedCosmosQuery<ChiliConfig>
    {
        private readonly ICosmosStore<ChiliConfig> _store;

        public ChiliPublishConfigQuery(ICosmosStore<ChiliConfig> store)
        {
            _store = store;
        }

        public async Task<ListPage<ChiliConfig>> List(IListArgs args, string partitionKey)
        {
            var query = _store.Query(new FeedOptions() { EnableCrossPartitionQuery = true })
                .Search(args)
                .Filter(args)
                .Sort(args);
            var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
            var count = await query.CountAsync();
            return list.ToListPage(args.Page, args.PageSize, count);
        }

        public async Task<ChiliConfig> Get(string id, string partitionKey)
        {
            var item = await _store.FindAsync(id, partitionKeyValue: partitionKey);
            return item;
        }

        public async Task<ChiliConfig> Save(ChiliConfig config, string partitionKey)
        {
            config.timeStamp = DateTime.Now;

            var result = await _store.UpsertAsync(config);
            return result.Entity;
        }

        public async Task<List<ChiliConfig>> SaveMany(List<ChiliConfig> configs, string partitionKey)
        {
            var result = await _store.UpsertRangeAsync(configs);
            return result.SuccessfulEntities.Select(e => e.Entity).ToList();
        }

        public async Task Delete(string id, string partitionKey)
        {
            await _store.RemoveByIdAsync(id, partitionKey);
        }
    }
}
