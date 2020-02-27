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
            var createVariantRequests = superProduct.Variants.Select(variant => 
            {
                var oldVariantID = variant.ID;
                variant.ID = variant.xp.NewID ?? variant.ID;
                variant.Name = variant.xp.NewID ?? variant.ID;
                return _oc.Products.SaveVariantAsync(_product.ID, oldVariantID, variant, accessToken: user.AccessToken);
            });
            await Task.WhenAll(createVariantRequests);
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
            // Two spec lists to compare
            IList<Spec> requestSpecs = superProduct.Specs;
            IList<Variant<MarketplaceVariantXp>> requestVariants = superProduct.Variants;
            IList<Spec> existingSpecs = (await _oc.Products.ListSpecsAsync(id, accessToken: user.AccessToken)).Items.ToList();
            IList<Variant> existingVariants = (await _oc.Products.ListVariantsAsync(id, pageSize: 100, accessToken: user.AccessToken)).Items.ToList();
            var specsToAdd = requestSpecs.Where(s => !existingSpecs.Any(s2 => s2.ID == s.ID)).ToList();
            var specsToDelete = existingSpecs.Where(s => !requestSpecs.Any(s2 => s2.ID == s.ID)).ToList();
            // Create new specs
            var addSpecRequests = specsToAdd.Select(spec => _oc.Specs.CreateAsync(spec, accessToken: user.AccessToken));
            await Task.WhenAll(addSpecRequests);
            // Add spec options for new specs
            foreach (var spec in specsToAdd)
            {
                var createOptionRequests = spec.Options.Select(opt => _oc.Specs.CreateOptionAsync(spec.ID, opt, accessToken: user.AccessToken));
                await Task.WhenAll(createOptionRequests);
            }
            // Make assignments for the new specs
            var newSpecAssignments = specsToAdd.Select(spec => _oc.Specs.SaveProductAssignmentAsync(new SpecProductAssignment { ProductID = id, SpecID = spec.ID }, accessToken: user.AccessToken));
            await Task.WhenAll(newSpecAssignments);
            // Delete removed specs
            var removeSpecProductAssignments = specsToDelete.Select(spec => _oc.Specs.DeleteProductAssignmentAsync(spec.ID, id, accessToken: user.AccessToken));
            await Task.WhenAll(removeSpecProductAssignments);
            var deleteSpecRequests = specsToDelete.Select(spec => _oc.Specs.DeleteAsync(spec.ID, accessToken: user.AccessToken));
            await Task.WhenAll(deleteSpecRequests);
            var _updatedPriceSchedule = await _oc.PriceSchedules.SaveAsync<PriceSchedule>(superProduct.PriceSchedule.ID, superProduct.PriceSchedule, user.AccessToken);
            var _updatedProduct = await _oc.Products.SaveAsync<MarketplaceProduct>(superProduct.Product.ID, superProduct.Product, user.AccessToken);
            // Check if Variants differ
            var variantsAdded = requestVariants.Where(v => !existingVariants.Any(v2 => v2.ID == v.ID)).ToList();
            var variantsRemoved = existingVariants.Where(v => !requestVariants.Any(v2 => v2.ID == v.ID)).ToList();
            // IF variants differ, then re-generate variants and re-patch IDs to match the user input.
            if (variantsAdded.Count > 0 || variantsRemoved.Count > 0)
            {
                // Generate Variants
                await _oc.Products.GenerateVariantsAsync(id, overwriteExisting: true, accessToken: user.AccessToken);
                // Patch Variants with the User Specified ID (SKU)
                var createVariantRequests = superProduct.Variants.Select(variant =>
                {
                    var oldVariantID = variant.ID;
                    variant.ID = variant.xp.NewID ?? variant.ID;
                    variant.Name = variant.xp.NewID ?? variant.ID;
                    return _oc.Products.SaveVariantAsync(id, oldVariantID, variant, accessToken: user.AccessToken);
                });
                await Task.WhenAll(createVariantRequests);
            };
            // List Variants
            var _variants = await _oc.Products.ListVariantsAsync<Variant<MarketplaceVariantXp>>(id, accessToken: user.AccessToken);
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
