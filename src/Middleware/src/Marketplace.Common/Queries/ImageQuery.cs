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
    public interface IImageQuery
    {
        Task Delete(string id);
        Task<Image> Get(string id);
        Task<ListPage<Image>> GetProductImages(string productID);
        Task<ListPage<Image>> List(ListArgs<Image> args);
        Task<Image> Save(Image img);
        Task<List<Image>> SaveMany(List<Image> imgs);
    }

    public class ImageQuery : IImageQuery
    {
        private readonly ICosmosStore<Image> _imageStore;
        private readonly ICosmosStore<ImageProductAssignment> _imageProductAssignmentStore;
        public ImageQuery(ICosmosStore<Image> store, ICosmosStore<ImageProductAssignment> imageProductAssignmentStore)
        {
            _imageStore = store;
            _imageProductAssignmentStore = imageProductAssignmentStore;
        }
        public async Task<ListPage<Image>> List(ListArgs<Image> args)
        {
            var query = _imageStore.Query(new FeedOptions() { EnableCrossPartitionQuery = true })
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
            var item = await _imageStore.FindAsync(id, options);
            return item;
        }

        public async Task<Image> Save(Image img)
        {
            img.timeStamp = DateTime.Now;
            var result = await _imageStore.UpsertAsync(img);
            return result.Entity;
        }

        public async Task<List<Image>> SaveMany(List<Image> imgs)
        {
            var result = await _imageStore.UpsertRangeAsync(imgs);
            return result.SuccessfulEntities.Select(e => e.Entity).ToList();
        }

        public async Task Delete(string id)
        {
            // Check if any assignments exist, if so - delete them.
            var assignments = await _imageProductAssignmentStore.Query(new FeedOptions() { EnableCrossPartitionQuery = true }).Where(x => x.ImageID == id).ToListAsync();
            await Throttler.RunAsync(assignments, 100, 5, a => _imageProductAssignmentStore.RemoveByIdAsync(a.id, new RequestOptions() { PartitionKey = new PartitionKey(a.ProductID) }));
            // Then remove the image
            var options = new RequestOptions { PartitionKey = new PartitionKey(id) };
            await _imageStore.RemoveByIdAsync(id, options);
        }

        public async Task<ListPage<Image>> GetProductImages(string productID)
        {
            var productImages = new ListPage<Image>() { Items = new List<Image>() };
            var assignments = await _imageProductAssignmentStore.Query(new FeedOptions() { EnableCrossPartitionQuery = true }).Where(x => x.ProductID == productID).ToListAsync();
            await Throttler.RunAsync(assignments, 100, 5, async a =>
            {
                var options = new RequestOptions { PartitionKey = new PartitionKey(a.ImageID) };
                var image = await _imageStore.FindAsync(a.ImageID, options);
                productImages.Items.Add(image);
            });
            return productImages;
        }
    }
}
