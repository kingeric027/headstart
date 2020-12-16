using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosmonaut.Extensions;
using Marketplace.Common.Models.Marketplace;
using Marketplace.Common.Services.CMS;
using Marketplace.Common.Services.CMS.Models;
using Marketplace.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ordercloud.integrations.library;
using ordercloud.integrations.library.helpers;
using OrderCloud.SDK;


namespace Marketplace.Common.Commands.Crud
{
    public interface IMarketplaceKitProductCommand
    {
        Task<MarketplaceKitProduct> Get(string id, string token);
        Task<MarketplaceMeKitProduct> GetMeKit(string id, VerifiedUserContext user);
        Task<ListPage<MarketplaceKitProduct>> List(ListArgs<Document<KitProduct>> args, string token);
        Task<MarketplaceKitProduct> Post(MarketplaceKitProduct kitProduct, string token);
        Task<MarketplaceKitProduct> Put(string id, MarketplaceKitProduct kitProduct, string token);
        Task Delete(string id, string token);
        Task<List<Asset>> GetProductImages(string productID, string token);
        Task<List<Asset>> GetProductAttachments(string productID, string token);
    }

    public class MarketplaceKitProductCommand : IMarketplaceKitProductCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly ICMSClient _cms;
        private readonly IMeProductCommand _meProductCommand;

        public MarketplaceKitProductCommand(
            AppSettings settings,
            ICMSClient cms,
            IOrderCloudClient elevatedOc,
            IMeProductCommand meProductCommand
        )
        {
            _cms = cms;
            _oc = elevatedOc;
            _meProductCommand = meProductCommand;
        }

        public async Task<List<Asset>> GetProductImages(string productID, string token)
        {
            var assets = await _cms.Assets.ListAssets(ResourceType.Products, productID, new ListArgsPageOnly() { PageSize = 100}, token);
            var images = assets.Items.Where(a => a.Type == AssetType.Image).ToList();
            return images;
        }
        public async Task<List<Asset>> GetProductAttachments(string productID, string token)
        {
            var assets = await _cms.Assets.ListAssets(ResourceType.Products, productID, new ListArgsPageOnly() { PageSize = 100 }, token);
            var attachments = assets.Items.Where(a => a.Title == "Product_Attachment").ToList();
            return attachments;
        }
        public async Task<MarketplaceKitProduct> Get(string id, string token)
        {
            var _product = await _oc.Products.GetAsync<Product>(id, token);
            var _images = GetProductImages(id, token);
            var _attachments = GetProductAttachments(id, token);
            var _productAssignments = await _cms.Documents.Get<KitProduct>("KitProduct", _product.ID, token);

            return new MarketplaceKitProduct
            {
                ID = _product.ID,
                Name = _product.Name,
                Product = _product,
                Images = await _images,
                Attachments = await _attachments,
                ProductAssignments = await _getKitDetails(_productAssignments.Doc, token)
            };
        }
        public async Task<MarketplaceMeKitProduct> GetMeKit(string id, VerifiedUserContext user)
        {
            var _product = await _oc.Me.GetProductAsync<MarketplaceMeProduct>(id, user.AccessToken);
            var _images = GetProductImages(id, user.AccessToken);
            var _attachments = GetProductAttachments(id, user.AccessToken);
            var _productAssignments = await _cms.Documents.Get<MeKitProduct>("KitProduct", _product.ID, user.AccessToken);
            var meKitProduct = new MarketplaceMeKitProduct
            {
                ID = _product.ID,
                Name = _product.Name,
                Product = _product,
                Images = await _images,
                Attachments = await _attachments,
                ProductAssignments = await _getMeKitDetails(_productAssignments.Doc, user.AccessToken)
            };
            return await _meProductCommand.ApplyBuyerPricing(meKitProduct, user);
        }

        public async Task<ListPage<MarketplaceKitProduct>> List(ListArgs<Document<KitProduct>> args, string token)
        {
            var _kitProducts = await _cms.Documents.List<KitProduct>("KitProduct", args, token);
            var _kitProductList = new List<MarketplaceKitProduct>();

            await Throttler.RunAsync(_kitProducts.Items, 100, 10, async product =>
            {
                var parentProduct = await _oc.Products.GetAsync(product.ID);
                var _images = GetProductImages(product.ID, token);
                var _attachments = GetProductAttachments(product.ID, token);
                _kitProductList.Add(new MarketplaceKitProduct
                {
                    ID = parentProduct.ID,
                    Name = parentProduct.Name,
                    Product = parentProduct,
                    Images = await _images,
                    Attachments = await _attachments,
                    ProductAssignments = await _getKitDetails(product.Doc, token)
                });
            });
            return new ListPage<MarketplaceKitProduct>
            {
                Meta = _kitProducts.Meta,
                Items = _kitProductList
            };
        }
        public async Task<MarketplaceKitProduct> Post(MarketplaceKitProduct kitProduct, string token)
        {
            var _product = await _oc.Products.CreateAsync<Product>(kitProduct.Product, token);
            var kitProductDoc = new Document<KitProduct>();
            kitProductDoc.ID = _product.ID;
            kitProductDoc.Doc = kitProduct.ProductAssignments;
            var _productAssignments = await _cms.Documents.Create("KitProduct", kitProductDoc, token);
            return new MarketplaceKitProduct
            {
                ID = _product.ID,
                Name = _product.Name,
                Product = _product,
                Images = new List<Asset>(),
                Attachments = new List<Asset>(),
                ProductAssignments = await _getKitDetails(_productAssignments.Doc, token)
            };
        }

