using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LazyCache;
using Headstart.Common.Commands.Crud;
using Headstart.Common.Models.Marketplace;
using Headstart.Common.Services;
using Headstart.Models;
using Headstart.Models.Misc;
using ordercloud.integrations.exchangerates;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Headstart.Common.Commands
{
	public interface IMeProductCommand
	{
		Task<ListPageWithFacets<HSMeProduct>> List(ListArgs<HSMeProduct> args, VerifiedUserContext user);
		Task<SuperHSMeProduct> Get(string id, VerifiedUserContext user);
		Task RequestProductInfo(ContactSupplierBody template);
		Task<HSMeKitProduct> ApplyBuyerPricing(HSMeKitProduct kitProduct, VerifiedUserContext user);
	}

	public class MeProductCommand : IMeProductCommand
	{
		private readonly IOrderCloudClient _oc;
		private readonly IHSBuyerCommand _marketplaceBuyerCommand;
		private readonly IHSProductCommand _marketplaceProductCommand;
		private readonly ISendgridService _sendgridService;
		private readonly IAppCache _cache;
		public MeProductCommand(
			IOrderCloudClient elevatedOc, 
			IHSBuyerCommand marketplaceBuyerCommand,
			IHSProductCommand marketplaceProductCommand,
			ISendgridService sendgridService,
			IAppCache cache
		)
		{
			_oc = elevatedOc;
			_marketplaceBuyerCommand = marketplaceBuyerCommand;
			_marketplaceProductCommand = marketplaceProductCommand;
			_sendgridService = sendgridService;
			_cache = cache;
		}
		public async Task<SuperHSMeProduct> Get(string id, VerifiedUserContext user)
		{
			var _product = _oc.Me.GetProductAsync<HSMeProduct>(id, user.AccessToken);
			var _specs = _oc.Me.ListSpecsAsync(id, null, null, user.AccessToken);
			var _variants = _oc.Products.ListVariantsAsync<HSVariant>(id, null, null, null, 1, 100, null);
			var _images = _marketplaceProductCommand.GetProductImages(id, user.AccessToken);
			var _attachments = _marketplaceProductCommand.GetProductAttachments(id, user.AccessToken);
			var unconvertedSuperMarketplaceProduct = new SuperHSMeProduct 
			{
				Product = await _product,
				PriceSchedule = (await _product).PriceSchedule,
				Specs = (await _specs).Items,
				Variants = (await _variants).Items,
				Images = await _images,
				Attachments = await _attachments
			};
			return await ApplyBuyerPricing(unconvertedSuperMarketplaceProduct, user);
		}

		private async Task<SuperHSMeProduct> ApplyBuyerPricing(SuperHSMeProduct superMarketplaceProduct, VerifiedUserContext user)
		{
			var defaultMarkupMultiplierRequest = GetDefaultMarkupMultiplier(user);

			var defaultMarkupMultiplier = await defaultMarkupMultiplierRequest;

			var markedupProduct = ApplyBuyerProductPricing(superMarketplaceProduct.Product, defaultMarkupMultiplier);
			var productCurrency = (Nullable<CurrencySymbol>)superMarketplaceProduct.Product.xp.Currency;
			var markedupSpecs = ApplySpecMarkups(superMarketplaceProduct.Specs.ToList(), defaultMarkupMultiplier, (Nullable<CurrencySymbol>)productCurrency);
		
			superMarketplaceProduct.Product = markedupProduct;
			superMarketplaceProduct.Specs = markedupSpecs;
			return superMarketplaceProduct;
		}

		public async Task<HSMeKitProduct> ApplyBuyerPricing(HSMeKitProduct kitProduct, VerifiedUserContext user)
		{
			var defaultMarkupMultiplierRequest = GetDefaultMarkupMultiplier(user);

			var defaultMarkupMultiplier = await defaultMarkupMultiplierRequest;

			foreach(var kit in kitProduct.ProductAssignments.ProductsInKit)
            {
				var markedupProduct = ApplyBuyerProductPricing(kit.Product, defaultMarkupMultiplier);
				var productCurrency = (Nullable<CurrencySymbol>)kit.Product.xp.Currency;
				var markedupSpecs = ApplySpecMarkups(kit.Specs.ToList(), defaultMarkupMultiplier, (Nullable<CurrencySymbol>)productCurrency);
				kit.Product = markedupProduct;
				kit.Specs = markedupSpecs;
			}

			return kitProduct;
		}

		private List<Spec> ApplySpecMarkups(List<Spec> specs, decimal defaultMarkupMultiplier, CurrencySymbol? productCurrency)
		{
			return specs.Select(spec =>
			{
				spec.Options = spec.Options.Select(option =>
				{
					if (option.PriceMarkup != null)
					{
						var unconvertedMarkup = option.PriceMarkup ?? 0;
						option.PriceMarkup = ConvertPrice(unconvertedMarkup, (Nullable<CurrencySymbol>)productCurrency);
					}
					return option;
				}).ToList();
				return spec;
			}).ToList();
		}

		public async Task<ListPageWithFacets<HSMeProduct>> List(ListArgs<HSMeProduct> args, VerifiedUserContext user)
		{
			var searchText = args.Search ?? "";
			var searchFields = args.Search!=null ? "ID,Name,Description,xp.Facets.supplier" : "";
			var sortBy = args.SortBy.FirstOrDefault();
			var meProducts = await _oc.Me.ListProductsAsync<HSMeProduct>(filters: args.ToFilterString(), page: args.Page, search: searchText, searchOn: searchFields, searchType: SearchType.ExactPhrasePrefix, sortBy: sortBy,  accessToken: user.AccessToken);
			if(!(bool)(meProducts?.Items?.Any()))
            {
				meProducts = await _oc.Me.ListProductsAsync<HSMeProduct>(filters: args.ToFilterString(), page: args.Page, search: searchText, searchOn: searchFields, searchType: SearchType.AnyTerm, sortBy: sortBy, accessToken: user.AccessToken);
				if (!(bool)(meProducts?.Items?.Any()))
                {
					//if no products after retry search, avoid making extra calls for pricing details
					return meProducts;
                }
			}

			var defaultMarkupMultiplier = await GetDefaultMarkupMultiplier(user);
			meProducts.Items = meProducts.Items.Select(product => ApplyBuyerProductPricing(product, defaultMarkupMultiplier)).ToList();

			return meProducts;
		}

		public async Task RequestProductInfo(ContactSupplierBody template)
        {
			await _sendgridService.SendContactSupplierAboutProductEmail(template);
        }

		private HSMeProduct ApplyBuyerProductPricing(HSMeProduct product, decimal defaultMarkupMultiplier)
		{
			
			if(product.PriceSchedule != null)
            {
				/* if the price schedule Id matches the product ID we 
				 * we mark up the produc
				 * if they dont match we just convert for currecny as the 
				 * seller has set custom pricing */
				var shouldMarkupProduct = product.PriceSchedule.ID == product.ID;
				if (shouldMarkupProduct)
				{
					product.PriceSchedule.PriceBreaks = product.PriceSchedule.PriceBreaks.Select(priceBreak =>
					{
						var markedupPrice = priceBreak.Price * defaultMarkupMultiplier;
						var currency = product?.xp?.Currency ?? CurrencySymbol.USD;
						var convertedPrice = ConvertPrice(markedupPrice, currency);
						priceBreak.Price = convertedPrice;
						return priceBreak;
					}).ToList();
				}
				else
				{
					product.PriceSchedule.PriceBreaks = product.PriceSchedule.PriceBreaks.Select(priceBreak =>
					{
						// price on price schedule will be in USD as it is set by the seller
						// may be different rates in the future
						// refactor to save price on the price schedule not product xp?
						var currency = (Nullable<CurrencySymbol>)CurrencySymbol.USD;
						priceBreak.Price = ConvertPrice(priceBreak.Price, currency);
						return priceBreak;
					}).ToList();
				}
			}
			return product;
		}

        private decimal ConvertPrice(decimal defaultPrice, CurrencySymbol? productCurrency, List<OrderCloudIntegrationsConversionRate> exchangeRates = null)
        {
            var exchangeRateForProduct = exchangeRates.Find(e => e.Currency == productCurrency).Rate;
            return defaultPrice / (decimal)(exchangeRateForProduct ?? 1);
        }

        private async Task<decimal> GetDefaultMarkupMultiplier(VerifiedUserContext user)
		{
			var buyer = await _cache.GetOrAddAsync($"buyer_{user.BuyerID}", () => _marketplaceBuyerCommand.Get(user.BuyerID), TimeSpan.FromHours(1));

			// must convert markup to decimal before division to prevent rouding error
			var markupPercent = (decimal)buyer.Markup.Percent / 100;
			var markupMultiplier = markupPercent + 1;
			return markupMultiplier;
		}
	}
}
