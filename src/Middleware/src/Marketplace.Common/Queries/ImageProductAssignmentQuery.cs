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
using static Marketplace.Helpers.Exceptions.ApiErrorException;

namespace Marketplace.Common.Queries
{
    public interface IImageProductAssignmentQuery
    {
        Task Delete(string ImageID, string ProductID);
        Task<ListPage<ImageProductAssignment>> List(ListArgs<ImageProductAssignment> args);
        Task<ImageProductAssignment> Save(ImageProductAssignment pia);
    }

    public class ImageProductAssignmentQuery : IImageProductAssignmentQuery
    {
        private readonly ICosmosStore<ImageProductAssignment> _store;
        public ImageProductAssignmentQuery(ICosmosStore<ImageProductAssignment> store)
        {
            _store = store;
        }
        public async Task<ListPage<ImageProductAssignment>> List(ListArgs<ImageProductAssignment> args)
        {
            var query = _store.Query(new FeedOptions() { EnableCrossPartitionQuery = true })
                .Search(args)
                .Filter(args)
                .Sort(args);
            var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
            var count = await query.CountAsync();
            return list.ToListPage(args.Page, args.PageSize, count);
        }

        public async Task<ImageProductAssignment> Save(ImageProductAssignment pia)
        {
            pia.id = $"{pia.ImageID}-{pia.ProductID}";
            pia.timeStamp = DateTime.Now;
            var result = await _store.UpsertAsync(pia);
            return result.Entity;
        }

        public async Task Delete(string ImageID, string ProductID)
        {
            var options = new RequestOptions { PartitionKey = new PartitionKey(ProductID) };
            await _store.RemoveByIdAsync($"{ImageID}-{ProductID}", options);
        }
    }
}
