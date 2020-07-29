using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosmonaut.Extensions;
using Marketplace.Common.Models.Marketplace;
using Marketplace.Models;
using ordercloud.integrations.cms;
using ordercloud.integrations.library;
using OrderCloud.SDK;


namespace Marketplace.Common.Commands.Crud
{
    public interface IMarketplaceKitProductCommand
    {
        Task<MarketplaceKitProduct> Get(string id, VerifiedUserContext user);
        Task<ListPage<MarketplaceKitProduct>> List(ListArgs<MarketplaceProduct> args, VerifiedUserContext user);
        Task<MarketplaceKitProduct> Post(MarketplaceKitProduct kitProduct, VerifiedUserContext user);
        Task<MarketplaceKitProduct> Put(string id, MarketplaceKitProduct kitProduct, KitProductDocument kitDoc, VerifiedUserContext user);
        Task Delete(string id, VerifiedUserContext user);
        Task<List<AssetForDelivery>> GetProductImages(string productID, VerifiedUserContext user);
        Task<List<AssetForDelivery>> GetProductAttachments(string productID, VerifiedUserContext user);
    }

    public class MarketplaceKitProductCommand : IMarketplaceKitProductCommand
    {
        private readonly IDocumentQuery _query;
        private readonly IOrderCloudClient _oc;
        private readonly IAssetedResourceQuery _assetedResources;
        private readonly IAssetQuery _assets;

        public MarketplaceKitProductCommand(AppSettings settings, IAssetedResourceQuery assetedResources, IAssetQuery assets, IOrderCloudClient elevatedOc, IDocumentQuery query)
        {
            _assetedResources = assetedResources;
            _assets = assets;
            _oc = elevatedOc;
            _query = query;
        }

        public async Task<List<AssetForDelivery>> GetProductImages(string productID, VerifiedUserContext user)
        {
            var assets = await _assetedResources.ListAssets(new Resource(ResourceType.Products, productID), user);
            var images = assets.Where(a => a.Type == AssetType.Image).ToList();
            return images;
        }
        public async Task<List<AssetForDelivery>> GetProductAttachments(string productID, VerifiedUserContext user)
        {
            var assets = await _assetedResources.ListAssets(new Resource(ResourceType.Products, productID), user);
            var attachments = assets.Where(a => a.Type == AssetType.Attachment).ToList();
            return attachments;
        }
        public async Task<MarketplaceKitProduct> Get(string id, VerifiedUserContext user)
        {
            var _product = await _oc.Products.GetAsync<MarketplaceProduct>(id, user.AccessToken);
            var _images = GetProductImages(id, user);
            var _attachments = GetProductAttachments(id, user);
            var _productAssignments = await _query.Get<KitProduct>("KitProduct", _product.ID, user);
            return new MarketplaceKitProduct
            {
                Product = _product,
                Images = await _images,
                Attachments = await _attachments,
                ProductAssignments = _productAssignments.Doc
            };
        }

        public async Task<ListPage<MarketplaceKitProduct>> List(ListArgs<MarketplaceProduct> args, VerifiedUserContext user)
        {
            var _productsList = await _oc.Products.ListAsync<MarketplaceProduct>(
                filters: args.ToFilterString(),
                search: args.Search,
                pageSize: args.PageSize,
                page: args.Page,
                accessToken: user.AccessToken);
            var _kitProducts = await _query.List<KitProduct>("KitProduct", args, user);
            var _kitProductList = new List<MarketplaceKitProduct>();

            await Throttler.RunAsync(_kitProducts.Items, 100, 10, async product =>
            {
                var parentProduct = await _oc.Products.GetAsync(product.InteropID);
                var _images = GetProductImages(product.InteropID, user);
                var _attachments = GetProductAttachments(product.InteropID, user);
                _kitProductList.Add(new MarketplaceKitProduct
                {
                    Product = parentProduct,
                    Images = await _images,
                    Attachments = await _attachments,
                    ProductAssignments = product.Doc
                });
            });
            return new ListPage<MarketplaceKitProduct>
            {
                Meta = _productsList.Meta,
                Items = _kitProductList
            };
        }
        public async Task<MarketplaceKitProduct> Post(MarketplaceKitProduct kitProduct, VerifiedUserContext user)
        {
            var _product = await _oc.Products.CreateAsync<MarketplaceProduct>(kitProduct.Product, user.AccessToken);
            var kitProductDoc = new KitProductDocument();
            kitProductDoc.InteropID = _product.ID;
            kitProductDoc.Doc = kitProduct.ProductAssignments;
            var _productAssignments = await _query.Create<KitProduct>("KitProduct", kitProductDoc, user);
            return new MarketplaceKitProduct
            {
                Product = _product,
                Images = new List<AssetForDelivery>(),
                Attachments = new List<AssetForDelivery>(),
                ProductAssignments = _productAssignments.Doc
            };
        }

        public async Task<MarketplaceKitProduct> Put(string id, MarketplaceKitProduct kitProduct, KitProductDocument kitDoc, VerifiedUserContext user)
        {
            var _updatedProduct = await _oc.Products.SaveAsync<MarketplaceProduct>(kitProduct.Product.ID, kitProduct.Product, user.AccessToken);
            var _productAssignments = await _query.Update<KitProduct>("KitProduct", _updatedProduct.ID, kitDoc, user);
            var _images = await GetProductImages(_updatedProduct.ID, user);
            var _attachments = await GetProductAttachments(_updatedProduct.ID, user);
            return new MarketplaceKitProduct
            {
                Product = _updatedProduct,
                Images = _images,
                Attachments = _attachments,
                ProductAssignments = _productAssignments.Doc
            };
        }

        public async Task Delete(string id, VerifiedUserContext user)
        {
            var product = await _oc.Products.GetAsync(id); // This is temporary to accommodate bad data where product.ID != product.DefaultPriceScheduleID
            var _images = await GetProductImages(id, user);
            var _attachments = await GetProductAttachments(id, user);
            // Delete images, attachments, and assignments associated with the requested product
            await Task.WhenAll(
                Throttler.RunAsync(_images, 100, 5, i => _assets.Delete(i.InteropID, user)),
                Throttler.RunAsync(_attachments, 100, 5, i => _assets.Delete(i.InteropID, user)),
                _query.Delete<KitProduct>("KitProduct", product.ID, user),
            _oc.Products.DeleteAsync(id, user.AccessToken)
            );
        }
    }
}
