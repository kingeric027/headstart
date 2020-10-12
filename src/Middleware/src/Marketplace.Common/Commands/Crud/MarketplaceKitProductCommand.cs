using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosmonaut.Extensions;
using Marketplace.Common.Models.Marketplace;
using Marketplace.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ordercloud.integrations.cms;
using ordercloud.integrations.library;
using ordercloud.integrations.library.helpers;
using OrderCloud.SDK;


namespace Marketplace.Common.Commands.Crud
{
    public interface IMarketplaceKitProductCommand
    {
        Task<MarketplaceKitProduct> Get(string id, VerifiedUserContext user);
        Task<MarketplaceMeKitProduct> GetMeKit(string id, VerifiedUserContext user);
        Task<ListPage<MarketplaceKitProduct>> List(ListArgs<Document<KitProduct>> args, VerifiedUserContext user);
        Task<MarketplaceKitProduct> Post(MarketplaceKitProduct kitProduct, VerifiedUserContext user);
        Task<MarketplaceKitProduct> Put(string id, MarketplaceKitProduct kitProduct, VerifiedUserContext user);
        Task Delete(string id, VerifiedUserContext user);
        Task<List<Asset>> GetProductImages(string productID, VerifiedUserContext user);
        Task<List<Asset>> GetProductAttachments(string productID, VerifiedUserContext user);
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

        public async Task<List<Asset>> GetProductImages(string productID, VerifiedUserContext user)
        {
            var assets = await _assetedResources.ListAssets(new Resource(ResourceType.Products, productID), new ListArgsPageOnly() { PageSize = 100},  user);
            var images = assets.Items.Where(a => a.Type == AssetType.Image).ToList();
            return images;
        }
        public async Task<List<Asset>> GetProductAttachments(string productID, VerifiedUserContext user)
        {
            var assets = await _assetedResources.ListAssets(new Resource(ResourceType.Products, productID), new ListArgsPageOnly() { PageSize = 100 }, user);
            var attachments = assets.Items.Where(a => a.Title == "Product_Attachment").ToList();
            return attachments;
        }
        public async Task<MarketplaceKitProduct> Get(string id, VerifiedUserContext user)
        {
            var _product = await _oc.Products.GetAsync<Product>(id, user.AccessToken);
            var _images = GetProductImages(id, user);
            var _attachments = GetProductAttachments(id, user);
            var _productAssignments = await _query.Get<KitProduct>("KitProduct", _product.ID, user);

            return new MarketplaceKitProduct
            {
                ID = _product.ID,
                Name = _product.Name,
                Product = _product,
                Images = await _images,
                Attachments = await _attachments,
                ProductAssignments = await _getKitDetails(_productAssignments.Doc, user)
            };
        }
        public async Task<MarketplaceMeKitProduct> GetMeKit(string id, VerifiedUserContext user)
        {
            var _product = await _oc.Me.GetProductAsync<MarketplaceMeProduct>(id, user.AccessToken);
            var _images = GetProductImages(id, user);
            var _attachments = GetProductAttachments(id, user);
            var _productAssignments = await _query.Get<MeKitProduct>("KitProduct", _product.ID, user);
            return new MarketplaceMeKitProduct
            {
                ID = _product.ID,
                Name = _product.Name,
                Product = _product,
                Images = await _images,
                Attachments = await _attachments,
                ProductAssignments = await _getMeKitDetails(_productAssignments.Doc, user)
            };
        }

        public async Task<ListPage<MarketplaceKitProduct>> List(ListArgs<Document<KitProduct>> args, VerifiedUserContext user)
        {
            var _kitProducts = await _query.List<KitProduct>("KitProduct", args, user);
            var _kitProductList = new List<MarketplaceKitProduct>();

            await Throttler.RunAsync(_kitProducts.Items, 100, 10, async product =>
            {
                var parentProduct = await _oc.Products.GetAsync(product.ID);
                var _images = GetProductImages(product.ID, user);
                var _attachments = GetProductAttachments(product.ID, user);
                _kitProductList.Add(new MarketplaceKitProduct
                {
                    ID = parentProduct.ID,
                    Name = parentProduct.Name,
                    Product = parentProduct,
                    Images = await _images,
                    Attachments = await _attachments,
                    ProductAssignments = await _getKitDetails(product.Doc, user)
                });
            });
            return new ListPage<MarketplaceKitProduct>
            {
                Meta = _kitProducts.Meta,
                Items = _kitProductList
            };
        }
        public async Task<MarketplaceKitProduct> Post(MarketplaceKitProduct kitProduct, VerifiedUserContext user)
        {
            var _product = await _oc.Products.CreateAsync<Product>(kitProduct.Product, user.AccessToken);
            var kitProductDoc = new KitProductDocument();
            kitProductDoc.ID = _product.ID;
            kitProductDoc.Doc = kitProduct.ProductAssignments;
            var _productAssignments = await _query.Create<KitProduct>("KitProduct", kitProductDoc, user);
            return new MarketplaceKitProduct
            {
                ID = _product.ID,
                Name = _product.Name,
                Product = _product,
                Images = new List<Asset>(),
                Attachments = new List<Asset>(),
                ProductAssignments = await _getKitDetails(_productAssignments.Doc, user)
            };
        }

