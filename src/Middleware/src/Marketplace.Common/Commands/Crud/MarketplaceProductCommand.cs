﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Extensions;
using Marketplace.Common.Helpers;
using Marketplace.Common.Services.CMS;
using Marketplace.Common.Services.CMS.Models;
using Marketplace.Models;
using ordercloud.integrations.library;
using ordercloud.integrations.library.Cosmos;
using ordercloud.integrations.library.helpers;
using OrderCloud.SDK;


namespace Marketplace.Common.Commands.Crud
{
	public interface IMarketplaceProductCommand
	{
		Task<SuperMarketplaceProduct> Get(string id, string token);
		Task<ListPage<SuperMarketplaceProduct>> List(ListArgs<MarketplaceProduct> args, string token);
		Task<SuperMarketplaceProduct> Post(SuperMarketplaceProduct product, VerifiedUserContext user);
		Task<SuperMarketplaceProduct> Put(string id, SuperMarketplaceProduct product, string token);
		Task Delete(string id, string token);
		Task<MarketplacePriceSchedule> GetPricingOverride(string id, string buyerID, string token);
		Task DeletePricingOverride(string id, string buyerID, string token);
		Task<MarketplacePriceSchedule> UpdatePricingOverride(string id, string buyerID, MarketplacePriceSchedule pricingOverride, string token);
		Task<MarketplacePriceSchedule> CreatePricingOverride(string id, string buyerID, MarketplacePriceSchedule pricingOverride, string token);
		Task<List<Asset>> GetProductImages(string productID, string token);
		Task<List<Asset>> GetProductAttachments(string productID, string token);
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
		private readonly ICMSClient _cms;
		private readonly AppSettings _settings;
		private readonly ISupplierApiClientHelper _apiClientHelper;
		public MarketplaceProductCommand(AppSettings settings, ICMSClient cms, IOrderCloudClient elevatedOc, ISupplierApiClientHelper apiClientHelper)
		{
			_cms = cms;
			_oc = elevatedOc;
			_settings = settings;
			_apiClientHelper = apiClientHelper;
		}

		public async Task<MarketplacePriceSchedule> GetPricingOverride(string id, string buyerID, string token)
		{
			var priceScheduleID = $"{id}-{buyerID}";
			var priceSchedule = await _oc.PriceSchedules.GetAsync<MarketplacePriceSchedule>(priceScheduleID);
			return priceSchedule;
		}

		public async Task DeletePricingOverride(string id, string buyerID, string token)
		{
			/* must remove the price schedule from the visibility assignments
			* deleting a price schedule with active visibility assignments removes the visbility
			* assignment completely, we want those product to usergroup catalog assignments to remain
		    * just without the override */
			var priceScheduleID = $"{id}-{buyerID}";
			await RemovePriceScheduleAssignmentFromProductCatalogAssignments(id, buyerID, priceScheduleID);
			await _oc.PriceSchedules.DeleteAsync(priceScheduleID);
		}

		public async Task<MarketplacePriceSchedule> CreatePricingOverride(string id, string buyerID, MarketplacePriceSchedule priceSchedule, string token)
		{
			/* must add the price schedule to the visibility assignments */
			var priceScheduleID = $"{id}-{buyerID}";
			priceSchedule.ID = priceScheduleID;
			var newPriceSchedule = await _oc.PriceSchedules.SaveAsync<MarketplacePriceSchedule>(priceScheduleID, priceSchedule);
			await AddPriceScheduleAssignmentToProductCatalogAssignments(id, buyerID, priceScheduleID);
			return newPriceSchedule;
		}

		public async Task<MarketplacePriceSchedule> UpdatePricingOverride(string id, string buyerID, MarketplacePriceSchedule priceSchedule, string token)
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

		public async Task<List<Asset>> GetProductImages(string productID, string token)
		{
			var assets = await _cms.Assets.ListAssets(ResourceType.Products, productID, new ListArgsPageOnly() { PageSize = 100 }, token);
			var images = assets.Items.Where(a => a.Type == AssetType.Image).ToList();
			return images;
		}
		public async Task<List<Asset>> GetProductAttachments(string productID, string token)
		{
			var assets = await _cms.Assets.ListAssets(ResourceType.Products, productID, new ListArgsPageOnly() { PageSize = 100 }, token);
			var attachments = assets.Items.Where(a => a.Title == "Product_Attachment").ToList();
			return attachments;
		}

