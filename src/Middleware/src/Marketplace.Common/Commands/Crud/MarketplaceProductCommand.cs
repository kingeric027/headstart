﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.CMS.Models;
using Marketplace.CMS.Queries;
using Marketplace.Common.Models;
using Marketplace.Helpers;
using Marketplace.Helpers.Models;
using Marketplace.Models;
using Newtonsoft.Json;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.Crud
{
    public interface IMarketplaceProductCommand
    {
        Task<SuperMarketplaceProduct> Get(string id, VerifiedUserContext user);
        Task<ListPage<SuperMarketplaceProduct>> List(ListArgs<MarketplaceProduct> args, VerifiedUserContext user);
        Task<SuperMarketplaceProduct> Post(SuperMarketplaceProduct product, VerifiedUserContext user);
        Task<SuperMarketplaceProduct> Put(string id, SuperMarketplaceProduct product, VerifiedUserContext user);
        Task Delete(string id, VerifiedUserContext user);

    }
    public class DefaultOptionSpecAssignment
    {
        public string SpecID { get; set; }
        public string OptionID { get; set; }
    }
    public class MarketplaceProductCommand : IMarketplaceProductCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly IAssetAssignmentQuery _assetAssignments;
        private readonly IImageCommand _imgCommand;
        public MarketplaceProductCommand(AppSettings settings, IImageCommand imgCommand, IAssetAssignmentQuery assetAssignments)
        {
            _oc = new OrderCloudClient(new OrderCloudClientConfig()
            {
                ApiUrl = settings.OrderCloudSettings.ApiUrl,
                AuthUrl = settings.OrderCloudSettings.AuthUrl,
            });
            _imgCommand = imgCommand;
            _assetAssignments = assetAssignments;
        }
        private async Task<List<Asset>> GetProductImages(string productID, string containerID)
        {
            var imageArgs = new ListArgs<Asset>
            {
                Filters = new List<ListFilter>()
                {
                    new ListFilter()
                    {
                        Name="ResourceID",
                        Values = new List<ListFilterValue>()
                        {
                            new ListFilterValue() { Operator = ListFilterOperator.Equal, Term = productID}
                        }
                    }
                }
            };
            var imageAssignments = await _assetAssignments.List(containerID, imageArgs);
            var imagesToReturn = new List<Asset>();
            foreach (var a in imageAssignments.Items)
            {
                if (a.Asset.Type == AssetType.Image)
                {
                    imagesToReturn.Add(a.Asset);
                }
            };
            return imagesToReturn;
        }
        public async Task<SuperMarketplaceProduct> Get(string id, VerifiedUserContext user)
        {
            var _product =  await _oc.Products.GetAsync<MarketplaceProduct>(id, user.AccessToken);
            var _priceSchedule = await _oc.PriceSchedules.GetAsync<PriceSchedule>(_product.DefaultPriceScheduleID, user.AccessToken);
            var _specs = await _oc.Products.ListSpecsAsync(id, null, null, null, 1, 100, null, user.AccessToken);
            var _variants = await _oc.Products.ListVariantsAsync<MarketplaceVariant>(id, null, null, null, 1, 100, null, user.AccessToken);
            var _images = await GetProductImages(_product.ID, "SEB");
            return new SuperMarketplaceProduct
            {
                Product = _product,
                PriceSchedule = _priceSchedule,
                Specs = _specs.Items,
                Variants = _variants.Items,
                Images = _images
            };
        }

        public async Task<ListPage<SuperMarketplaceProduct>> List(ListArgs<MarketplaceProduct> args, VerifiedUserContext user)
        {
            var _productsList = await _oc.Products.ListAsync<MarketplaceProduct>(
                filters: args.ToFilterString(),
                search: args.Search,
                pageSize: args.PageSize,
                page: args.Page,
                accessToken: user.AccessToken);
            var _superProductsList = new List<SuperMarketplaceProduct> { };
            await Throttler.RunAsync(_productsList.Items, 100, 10, async product => {
                var priceSchedule = await _oc.PriceSchedules.GetAsync(product.DefaultPriceScheduleID, user.AccessToken);
                var _specs = await _oc.Products.ListSpecsAsync(product.ID, null, null, null, 1, 100, null, user.AccessToken);
                var _variants = await _oc.Products.ListVariantsAsync<MarketplaceVariant>(product.ID, null, null, null, 1, 100, null, user.AccessToken);
                var _images = await GetProductImages(product.ID, "SEB");
                _superProductsList.Add(new SuperMarketplaceProduct
                {
                    Product = product,
                    PriceSchedule = priceSchedule,
                    Specs = _specs.Items,
                    Variants = _variants.Items,
                    Images = _images
                });
            });
            return new ListPage<SuperMarketplaceProduct>
            {
                Meta = _productsList.Meta,
                Items = _superProductsList
            };
        }

        public async Task<SuperMarketplaceProduct> Post(SuperMarketplaceProduct superProduct, VerifiedUserContext user)
        {
            var defaultSpecOptions = new List<DefaultOptionSpecAssignment>();
            // Create Specs
            var specRequests = await Throttler.RunAsync(superProduct.Specs, 100, 5, s =>
            {
                defaultSpecOptions.Add(new DefaultOptionSpecAssignment { SpecID = s.ID, OptionID = s.DefaultOptionID });
                s.DefaultOptionID = null;
                return _oc.Specs.SaveAsync<Spec>(s.ID, s, accessToken: user.AccessToken);
            });
            // Create Spec Options
            foreach (Spec spec in superProduct.Specs)
            {
                await Throttler.RunAsync(spec.Options, 100, 5, o => _oc.Specs.SaveOptionAsync(spec.ID, o.ID, o, accessToken: user.AccessToken));
            }
            // Patch Specs with requested DefaultOptionID
            await Throttler.RunAsync(defaultSpecOptions, 100, 10, a => _oc.Specs.PatchAsync(a.SpecID, new PartialSpec { DefaultOptionID = a.OptionID }, accessToken: user.AccessToken));
            // Create Price Schedule
            var _priceSchedule = await _oc.PriceSchedules.CreateAsync<PriceSchedule>(superProduct.PriceSchedule, user.AccessToken);
            // Create Product
            superProduct.Product.DefaultPriceScheduleID = _priceSchedule.ID;
            superProduct.Product.xp.Facets.Add("supplier", new List<string>() { user.SupplierID });
            var _product = await _oc.Products.CreateAsync<MarketplaceProduct>(superProduct.Product, user.AccessToken);
            // Make Spec Product Assignments
            await Throttler.RunAsync(superProduct.Specs, 100, 5, s => _oc.Specs.SaveProductAssignmentAsync(new SpecProductAssignment { ProductID = _product.ID, SpecID = s.ID }, accessToken: user.AccessToken));
            // Generate Variants
            await _oc.Products.GenerateVariantsAsync(_product.ID, accessToken: user.AccessToken);
            // Patch Variants with the User Specified ID(SKU) AND necessary display xp values
            await Throttler.RunAsync(superProduct.Variants, 100, 5, v =>
            {
                var oldVariantID = v.ID;
                v.ID = v.xp.NewID ?? v.ID;
                v.Name = v.xp.NewID ?? v.ID;
                return _oc.Products.PatchVariantAsync(_product.ID, oldVariantID, new PartialVariant { ID = v.ID, Name = v.Name, xp = v.xp }, accessToken: user.AccessToken);
            });
            // List Variants
            var _variants = await _oc.Products.ListVariantsAsync<MarketplaceVariant>(_product.ID, accessToken: user.AccessToken);
            // List Product Specs
            var _specs = await _oc.Products.ListSpecsAsync<Spec>(_product.ID, accessToken: user.AccessToken);
            // Return the SuperProduct
            return new SuperMarketplaceProduct
            {
                Product = _product,
                PriceSchedule = _priceSchedule,
                Specs = _specs.Items,
                Variants = _variants.Items,
                Images = new List<Asset>(),
            };
        }

        public async Task<SuperMarketplaceProduct> Put(string id, SuperMarketplaceProduct superProduct, VerifiedUserContext user)
        {
            // Two spec lists to compare (requestSpecs and existingSpecs)
            IList<Spec> requestSpecs = superProduct.Specs.ToList();
            IList<Spec> existingSpecs = (await _oc.Products.ListSpecsAsync(id, accessToken: user.AccessToken)).Items.ToList();
            // Two variant lists to compare (requestVariants and existingVariants)
            IList<MarketplaceVariant> requestVariants = superProduct.Variants;
            IList<Variant> existingVariants = (await _oc.Products.ListVariantsAsync(id, pageSize: 100, accessToken: user.AccessToken)).Items.ToList();
            // Calculate differences in specs - specs to add, and specs to delete
            var specsToAdd = requestSpecs.Where(s => !existingSpecs.Any(s2 => s2.ID == s.ID)).ToList();
            var specsToDelete = existingSpecs.Where(s => !requestSpecs.Any(s2 => s2.ID == s.ID)).ToList();
            // Get spec options to add -- WAIT ON THESE, RUN PARALLEL THE ADD AND DELETE SPEC REQUESTS
            foreach (var rSpec in requestSpecs)
            {
                foreach (var eSpec in existingSpecs)
                {
                    if (rSpec.ID == eSpec.ID)
                    {
                        await Throttler.RunAsync(rSpec.Options.Where(rso => !eSpec.Options.Any(eso => eso.ID == rso.ID)), 100, 5, o => _oc.Specs.CreateOptionAsync(rSpec.ID, o, accessToken: user.AccessToken));
                        await Throttler.RunAsync(eSpec.Options.Where(eso => !rSpec.Options.Any(rso => rso.ID == eso.ID)), 100, 5, o => _oc.Specs.DeleteOptionAsync(rSpec.ID, o.ID, accessToken: user.AccessToken));
                    }
                };
            };
            // Create new specs and Delete removed specs
            var defaultSpecOptions = new List<DefaultOptionSpecAssignment>();
            await Throttler.RunAsync(specsToAdd, 100, 5, s => {
                defaultSpecOptions.Add(new DefaultOptionSpecAssignment { SpecID = s.ID, OptionID = s.DefaultOptionID });
                s.DefaultOptionID = null;
                return _oc.Specs.SaveAsync<Spec>(s.ID, s, accessToken: user.AccessToken);
            });
            await Throttler.RunAsync(specsToDelete, 100, 5, s => _oc.Specs.DeleteAsync(s.ID, accessToken: user.AccessToken));
            // Add spec options for new specs
            foreach (var spec in specsToAdd)
            {
                await Throttler.RunAsync(spec.Options, 100, 5, o => _oc.Specs.CreateOptionAsync(spec.ID, o, accessToken: user.AccessToken));
            }
            // Patch Specs with requested DefaultOptionID
            await Throttler.RunAsync(defaultSpecOptions, 100, 10, a => _oc.Specs.PatchAsync(a.SpecID, new PartialSpec { DefaultOptionID = a.OptionID }, accessToken: user.AccessToken));
            // Make assignments for the new specs
            await Throttler.RunAsync(specsToAdd, 100, 5, s => _oc.Specs.SaveProductAssignmentAsync(new SpecProductAssignment { ProductID = id, SpecID = s.ID }, accessToken: user.AccessToken));
            // Check if Variants differ
            var variantsAdded = requestVariants.Any(v => !existingVariants.Any(v2 => v2.ID == v.ID));
            var variantsRemoved = existingVariants.Any(v => !requestVariants.Any(v2 => v2.ID == v.ID));
            // IF variants differ, then re-generate variants and re-patch IDs to match the user input.
            if (variantsAdded || variantsRemoved || requestVariants.Any(v => v.xp.NewID != null))
            {
                // Re-generate Variants
                await _oc.Products.GenerateVariantsAsync(id, overwriteExisting: true, accessToken: user.AccessToken);
                // Patch NEW variants with the User Specified ID (Name,ID), and correct xp values (SKU)
                await Throttler.RunAsync(superProduct.Variants, 100, 5, v =>
                {
                    var oldVariantID = v.ID;
                    v.ID = v.xp.NewID ?? v.ID;
                    v.Name = v.xp.NewID ?? v.ID;
                    return _oc.Products.PatchVariantAsync(id, oldVariantID, new PartialVariant { ID = v.ID, Name = v.Name, xp = v.xp }, accessToken: user.AccessToken);
                });
            };
            // Update the Product PriceSchedule
            var _updatedPriceSchedule = await _oc.PriceSchedules.SaveAsync<PriceSchedule>(superProduct.PriceSchedule.ID, superProduct.PriceSchedule, user.AccessToken);
            // Update the Product itself
            var _updatedProduct = await _oc.Products.SaveAsync<MarketplaceProduct>(superProduct.Product.ID, superProduct.Product, user.AccessToken);
            // List Variants
            var _variants = await _oc.Products.ListVariantsAsync<MarketplaceVariant>(id, pageSize: 100, accessToken: user.AccessToken);
            // List Product Specs
            var _specs = await _oc.Products.ListSpecsAsync<Spec>(id, accessToken: user.AccessToken);
            // List Product Images
            var _images = await GetProductImages(_updatedProduct.ID, "SEB");
            return new SuperMarketplaceProduct
            {
                Product = _updatedProduct,
                PriceSchedule = _updatedPriceSchedule,
                Specs = _specs.Items,
                Variants = _variants.Items,
                Images = _images,
            };
        }

        public async Task Delete(string id, VerifiedUserContext user)
        {
            var _specs = await _oc.Products.ListSpecsAsync<Spec>(id, accessToken: user.AccessToken);
            var _images = await _imgCommand.GetProductImages(id);
            // Delete specs and images associated with the requested product
            await Throttler.RunAsync(_images.Items, 100, 5, i => _imgCommand.Delete(i.id));
            await Throttler.RunAsync(_specs.Items, 100, 5, s => _oc.Specs.DeleteAsync(s.ID, accessToken: user.AccessToken));
            await _oc.Products.DeleteAsync(id, user.AccessToken);
        }
    }
}
