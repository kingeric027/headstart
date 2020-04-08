using System.Threading.Tasks;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Marketplace.Helpers;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands
{
    public interface IImageCommand
    {
        Task<ListPage<Image>> List(ListArgs<Image> marketplaceListArgs);
        Task<Image> Create(Image img);
        Task Delete(string ID);
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
        public async Task<Image> Create(Image img)
        {
            return await _img.Save(img);
        }
        public async Task Delete(string ID)
        {
            await _img.Delete(ID);
        }
    }
}
