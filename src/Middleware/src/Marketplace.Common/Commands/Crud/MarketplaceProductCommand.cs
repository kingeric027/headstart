using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public class MarketplaceProductCommand : IMarketplaceProductCommand
    {
        private readonly IOrderCloudClient _oc;
        public MarketplaceProductCommand(AppSettings settings)
        {
            _oc = new OrderCloudClient(new OrderCloudClientConfig()
            {
                ApiUrl = settings.OrderCloudSettings.ApiUrl,
                AuthUrl = settings.OrderCloudSettings.AuthUrl,
            });
        }

        public async Task<SuperMarketplaceProduct> Get(string id, VerifiedUserContext user)
        {
            var _product =  await _oc.Products.GetAsync<MarketplaceProduct>(id, user.AccessToken);
            var _priceSchedule = await _oc.PriceSchedules.GetAsync<PriceSchedule>(_product.DefaultPriceScheduleID, user.AccessToken);
            var _specs = await _oc.Products.ListSpecsAsync(id, null, null, null, 1, 100, null, user.AccessToken);
            var _variants = await _oc.Products.ListVariantsAsync<Variant<MarketplaceVariantXp>>(id, null, null, null, 1, 100, null, user.AccessToken);
        
            return new SuperMarketplaceProduct
            {
                Product = _product,
                PriceSchedule = _priceSchedule,
                Specs = _specs.Items,
                Variants = _variants.Items
            };
        }

        public async Task<ListPage<SuperMarketplaceProduct>> List(ListArgs<MarketplaceProduct> args, VerifiedUserContext user)
        {
            var _productsList =  await _oc.Products.ListAsync<MarketplaceProduct>(filters: args, accessToken: user.AccessToken);
            var _superProductsList = new List<SuperMarketplaceProduct> { };
            foreach (MarketplaceProduct product in _productsList.Items)
            {
                var priceSchedule = await _oc.PriceSchedules.GetAsync(product.DefaultPriceScheduleID, user.AccessToken);
                var _specs = await _oc.Products.ListSpecsAsync(product.ID, null, null, null, 1, 100, null, user.AccessToken);
                var _variants = await _oc.Products.ListVariantsAsync<Variant<MarketplaceVariantXp>>(product.ID, null, null, null, 1, 100, null, user.AccessToken);
                _superProductsList.Add(new SuperMarketplaceProduct
                {
                    Product = product,
                    PriceSchedule = priceSchedule,
                    Specs = _specs.Items,
                    Variants = _variants.Items
                });
            }
            return new ListPage<SuperMarketplaceProduct>
            {
                Meta = _productsList.Meta,
                Items = _superProductsList
            };
        }

        public async Task<SuperMarketplaceProduct> Post(SuperMarketplaceProduct superProduct, VerifiedUserContext user)
        {
            // Create Specs
            var specRequests = await Throttler.RunAsync(superProduct.Specs, 100, 5, s => _oc.Specs.SaveAsync<Spec>(s.ID, s, accessToken: user.AccessToken));
            // Create Spec Options
            foreach (Spec spec in superProduct.Specs)
            {
                await Throttler.RunAsync(spec.Options, 100, 5, o => _oc.Specs.SaveOptionAsync(spec.ID, o.ID, o, accessToken: user.AccessToken));
            }
            // Create Price Schedule
            var _priceSchedule = await _oc.PriceSchedules.CreateAsync<PriceSchedule>(superProduct.PriceSchedule, user.AccessToken);
            // Create Product
            superProduct.Product.DefaultPriceScheduleID = _priceSchedule.ID;
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
            var _variants = await _oc.Products.ListVariantsAsync<Variant<MarketplaceVariantXp>>(_product.ID, accessToken: user.AccessToken);
            // List Product Specs
            var _specs = await _oc.Products.ListSpecsAsync<Spec>(_product.ID, accessToken: user.AccessToken);
            // Return the SuperProduct
            return new SuperMarketplaceProduct
            {
                Product = _product,
                PriceSchedule = _priceSchedule,
                Specs = _specs.Items,
                Variants = _variants.Items,
            };
        }

        public async Task<SuperMarketplaceProduct> Put(string id, SuperMarketplaceProduct superProduct, VerifiedUserContext user)
        {
            // Two spec lists to compare (requestSpecs and existingSpecs)
            IList<Spec> requestSpecs = superProduct.Specs.ToList();
            IList<Spec> existingSpecs = (await _oc.Products.ListSpecsAsync(id, accessToken: user.AccessToken)).Items.ToList();
            // Two variant lists to compare (requestVariants and existingVariants)
            IList<Variant<MarketplaceVariantXp>> requestVariants = superProduct.Variants;
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
            await Throttler.RunAsync(specsToAdd, 100, 5, s => _oc.Specs.CreateAsync(s, accessToken: user.AccessToken));
            await Throttler.RunAsync(specsToDelete, 100, 5, s => _oc.Specs.DeleteAsync(s.ID, accessToken: user.AccessToken));
            // Add spec options for new specs
            foreach (var spec in specsToAdd)
            {
                await Throttler.RunAsync(spec.Options, 100, 5, o => _oc.Specs.CreateOptionAsync(spec.ID, o, accessToken: user.AccessToken));
            }
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
            var _variants = await _oc.Products.ListVariantsAsync<Variant<MarketplaceVariantXp>>(id, pageSize: 100, accessToken: user.AccessToken);
            // List Product Specs
            var _specs = await _oc.Products.ListSpecsAsync<Spec>(id, accessToken: user.AccessToken);
            return new SuperMarketplaceProduct
            {
                Product = _updatedProduct,
                PriceSchedule = _updatedPriceSchedule,
                Specs = _specs.Items,
                Variants = _variants.Items,
            };
        }

        public async Task Delete(string id, VerifiedUserContext user)
        {
            await _oc.Products.DeleteAsync(id, user.AccessToken);
        }
    }
}
