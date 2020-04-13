using System.Collections.Generic;
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
        Task<List<Image>> GetProductImages(string productID);
    }

    public class ImageCommand : IImageCommand
    {
        private readonly AppSettings _settings;
        private readonly ImageQuery _img;

        public ImageCommand(AppSettings settings, ImageQuery img)
        {
            _settings = settings;
            _img = img;
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
        public async Task Delete(string id)
        {
            await _img.Delete(id);
        }
        public async Task<List<Image>> GetProductImages(string productID)
        {
            return await _img.GetProductImages(productID);
        }
    }
}