		public async Task<SuperMarketplaceProduct> Get(string id, string token)
		{
			var _product = await _oc.Products.GetAsync<MarketplaceProduct>(id, token);
			// Get the price schedule, if it exists, if not - send empty price schedule
			var _priceSchedule = new PriceSchedule();
			try
			{
				_priceSchedule = await _oc.PriceSchedules.GetAsync<PriceSchedule>(_product.ID, token);
			} catch
			{
				_priceSchedule = new PriceSchedule();
			}
			var _specs = _oc.Products.ListSpecsAsync(id, null, null, null, 1, 100, null, token);
			var _variants = _oc.Products.ListVariantsAsync<MarketplaceVariant>(id, null, null, null, 1, 100, null, token);
			var _images = GetProductImages(id, token);
			var _attachments = GetProductAttachments(id, token);
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

		public async Task<ListPage<SuperMarketplaceProduct>> List(ListArgs<MarketplaceProduct> args, string token)
		{
			var _productsList = await _oc.Products.ListAsync<MarketplaceProduct>(
				filters: args.ToFilterString(),
				search: args.Search,
				searchType: SearchType.ExactPhrasePrefix,
				sortBy: args.SortBy.FirstOrDefault(),
				pageSize: args.PageSize,
				page: args.Page,
				accessToken: token);
			var _superProductsList = new List<SuperMarketplaceProduct> { };
			await Throttler.RunAsync(_productsList.Items, 100, 10, async product =>
			{
				var priceSchedule = _oc.PriceSchedules.GetAsync(product.DefaultPriceScheduleID, token);
				var _specs = _oc.Products.ListSpecsAsync(product.ID, null, null, null, 1, 100, null, token);
				var _variants = _oc.Products.ListVariantsAsync<MarketplaceVariant>(product.ID, null, null, null, 1, 100, null, token);
				var _images = GetProductImages(product.ID, token);
				var _attachments = GetProductAttachments(product.ID, token);
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
			//All products must have a price schedule for orders to be submitted.  The front end provides a default Price of $0 for quote products that don't have one.
			superProduct.PriceSchedule.ID = superProduct.Product.ID;
			try
            {
				_priceSchedule = await _oc.PriceSchedules.CreateAsync<PriceSchedule>(superProduct.PriceSchedule, user.AccessToken);
			}
			catch (OrderCloudException ex)
			{
				if (ex.HttpStatus == System.Net.HttpStatusCode.Conflict)
                {
					throw new Exception($"Product SKU {superProduct.PriceSchedule.ID} already exists.  Please try a different SKU.");
				}
			}
			superProduct.Product.DefaultPriceScheduleID = _priceSchedule.ID;
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
				return _oc.Products.PatchVariantAsync(_product.ID, oldVariantID, new PartialVariant { ID = v.ID, Name = v.Name, xp = v.xp, Inventory = v.Inventory }, accessToken: user.AccessToken);
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

		public async Task<SuperMarketplaceProduct> Put(string id, SuperMarketplaceProduct superProduct, string token)
		{
			// Update the Product itself
			var _updatedProduct = await _oc.Products.SaveAsync<MarketplaceProduct>(superProduct.Product.ID, superProduct.Product, token);
			// Two spec lists to compare (requestSpecs and existingSpecs)
			IList<Spec> requestSpecs = superProduct.Specs.ToList();
			IList<Spec> existingSpecs = (await _oc.Products.ListSpecsAsync(id, accessToken: token)).Items.ToList();
			// Two variant lists to compare (requestVariants and existingVariants)
			IList<MarketplaceVariant> requestVariants = superProduct.Variants;
			IList<Variant> existingVariants = (await _oc.Products.ListVariantsAsync(id, pageSize: 100, accessToken: token)).Items.ToList();
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
						await Throttler.RunAsync(rSpec.Options.Where(rso => !eSpec.Options.Any(eso => eso.ID == rso.ID)), 100, 5, o => _oc.Specs.CreateOptionAsync(rSpec.ID, o, accessToken: token));
						await Throttler.RunAsync(eSpec.Options.Where(eso => !rSpec.Options.Any(rso => rso.ID == eso.ID)), 100, 5, o => _oc.Specs.DeleteOptionAsync(rSpec.ID, o.ID, accessToken: token));
					}
				};
			};
			// Create new specs and Delete removed specs
			var defaultSpecOptions = new List<DefaultOptionSpecAssignment>();
			await Throttler.RunAsync(specsToAdd, 100, 5, s =>
			{
				defaultSpecOptions.Add(new DefaultOptionSpecAssignment { SpecID = s.ID, OptionID = s.DefaultOptionID });
				s.DefaultOptionID = null;
				return _oc.Specs.SaveAsync<Spec>(s.ID, s, accessToken: token);
			});
			await Throttler.RunAsync(specsToDelete, 100, 5, s => _oc.Specs.DeleteAsync(s.ID, accessToken: token));
			// Add spec options for new specs
			foreach (var spec in specsToAdd)
			{
				await Throttler.RunAsync(spec.Options, 100, 5, o => _oc.Specs.CreateOptionAsync(spec.ID, o, accessToken: token));
			}
			// Patch Specs with requested DefaultOptionID
			await Throttler.RunAsync(defaultSpecOptions, 100, 10, a => _oc.Specs.PatchAsync(a.SpecID, new PartialSpec { DefaultOptionID = a.OptionID }, accessToken: token));
			// Make assignments for the new specs
			await Throttler.RunAsync(specsToAdd, 100, 5, s => _oc.Specs.SaveProductAssignmentAsync(new SpecProductAssignment { ProductID = id, SpecID = s.ID }, accessToken: token));
			HandleSpecOptionChanges(requestSpecs, existingSpecs, token);
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
				await _oc.Products.GenerateVariantsAsync(id, overwriteExisting: true, accessToken: token);
				// Patch NEW variants with the User Specified ID (Name,ID), and correct xp values (SKU)
				await Throttler.RunAsync(superProduct.Variants, 100, 5, v =>
				{
					v.ID = v.xp.NewID ?? v.ID;
					v.Name = v.xp.NewID ?? v.ID;
					if ((superProduct?.Product?.Inventory?.VariantLevelTracking) != null && v.Inventory == null)
					{
						v.Inventory = new PartialVariantInventory { QuantityAvailable = 0 };
					}
					if (superProduct.Product?.Inventory == null)
					{
						//If Inventory doesn't exist on the product, don't patch variants with inventory either.
						return _oc.Products.PatchVariantAsync(id, $"{superProduct.Product.ID}-{v.xp.SpecCombo}", new PartialVariant { ID = v.ID, Name = v.Name, xp = v.xp, Active = v.Active }, accessToken: token);
					}
					else
					{
						return _oc.Products.PatchVariantAsync(id, $"{superProduct.Product.ID}-{v.xp.SpecCombo}", new PartialVariant { ID = v.ID, Name = v.Name, xp = v.xp, Active = v.Active, Inventory = v.Inventory }, accessToken: token);
					}
				});
			};
			// If applicable, update OR create the Product PriceSchedule
			var tasks = new List<Task>();
			Task<PriceSchedule> _priceScheduleReq = null;
            if (superProduct.PriceSchedule != null)
            {
                _priceScheduleReq = UpdateRelatedPriceSchedules(superProduct.PriceSchedule, token);
                tasks.Add(_priceScheduleReq);
            }
            // List Variants
            var _variantsReq = _oc.Products.ListVariantsAsync<MarketplaceVariant>(id, pageSize: 100, accessToken: token);
			tasks.Add(_variantsReq);
			// List Product Specs
			var _specsReq = _oc.Products.ListSpecsAsync<Spec>(id, accessToken: token);
			tasks.Add(_specsReq);
			// List Product Images
			var _imagesReq = GetProductImages(_updatedProduct.ID, token);
			tasks.Add(_imagesReq);
			// List Product Attachments
			var _attachmentsReq = GetProductAttachments(_updatedProduct.ID, token);
			tasks.Add(_attachmentsReq);

			await Task.WhenAll(tasks);

			return new SuperMarketplaceProduct
			{
				Product = _updatedProduct,
				PriceSchedule = _priceScheduleReq?.Result,
				Specs = _specsReq?.Result?.Items,
				Variants = _variantsReq?.Result?.Items,
				Images = _imagesReq?.Result,
				Attachments = _attachmentsReq?.Result
			};
		}

