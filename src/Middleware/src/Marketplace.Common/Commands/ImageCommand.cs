using System.Linq;
using System.Threading.Tasks;
using Cosmonaut;
using Cosmonaut.Extensions;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Marketplace.Helpers;
using Marketplace.Helpers.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands
{
    public interface IImageCommand
    {
        Task<ListPage<Image>> List(ListArgs<Image> marketplaceListArgs);
        Task<Image> Get(string id);
        Task<Image> Create(Image img);
        Task Delete(string ID);
    }

    public class ImageCommand : IImageCommand
    {
        private readonly AppSettings _settings;
        private readonly ImageQuery _img;
        private readonly ICosmosStore<ImageProductAssignment> _store;

        public ImageCommand(AppSettings settings, ImageQuery img, ICosmosStore<ImageProductAssignment> store)
        {
            _settings = settings;
            _img = img;
            _store = store;
        }

        public async Task<ListPage<Image>> List(ListArgs<Image> marketplaceListArgs)
        {
            return await _img.List(marketplaceListArgs);
        }
        public async Task<Image> Get(string id)
        {
            return await _img.Get(id);
        }
        public async Task<Image> Create(Image img)
        {
            return await _img.Save(img);
        }
        public async Task Delete(string ID)
        {
            // NOT WORKING - Check if any assignments exist, if so - delete them.
            //var assignments = await _store.Query(new FeedOptions() { EnableCrossPartitionQuery = true }).ToListAsync();
            //if (assignments[0] != null)
            //{
            //    await Throttler.RunAsync(assignments, 100, 5, a => _store.RemoveAsync(x => x.ProductID == a.ProductID, new FeedOptions() { EnableCrossPartitionQuery = true, PartitionKey = new PartitionKey(ID) }));
            //}
            //Delete the image
           await _img.Delete(ID);
        }
    }
}
