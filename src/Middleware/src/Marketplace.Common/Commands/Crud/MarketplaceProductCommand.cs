using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Mappers;
using Marketplace.Models;
using ordercloud.integrations.cms;
using ordercloud.integrations.library;
using ordercloud.integrations.library.Cosmos;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.Crud
{
	public interface IMarketplaceProductCommand
	{
		Task<SuperMarketplaceProduct> Get(string id, VerifiedUserContext user);
		Task<SuperMarketplaceProduct> MeGet(string id, VerifiedUserContext user);
		Task<ListPage<SuperMarketplaceProduct>> List(ListArgs<MarketplaceProduct> args, VerifiedUserContext user);
		Task<SuperMarketplaceProduct> Post(SuperMarketplaceProduct product, VerifiedUserContext user);
		Task<SuperMarketplaceProduct> Put(string id, SuperMarketplaceProduct product, VerifiedUserContext user);
		Task Delete(string id, VerifiedUserContext user);
		Task<MarketplacePriceSchedule> GetPricingOverride(string id, string buyerID, VerifiedUserContext user);
		Task DeletePricingOverride(string id, string buyerID, VerifiedUserContext user);
		Task<MarketplacePriceSchedule> UpdatePricingOverride(string id, string buyerID, MarketplacePriceSchedule pricingOverride, VerifiedUserContext user);
		Task<MarketplacePriceSchedule> CreatePricingOverride(string id, string buyerID, MarketplacePriceSchedule pricingOverride, VerifiedUserContext user);
	}

	public class DefaultOptionSpecAssignment
	{
		public string SpecID { get; set; }
		public string OptionID { get; set; }
	}
	public class MarketplaceProductCommand : IMarketplaceProductCommand
	{
		private readonly IOrderCloudClient _oc;
		private readonly IAssetedResourceQuery _assetedResources;
		private readonly IAssetQuery _assets;
		public MarketplaceProductCommand(AppSettings settings, IAssetedResourceQuery assetedResources, IAssetQuery assets, IOrderCloudClient elevatedOc)
		{
			_assetedResources = assetedResources;
			_assets = assets;
			_oc = elevatedOc;
		}

		public async Task<MarketplacePriceSchedule> GetPricingOverride(string id, string buyerID, VerifiedUserContext user)
		{
			var priceScheduleID = $"{id}-{buyerID}";
			var priceSchedule = await _oc.PriceSchedules.GetAsync<MarketplacePriceSchedule>(priceScheduleID);
			return priceSchedule;
		}
		
		public async Task DeletePricingOverride(string id, string buyerID, VerifiedUserContext user)
		{
			/* must remove the price schedule from the visibility assignments
			* deleting a price schedule with active visibility assignments removes the visbility
			* assignment completely, we want those product to usergroup catalog assignments to remain
		    * just without the override */
			var priceScheduleID = $"{id}-{buyerID}";
			await RemovePriceScheduleAssignmentFromProductCatalogAssignments(id, buyerID, priceScheduleID);
			await _oc.PriceSchedules.DeleteAsync(priceScheduleID);
		}

		public async Task<MarketplacePriceSchedule> CreatePricingOverride(string id, string buyerID, MarketplacePriceSchedule priceSchedule, VerifiedUserContext user)
		{
			/* must add the price schedule to the visibility assignments */
			var priceScheduleID = $"{id}-{buyerID}";
			priceSchedule.ID = priceScheduleID;
			var newPriceSchedule = await _oc.PriceSchedules.SaveAsync<MarketplacePriceSchedule>(priceScheduleID, priceSchedule);
			await AddPriceScheduleAssignmentToProductCatalogAssignments(id, buyerID, priceScheduleID);
			return newPriceSchedule;
		}
		
		public async Task<MarketplacePriceSchedule> UpdatePricingOverride(string id, string buyerID, MarketplacePriceSchedule priceSchedule, VerifiedUserContext user)
		{
			var priceScheduleID = $"{id}-{buyerID}";
			var newPriceSchedule = await _oc.PriceSchedules.SaveAsync<MarketplacePriceSchedule>(priceScheduleID, priceSchedule);
			return newPriceSchedule;
		}

		public async Task RemovePriceScheduleAssignmentFromProductCatalogAssignments(string productID, string buyerID, string priceScheduleID)
		{
			var relatedProductCatalogAssignments = await _oc.Products.ListAssignmentsAsync(productID: productID, buyerID: buyerID, pageSize: 100);
			await Throttler.RunAsync(relatedProductCatalogAssignments.Items, 100, 5, assignment =>
			{
				return _oc.Products.SaveAssignmentAsync(new ProductAssignment
				{
					BuyerID = buyerID,
					PriceScheduleID = null,
					ProductID = productID,
					UserGroupID = assignment.UserGroupID
				});
			});
		}

		public async Task AddPriceScheduleAssignmentToProductCatalogAssignments(string productID, string buyerID, string priceScheduleID)
		{
			var relatedProductCatalogAssignments = await _oc.Products.ListAssignmentsAsync(productID: productID, buyerID: buyerID, pageSize: 100);
			await Throttler.RunAsync(relatedProductCatalogAssignments.Items, 100, 5, assignment =>
			{
				return _oc.Products.SaveAssignmentAsync(new ProductAssignment
				{
					BuyerID = buyerID,
					PriceScheduleID = priceScheduleID,
					ProductID = productID,
					UserGroupID = assignment.UserGroupID
				});
			});
		}

		private async Task<List<AssetForDelivery>> GetProductImages(string productID, VerifiedUserContext user)
		{
			var assets = await _assetedResources.ListAssets(new Resource(ResourceType.Products, productID), user);
			var images = assets.Where(a => a.Type == AssetType.Image).ToList();
			return images;
		}
		private async Task<List<AssetForDelivery>> GetProductAttachments(string productID, VerifiedUserContext user)
		{
			var assets = await _assetedResources.ListAssets(new Resource(ResourceType.Products, productID), user);
			var attachments = assets.Where(a => a.Type == AssetType.Attachment).ToList();
			return attachments;
		}
		public async Task<SuperMarketplaceProduct> MeGet(string id, VerifiedUserContext user)
		{
			var _product = _oc.Me.GetProductAsync<BuyerProduct<ProductXp, PriceSchedule>>(id, user.AccessToken);
			var _specs = _oc.Me.ListSpecsAsync(id, null, null, user.AccessToken);
			var _variants = _oc.Products.ListVariantsAsync<MarketplaceVariant>(id, null, null, null, 1, 100, null);
			var _images = GetProductImages(id, user);
			var _attachments =  GetProductAttachments(id, user);
			return new SuperMarketplaceProduct
			{
				Product = ProductMapper.MeProductToProduct(await _product),
				PriceSchedule = (await _product).PriceSchedule,
				Specs = (await _specs).Items,
				Variants =(await _variants).Items,
				Images = await _images,
				Attachments = await _attachments
			};
		}

		public async Task<SuperMarketplaceProduct> Get(string id, VerifiedUserContext user)
		{
			var _product = await _oc.Products.GetAsync<MarketplaceProduct>(id, user.AccessToken);
			// Once data is good, switch away from DefaultPriceScheduleID. See comment in Delete().
			var _priceSchedule = _oc.PriceSchedules.GetAsync<PriceSchedule>(_product.DefaultPriceScheduleID, user.AccessToken); 
			var _specs = _oc.Products.ListSpecsAsync(id, null, null, null, 1, 100, null, user.AccessToken);
			var _variants = _oc.Products.ListVariantsAsync<MarketplaceVariant>(id, null, null, null, 1, 100, null, user.AccessToken);
			var _images = GetProductImages(id, user);
			var _attachments =  GetProductAttachments(id, user);
			return new SuperMarketplaceProduct
			{
				Product = _product,
				PriceSchedule = await _priceSchedule,
				Specs = (await _specs).Items,
				Variants = (await _variants).Items,
				Images = await _images,
				Attachments = await _attachments
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
			await Throttler.RunAsync(_productsList.Items, 100, 10, async product =>
			{
				var priceSchedule = _oc.PriceSchedules.GetAsync(product.DefaultPriceScheduleID, user.AccessToken);
				var _specs = _oc.Products.ListSpecsAsync(product.ID, null, null, null, 1, 100, null, user.AccessToken);
				var _variants = _oc.Products.ListVariantsAsync<MarketplaceVariant>(product.ID, null, null, null, 1, 100, null, user.AccessToken);
				var _images = GetProductImages(product.ID, user);
				var _attachments = GetProductAttachments(product.ID, user);
				_superProductsList.Add(new SuperMarketplaceProduct
				{
					Product = product,
					PriceSchedule = await priceSchedule,
					Specs = (await _specs).Items,
					Variants = (await _variants).Items,
					Images = await _images,
					Attachments = await _attachments
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
			// Determine ID up front so price schedule ID can match
			superProduct.Product.ID = superProduct.Product.ID ?? CosmosInteropID.New();
			// Create Specs
			var defaultSpecOptions = new List<DefaultOptionSpecAssignment>();
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
			superProduct.PriceSchedule.ID = superProduct.Product.ID;
			var _priceSchedule = await _oc.PriceSchedules.CreateAsync<PriceSchedule>(superProduct.PriceSchedule, user.AccessToken);
			// Create Product
			superProduct.Product.DefaultPriceScheduleID = _priceSchedule.ID;
			var supplierName = await GetSupplierNameForXpFacet(user.SupplierID, user.AccessToken);
			superProduct.Product.xp.Facets.Add("supplier", new List<string>() { supplierName });
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
				Images = new List<AssetForDelivery>(),
				Attachments = new List<AssetForDelivery>()
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
			await Throttler.RunAsync(specsToAdd, 100, 5, s =>
			{
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
			var _images = await GetProductImages(_updatedProduct.ID, user);
			// List Product Attachments
			var _attachments = await GetProductAttachments(_updatedProduct.ID, user);
			return new SuperMarketplaceProduct
			{
				Product = _updatedProduct,
				PriceSchedule = _updatedPriceSchedule,
				Specs = _specs.Items,
				Variants = _variants.Items,
				Images = _images,
				Attachments = _attachments
			};
		}

		public async Task Delete(string id, VerifiedUserContext user)
		{
			var product = await _oc.Products.GetAsync(id); // This is temporary to accommodate bad data where product.ID != product.DefaultPriceScheduleID
			var _specs = await _oc.Products.ListSpecsAsync<Spec>(id, accessToken: user.AccessToken);
			var _images = await GetProductImages(id, user);
			var _attachments = await GetProductAttachments(id, user);
			// Delete specs images and attachments associated with the requested product
			await Task.WhenAll(
				_oc.PriceSchedules.DeleteAsync(product.DefaultPriceScheduleID),
				Throttler.RunAsync(_images, 100, 5, i => _assets.Delete(i.InteropID, user)),
				Throttler.RunAsync(_attachments, 100, 5, i => _assets.Delete(i.InteropID, user)),
				Throttler.RunAsync(_specs.Items, 100, 5, s => _oc.Specs.DeleteAsync(s.ID, accessToken: user.AccessToken)),
				_oc.Products.DeleteAsync(id, user.AccessToken)
			);
		}

		private async Task<string> GetSupplierNameForXpFacet(string supplierID, string accessToken)
		{
			var supplier = await _oc.Suppliers.GetAsync(supplierID, accessToken);
			return supplier.Name;
		}
	}
}