		private async Task<PriceSchedule> UpdateRelatedPriceSchedules(PriceSchedule updated, string token)
        {
			var initial = await _oc.PriceSchedules.GetAsync(updated.ID);
			if (initial.MaxQuantity != updated.MaxQuantity ||
				initial.MinQuantity != updated.MinQuantity ||
				initial.UseCumulativeQuantity != updated.UseCumulativeQuantity || 
				initial.RestrictedQuantity != updated.RestrictedQuantity || 
				initial.ApplyShipping != updated.ApplyShipping || 
				initial.ApplyTax != updated.ApplyTax)
            {
				var patch = new PartialPriceSchedule()
				{
					MinQuantity = updated.MinQuantity,
					MaxQuantity = updated.MaxQuantity,
					UseCumulativeQuantity = updated.UseCumulativeQuantity,
					RestrictedQuantity = updated.RestrictedQuantity,
					ApplyShipping = updated.ApplyShipping,
					ApplyTax = updated.ApplyTax
				};
				var relatedPriceSchedules = await ListAllAsync.List((page) => _oc.PriceSchedules.ListAsync(search: initial.ID, page: page, pageSize: 100));
				var priceSchedulesToUpdate = relatedPriceSchedules.Where(p => p.ID.StartsWith(updated.ID) && p.ID != updated.ID);
				await Throttler.RunAsync(priceSchedulesToUpdate, 100, 5, p =>
				{
					return _oc.PriceSchedules.PatchAsync(p.ID, patch, token);
				});
            }
			return await _oc.PriceSchedules.SaveAsync<PriceSchedule>(updated.ID, updated, token);

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
			if (variant.Inventory != currVariant.Inventory) { return true; }

			return false;
		}

