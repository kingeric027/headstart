using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Models;
using Newtonsoft.Json;
using ordercloud.integrations.cms;
using ordercloud.integrations.library;
using ordercloud.integrations.library.Cosmos;
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
		Task<MarketplacePriceSchedule> GetPricingOverride(string id, string buyerID, VerifiedUserContext user);
		Task DeletePricingOverride(string id, string buyerID, VerifiedUserContext user);
		Task<MarketplacePriceSchedule> UpdatePricingOverride(string id, string buyerID, MarketplacePriceSchedule pricingOverride, VerifiedUserContext user);
		Task<MarketplacePriceSchedule> CreatePricingOverride(string id, string buyerID, MarketplacePriceSchedule pricingOverride, VerifiedUserContext user);
		Task<List<Asset>> GetProductImages(string productID, VerifiedUserContext user);
		Task<List<Asset>> GetProductAttachments(string productID, VerifiedUserContext user);
		Task<Product> FilterOptionOverride(string id, string supplierID, IDictionary<string, object> facets, VerifiedUserContext user);
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
		private readonly AppSettings _settings;
		public MarketplaceProductCommand(AppSettings settings, IAssetedResourceQuery assetedResources, IAssetQuery assets, IOrderCloudClient elevatedOc)
		{
			_assetedResources = assetedResources;
			_assets = assets;
			_oc = elevatedOc;
			_settings = settings;
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

		public async Task<List<Asset>> GetProductImages(string productID, VerifiedUserContext user)
		{
			var assets = await _assetedResources.ListAssets(new Resource(ResourceType.Products, productID), new ListArgsPageOnly() { PageSize = 100 }, user);
			var images = assets.Items.Where(a => a.Type == AssetType.Image).ToList();
			return images;
		}
		public async Task<List<Asset>> GetProductAttachments(string productID, VerifiedUserContext user)
		{
			var assets = await _assetedResources.ListAssets(new Resource(ResourceType.Products, productID), new ListArgsPageOnly() { PageSize = 100 }, user);
			var attachments = assets.Items.Where(a => a.Type == AssetType.Attachment).ToList();
			return attachments;
		}

		public async Task<SuperMarketplaceProduct> Get(string id, VerifiedUserContext user)
		{
			var _product = await _oc.Products.GetAsync<MarketplaceProduct>(id, user.AccessToken);
			// Get the price schedule, if it exists, if not - send empty price schedule
			var _priceSchedule = new PriceSchedule();
			try
			{
				_priceSchedule = await _oc.PriceSchedules.GetAsync<PriceSchedule>(_product.ID, user.AccessToken);
			} catch
			{
				_priceSchedule = new PriceSchedule();
			}
			var _specs = _oc.Products.ListSpecsAsync(id, null, null, null, 1, 100, null, user.AccessToken);
			var _variants = _oc.Products.ListVariantsAsync<MarketplaceVariant>(id, null, null, null, 1, 100, null, user.AccessToken);
			var _images = GetProductImages(id, user);
			var _attachments = GetProductAttachments(id, user);
			try
			{
				return new SuperMarketplaceProduct
				{
					Product = _product,
					PriceSchedule = _priceSchedule,
					Specs = (await _specs).Items,
					Variants = (await _variants).Items,
					Images = await _images,
					Attachments = await _attachments
				};
			} catch (Exception e)
			{
				throw e;
			}
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
			PriceSchedule _priceSchedule = null;
			// If the superProduct has a price schedule, create
			if (superProduct.PriceSchedule != null)
			{
				superProduct.PriceSchedule.ID = superProduct.Product.ID;
				_priceSchedule = await _oc.PriceSchedules.CreateAsync<PriceSchedule>(superProduct.PriceSchedule, user.AccessToken);
				superProduct.Product.DefaultPriceScheduleID = _priceSchedule.ID;
			}
			// Create Product
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
				Images = new List<Asset>(),
				Attachments = new List<Asset>()
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
			bool hasVariantChange = false;

			foreach(Variant variant in requestVariants)
            {
				var currVariant = existingVariants.Where(v => v.ID == variant.ID);
				if (currVariant == null || currVariant.Count() < 1) { continue; }
				hasVariantChange = HasVariantChange(variant, currVariant.First());
				if (hasVariantChange) { break; }
            }
			// IF variants differ, then re-generate variants and re-patch IDs to match the user input.
			if (variantsAdded || variantsRemoved || hasVariantChange || requestVariants.Any(v => v.xp.NewID != null))
			{
				// Re-generate Variants
				await _oc.Products.GenerateVariantsAsync(id, overwriteExisting: true, accessToken: user.AccessToken);
				// Patch NEW variants with the User Specified ID (Name,ID), and correct xp values (SKU)
				await Throttler.RunAsync(superProduct.Variants, 100, 5, v =>
				{
					v.ID = v.xp.NewID ?? v.ID;
					v.Name = v.xp.NewID ?? v.ID;
					return _oc.Products.PatchVariantAsync(id, $"{superProduct.Product.ID}-{v.xp.SpecCombo}", new PartialVariant { ID = v.ID, Name = v.Name, xp = v.xp, Active = v.Active }, accessToken: user.AccessToken);
				});
			};
			// If applicable, update OR create the Product PriceSchedule
			PriceSchedule _priceSchedule = null;
			if (superProduct.PriceSchedule != null)
			{
				_priceSchedule = await _oc.PriceSchedules.SaveAsync<PriceSchedule>(superProduct.PriceSchedule.ID, superProduct.PriceSchedule, user.AccessToken);
			}
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
				PriceSchedule = _priceSchedule,
				Specs = _specs.Items,
				Variants = _variants.Items,
				Images = _images,
				Attachments = _attachments
			};
		}

        private bool HasVariantChange(Variant variant, Variant currVariant)
        {
            if (variant.Active != currVariant.Active) { return true; }
			if (variant.Description != currVariant.Description) { return true; }
			if (variant.Name != currVariant.Name) { return true; }
			if (variant.ShipHeight != currVariant.ShipHeight) { return true; }
			if (variant.ShipLength != currVariant.ShipLength) { return true; }
			if (variant.ShipWeight != currVariant.ShipWeight) { return true; }
			if (variant.ShipWidth != currVariant.ShipWidth) { return true; }

			return false;
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
				Throttler.RunAsync(_images, 100, 5, i => _assets.Delete(i.ID, user)),
				Throttler.RunAsync(_attachments, 100, 5, i => _assets.Delete(i.ID, user)),
				Throttler.RunAsync(_specs.Items, 100, 5, s => _oc.Specs.DeleteAsync(s.ID, accessToken: user.AccessToken)),
				_oc.Products.DeleteAsync(id, user.AccessToken)
			);
		}

		public async Task<Product> FilterOptionOverride(string id, string supplierID, IDictionary<string, object> facets, VerifiedUserContext user)
		{
			//Use supplier integrations client with a DefaultContextUserName to access a supplier token.  
			//All suppliers have integration clients with a default user of dev_{supplierID}.
			var supplierClient = await _oc.ApiClients.ListAsync(filters: $"DefaultContextUserName=dev_{supplierID}");
			var selectedSupplierClient = supplierClient.Items[0];
			var configToUse = new OrderCloudClientConfig
			{
				ApiUrl = user.ApiUrl,
				AuthUrl = user.AuthUrl,
				ClientId = selectedSupplierClient.ID,
				ClientSecret = selectedSupplierClient.ClientSecret,
				GrantType = GrantType.ClientCredentials,
				Roles = new[]
						   {
								 ApiRole.SupplierAdmin,
								 ApiRole.ProductAdmin
							},

			};
			var ocClient = new OrderCloudClient(configToUse);
			await ocClient.AuthenticateAsync();
			var token = ocClient.TokenResponse.AccessToken;

			//Format the facet data to change for request body
			var facetDataFormatted = new ExpandoObject();
			var facetDataFormattedCollection = (ICollection<KeyValuePair<string, object>>) facetDataFormatted;
			foreach (var kvp in facets)
            {
				facetDataFormattedCollection.Add(kvp);
            }
			dynamic facetDataFormattedDynamic = facetDataFormatted;

			//Update the product with a supplier token
			var updatedProduct = await ocClient.Products.PatchAsync(
				id,
				new PartialProduct() {  xp = new { Facets = facetDataFormattedDynamic }},
				accessToken: token
				);
            return updatedProduct;
        }

		private async Task<string> GetSupplierNameForXpFacet(string supplierID, string accessToken)
		{
			var supplier = await _oc.Suppliers.GetAsync(supplierID, accessToken);
			return supplier.Name;
		}
	}
}
