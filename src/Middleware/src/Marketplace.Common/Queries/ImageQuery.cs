using Cosmonaut;
using Cosmonaut.Extensions;
using Marketplace.Common.Models;
using Marketplace.Helpers;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Queries
{
    public class ImageQuery
    {
        private readonly ICosmosStore<Image> _store;
        public ImageQuery(ICosmosStore<Image> store)
        {
            _store = store;
        }
        public async Task<ListPage<Image>> List(ListArgs<Image> args)
        {
            var query = _store.Query(new FeedOptions() { EnableCrossPartitionQuery = true })
                .Search(args)
                .Filter(args)
                .Sort(args);
            var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
            var count = await query.CountAsync();
            return list.ToListPage(args.Page, args.PageSize, count);
        }

        public async Task<Image> Get(string id)
        {
            var options = new RequestOptions { PartitionKey = new PartitionKey(id) };
            var item = await _store.FindAsync(id, options);
            return item;
        }

        public async Task<Image> Save(Image img)
        {
            img.timeStamp = DateTime.Now;
            var result = await _store.UpsertAsync(img);
            return result.Entity;
        }

        public async Task<List<Image>> SaveMany(List<Image> imgs)
        {
            var result = await _store.UpsertRangeAsync(imgs);
            return result.SuccessfulEntities.Select(e => e.Entity).ToList();
        }

        public async Task Delete(string id)
        {
            var options = new RequestOptions { PartitionKey = new PartitionKey(id) };
            await _store.RemoveByIdAsync(id, options);
        }
    }
}