		private async void HandleSpecOptionChanges(IList<Spec> requestSpecs, IList<Spec> existingSpecs, string token)
        {
			var requestSpecOptions = new Dictionary<string, List<SpecOption>>();
			var existingSpecOptions = new List<SpecOption>();
			foreach (Spec requestSpec in requestSpecs)
			{
				List<SpecOption> specOpts = new List<SpecOption>();
				foreach (SpecOption requestSpecOption in requestSpec.Options)
				{
					specOpts.Add(requestSpecOption);
				}
				requestSpecOptions.Add(requestSpec.ID, specOpts);
			}
			foreach (Spec existingSpec in existingSpecs)
            {
				foreach (SpecOption existingSpecOption in existingSpec.Options)
                {
					existingSpecOptions.Add(existingSpecOption);
				}
            }
			foreach (var spec in requestSpecOptions)
			{
				IList<SpecOption> changedSpecOptions = ChangedSpecOptions(spec.Value, existingSpecOptions);
				await Throttler.RunAsync(changedSpecOptions, 100, 5, option => _oc.Specs.SaveOptionAsync(spec.Key, option.ID, option, token));
			}
		}

        private IList<SpecOption> ChangedSpecOptions(List<SpecOption> requestOptions, List<SpecOption> existingOptions)
        {
            return requestOptions.FindAll(requestOption => OptionHasChanges(requestOption, existingOptions));
        }

        private bool OptionHasChanges(SpecOption requestOption, List<SpecOption> currentOptions)
        {
			var matchingOption = currentOptions.Find(currentOption => currentOption.ID == requestOption.ID);
			if (matchingOption == null) { return false; };
			if (matchingOption.PriceMarkup != requestOption.PriceMarkup) { return true; };
			if (matchingOption.IsOpenText != requestOption.IsOpenText) { return true; };
			if (matchingOption.ListOrder != requestOption.ListOrder) { return true; };
			if (matchingOption.PriceMarkupType != requestOption.PriceMarkupType) { return true; };

			return false;
        }

        public async Task Delete(string id, string token)
		{
			var product = await _oc.Products.GetAsync(id); // This is temporary to accommodate bad data where product.ID != product.DefaultPriceScheduleID
			var _specs = await _oc.Products.ListSpecsAsync<Spec>(id, accessToken: token);
			var _images = await GetProductImages(id, token);
			var _attachments = await GetProductAttachments(id, token);
			// Delete specs images and attachments associated with the requested product
			await Task.WhenAll(
				_oc.PriceSchedules.DeleteAsync(product.DefaultPriceScheduleID),
				Throttler.RunAsync(_images, 100, 5, i => _cms.Assets.Delete(i.ID, token)),
				Throttler.RunAsync(_attachments, 100, 5, i => _cms.Assets.Delete(i.ID, token)),
				Throttler.RunAsync(_specs.Items, 100, 5, s => _oc.Specs.DeleteAsync(s.ID, accessToken: token)),
				_oc.Products.DeleteAsync(id, token)
			);
		}

		public async Task<Product> FilterOptionOverride(string id, string supplierID, IDictionary<string, object> facets, VerifiedUserContext user)
		{
			
			ApiClient supplierClient = await _apiClientHelper.GetSupplierApiClient(supplierID, user.AccessToken);
			if (supplierClient == null) { throw new Exception($"Default supplier client not found. SupplierID: {supplierID}"); }
			var configToUse = new OrderCloudClientConfig
			{
				ApiUrl = user.ApiUrl,
				AuthUrl = user.AuthUrl,
				ClientId = supplierClient.ID,
				ClientSecret = supplierClient.ClientSecret,
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