        public async Task<MarketplaceKitProduct> Put(string id, MarketplaceKitProduct kitProduct, string token)
        {
            var _updatedProduct = await _oc.Products.SaveAsync<Product>(kitProduct.Product.ID, kitProduct.Product, token);
            var kitProductDoc = new Document<KitProduct>();
            kitProductDoc.ID = _updatedProduct.ID;
            kitProductDoc.Doc = kitProduct.ProductAssignments;
            var _productAssignments = await _cms.Documents.Save<KitProduct>("KitProduct", _updatedProduct.ID, kitProductDoc, token);
            var _images = await GetProductImages(_updatedProduct.ID, token);
            var _attachments = await GetProductAttachments(_updatedProduct.ID, token);
            return new MarketplaceKitProduct
            {
                ID = _updatedProduct.ID,
                Name = _updatedProduct.Name,
                Product = _updatedProduct,
                Images = _images,
                Attachments = _attachments,
                ProductAssignments = await _getKitDetails(_productAssignments.Doc, token)
            };
        }

        public async Task<KitProduct> _getKitDetails(KitProduct kit, string token)
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
                    p.Images = await GetProductImages(p.ID, token);
                    p.Attachments = await GetProductAttachments(p.ID, token);

                } catch(Exception)
                {
                    p.Product = null;
                }
            }

            // filter out products in kit that we failed to retrieve details for (product might have been deleted since kit was created)
            kit.ProductsInKit = kit.ProductsInKit.Where(p => p.Product != null).ToList();
            return kit;
        }

        public async Task<MeKitProduct> _getMeKitDetails(MeKitProduct kit, string token)
        {
            // get product, specs, variants, and images for each product in the kit
            foreach (var p in kit.ProductsInKit)
            {
                try
                {
                    var productRequest = _oc.Me.GetProductAsync<MarketplaceMeProduct>(p.ID, token);
                    var specListRequest = ListAllAsync.List((page) => _oc.Products.ListSpecsAsync(p.ID, page: page, pageSize: 100));
                    var variantListRequest = ListAllAsync.List((page) => _oc.Products.ListVariantsAsync(p.ID, page: page, pageSize: 100));
                    await Task.WhenAll(specListRequest, variantListRequest);

                    var product = await productRequest;
                    if(product?.PriceSchedule != null)
                    {
                        // set min/max from kit only if its within the bounds of what the product can set
                        // this should be enforced at the admin creation level but may change after initially set
                        var productMax = product.PriceSchedule.MaxQuantity;
                        var productMin = product.PriceSchedule.MinQuantity;
                        var kitMax = p.MaxQty;
                        var kitMin = p.MinQty;

                        // set product min
                        if(kitMin != null && (productMin == null || productMin < kitMin ))
                        {
                            product.PriceSchedule.MinQuantity = kitMin;
                        }

                        // set product max
                        if(kitMax != null && (productMax == null || productMax > kitMax) && kitMax > product.PriceSchedule.MinQuantity) // extra check needed because minqty might have changed
                        {
                            product.PriceSchedule.MaxQuantity = kitMax;
                        }
                    }

                    p.Product = product;
                    p.Specs = await specListRequest;
                    p.Variants = await variantListRequest;
                    p.Images = await GetProductImages(p.ID, token);
                    p.Attachments = await GetProductAttachments(p.ID, token);
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

        public async Task Delete(string id, string token)
        {
            var product = await _oc.Products.GetAsync(id);
            var _images = await GetProductImages(id, token);
            var _attachments = await GetProductAttachments(id, token);
            // Delete images, attachments, and assignments associated with the requested product
            await Task.WhenAll(
                Throttler.RunAsync(_images, 100, 5, i => _cms.Assets.Delete(i.ID, token)),
                Throttler.RunAsync(_attachments, 100, 5, i => _cms.Assets.Delete(i.ID, token)),
                _cms.Documents.Delete("KitProduct", product.ID, token),
            _oc.Products.DeleteAsync(id, token)
            );
        }
    }
}
