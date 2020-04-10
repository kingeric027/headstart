using System.Threading.Tasks;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Marketplace.Helpers;
using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Exceptions;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;
using static Marketplace.Helpers.Exceptions.ApiErrorException;

namespace Marketplace.Common.Commands
{
    public interface IImageProductAssignmentCommand
    {
        Task<ListPage<ImageProductAssignment>> List(ListArgs<ImageProductAssignment> marketplaceListArgs);
        Task<ImageProductAssignment> Create(ImageProductAssignment pia, VerifiedUserContext user);
        Task Delete(string ImageID, string ProductID);
    }

    public class ImageProductAssignmentCommand : IImageProductAssignmentCommand
    {
        private readonly AppSettings _settings;
        private readonly ImageProductAssignmentQuery _ipa;
        private readonly ImageQuery _img;
        private readonly IOrderCloudClient _oc;

        public ImageProductAssignmentCommand(AppSettings settings, ImageProductAssignmentQuery ipa, ImageQuery img, IOrderCloudClient oc)
        {
            _settings = settings;
            _ipa = ipa;
            _img = img;
            _oc = oc;
        }
        public async Task<ListPage<ImageProductAssignment>> List(ListArgs<ImageProductAssignment> marketplaceListArgs)
        {
            return await _ipa.List(marketplaceListArgs);
        }
        public async Task<ImageProductAssignment> Create(ImageProductAssignment pia, VerifiedUserContext user)
        {
            // Ensure that an OrderCloud product with ID = pia.ProductID exists
            var ocProduct = await _oc.Products.GetAsync(pia.ProductID, accessToken: user.AccessToken);
            // Ensure that an Image record exists in Cosmos for the ID Specified
            var i = await _img.Get(pia.ImageID);
            if (i == null) throw new NotFoundException("Image", pia.ImageID);
            return await _ipa.Save(pia);
        }
        public async Task Delete(string ImageID, string ProductID)
        {
            await _ipa.Delete(ImageID, ProductID);
        }
    }
}
