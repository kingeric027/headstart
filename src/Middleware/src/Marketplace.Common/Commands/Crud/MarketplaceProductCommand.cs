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
            var specRequests = superProduct.Specs.Select(spec => _oc.Specs.SaveAsync<Spec>(spec.ID, spec, accessToken: user.AccessToken)).ToList();
            await Task.WhenAll(specRequests);
            // Create Spec Options
            var specOptions = new List<SpecOption>() { };
            foreach (Spec spec in superProduct.Specs)
            {
                var specOptionRequests = spec.Options.Select(option => _oc.Specs.SaveOptionAsync(spec.ID, option.ID, option, accessToken: user.AccessToken));
                await Task.WhenAll(specOptionRequests);
            }
            // Create Price Schedule
            var _priceSchedule = await _oc.PriceSchedules.CreateAsync<PriceSchedule>(superProduct.PriceSchedule, user.AccessToken);
            // Create Product - WORKING
            superProduct.Product.DefaultPriceScheduleID = _priceSchedule.ID;
            var _product = await _oc.Products.CreateAsync<MarketplaceProduct>(superProduct.Product, user.AccessToken);
            // Make Spec Product Assignments
            var specProductAssignmentRequests = superProduct.Specs.Select(spec => _oc.Specs.SaveProductAssignmentAsync(new SpecProductAssignment { ProductID = _product.ID, SpecID = spec.ID }, accessToken: user.AccessToken));
            await Task.WhenAll(specProductAssignmentRequests);
            // Generate Variants
            await _oc.Products.GenerateVariantsAsync(_product.ID, accessToken: user.AccessToken);
            // Patch Variants with the User Specified ID(SKU)
            if (superProduct.Variants.Any(v => v.xp.NewID != null))
            {
                var createVariantRequests = superProduct.Variants.Select(variant => 
                {
                    var oldVariantID = variant.ID;
                    variant.ID = variant.xp.NewID ?? variant.ID;
                    variant.Name = variant.xp.NewID ?? variant.ID;
                    return _oc.Products.PatchVariantAsync(_product.ID, oldVariantID, new PartialVariant { ID = variant.ID, Name = variant.Name, xp = variant.xp }, accessToken: user.AccessToken);
                });
                await Task.WhenAll(createVariantRequests);
            }
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
                        var optionsToAdd = rSpec.Options.Where(rso => !eSpec.Options.Any(eso => eso.ID == rso.ID)).Select(opt => _oc.Specs.CreateOptionAsync(rSpec.ID, opt, accessToken: user.AccessToken));
                        var optionsToDelete = eSpec.Options.Where(eso => !rSpec.Options.Any(rso => rso.ID == eso.ID)).Select(opt => _oc.Specs.DeleteOptionAsync(rSpec.ID, opt.ID, accessToken: user.AccessToken));
                        var allOptionRequests = optionsToDelete.Concat(optionsToAdd);
                        await Task.WhenAll(allOptionRequests);
                    }
                };
            };
            // Create new specs and Delete removed specs
            var allSpecRequests = specsToAdd.Select(spec => _oc.Specs.CreateAsync(spec, accessToken: user.AccessToken)).Concat(specsToDelete.Select(spec => _oc.Specs.DeleteAsync(spec.ID, accessToken: user.AccessToken)));
            await Task.WhenAll(allSpecRequests);
            // Add spec options for new specs
            foreach (var spec in specsToAdd)
            {
                var createOptionRequests = spec.Options.Select(opt => _oc.Specs.CreateOptionAsync(spec.ID, opt, accessToken: user.AccessToken));
                await Task.WhenAll(createOptionRequests);
            }
            // Make assignments for the new specs
            var newSpecAssignments = specsToAdd.Select(spec => _oc.Specs.SaveProductAssignmentAsync(new SpecProductAssignment { ProductID = id, SpecID = spec.ID }, accessToken: user.AccessToken));
            await Task.WhenAll(newSpecAssignments);
            // Check if Variants differ
            var variantsAdded = requestVariants.Any(v => !existingVariants.Any(v2 => v2.ID == v.ID));
            var variantsRemoved = existingVariants.Any(v => !requestVariants.Any(v2 => v2.ID == v.ID));
            // IF variants differ, then re-generate variants and re-patch IDs to match the user input.
            if (variantsAdded || variantsRemoved || requestVariants.Any(v => v.xp.NewID != null))
            {
                // Re-generate Variants
                await _oc.Products.GenerateVariantsAsync(id, overwriteExisting: true, accessToken: user.AccessToken);
                // Patch NEW variants with the User Specified ID (Name,ID), and correct xp values (SKU)
                var createVariantRequests = superProduct.Variants.Select(variant =>
                {
                    var oldVariantID = variant.ID;
                    variant.ID = variant.xp.NewID ?? variant.ID;
                    variant.Name = variant.xp.NewID ?? variant.ID;
                    return _oc.Products.PatchVariantAsync(id, oldVariantID, new PartialVariant { ID = variant.ID, Name = variant.Name, xp = variant.xp }, accessToken: user.AccessToken);
                });
                await Task.WhenAll(createVariantRequests);
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