        public async Task<MarketplaceKitProduct> Put(string id, MarketplaceKitProduct kitProduct, VerifiedUserContext user)
        {
            var _updatedProduct = await _oc.Products.SaveAsync<Product>(kitProduct.Product.ID, kitProduct.Product, user.AccessToken);
            var kitProductDoc = new KitProductDocument();
            kitProductDoc.ID = _updatedProduct.ID;
            kitProductDoc.Doc = kitProduct.ProductAssignments;
            var _productAssignments = await _query.Save<KitProduct>("KitProduct", _updatedProduct.ID, kitProductDoc, user);
            var _images = await GetProductImages(_updatedProduct.ID, user);
            var _attachments = await GetProductAttachments(_updatedProduct.ID, user);
            return new MarketplaceKitProduct
            {
                ID = _updatedProduct.ID,
                Name = _updatedProduct.Name,
                Product = _updatedProduct,
                Images = _images,
                Attachments = _attachments,
                ProductAssignments = await _getKitDetails(_productAssignments.Doc, user)
            };
        }

        public async Task<KitProduct> _getKitDetails(KitProduct kit, VerifiedUserContext user)
        {
            
            // get product, specs, variants, and images for each product in the kit
            foreach (var p in kit.ProductsInKit)
            {
                try
                {
                    var productRequest = _oc.Products.GetAsync<MarketplaceProduct>(p.ID);
                    var specListRequest = ListAllAsync.List((page) => _oc.Products.ListSpecsAsync(p.ID, page: page, pageSize: 100));
                    var variantListRequest = ListAllAsync.List((page) => _oc.Products.ListVariantsAsync(p.ID, page: page, pageSize: 100));
                    await Task.WhenAll(specListRequest, variantListRequest);

                    p.Product = await productRequest;
                    p.Specs = await specListRequest;
                    p.Variants = await variantListRequest;
                    p.Images = await GetProductImages(p.ID, user);
                } catch(Exception)
                {
                    p.Product = null;
                }
            }

            // filter out products in kit that we failed to retrieve details for (product might have been deleted since kit was created)
            kit.ProductsInKit = kit.ProductsInKit.Where(p => p.Product != null).ToList();
            return kit;
        }

        public async Task<MeKitProduct> _getMeKitDetails(MeKitProduct kit, VerifiedUserContext user)
        {
            // get product, specs, variants, and images for each product in the kit
            foreach (var p in kit.ProductsInKit)
            {
                try
                {
                    var productRequest = _oc.Me.GetProductAsync<MarketplaceMeProduct>(p.ID, user.AccessToken);
                    var specListRequest = ListAllAsync.List((page) => _oc.Products.ListSpecsAsync(p.ID, page: page, pageSize: 100));
                    var variantListRequest = ListAllAsync.List((page) => _oc.Products.ListVariantsAsync(p.ID, page: page, pageSize: 100));
                    await Task.WhenAll(specListRequest, variantListRequest);

                    p.Product = await productRequest;
                    p.Specs = await specListRequest;
                    p.Variants = await variantListRequest;
                    p.Images = await GetProductImages(p.ID, user);
                }
                catch (Exception)
                {
                    p.Product = null;
                }
            }

            // filter out products in kit that we failed to retrieve details for (product might have been deleted since kit was created)
            kit.ProductsInKit = kit.ProductsInKit.Where(p => p.Product != null).ToList();
            return kit;
        }

        public async Task Delete(string id, VerifiedUserContext user)
        {
            var product = await _oc.Products.GetAsync(id);
            var _images = await GetProductImages(id, user);
            var _attachments = await GetProductAttachments(id, user);
            // Delete images, attachments, and assignments associated with the requested product
            await Task.WhenAll(
                Throttler.RunAsync(_images, 100, 5, i => _assets.Delete(i.ID, user)),
                Throttler.RunAsync(_attachments, 100, 5, i => _assets.Delete(i.ID, user)),
                _query.Delete("KitProduct", product.ID, user),
            _oc.Products.DeleteAsync(id, user.AccessToken)
            );
        }
    }
}
