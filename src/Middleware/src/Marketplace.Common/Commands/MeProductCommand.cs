using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Commands.Crud;
using Marketplace.Models;
using ordercloud.integrations.exchangerates;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands
{
	public interface IMeProductCommand
	{
		Task<ListPageWithFacets<MarketplaceMeProduct>> List(ListArgs<MarketplaceMeProduct> args, VerifiedUserContext user);
		Task<SuperMarketplaceMeProduct> Get(string id, VerifiedUserContext user);
	}

	public class MeProductCommand : IMeProductCommand
	{
		private readonly IOrderCloudClient _oc;
		private readonly IMarketplaceBuyerCommand _marketplaceBuyerCommand;
		private readonly IExchangeRatesCommand _exchangeRatesCommand;
		private readonly IMarketplaceProductCommand _marketplaceProductCommand;
		public MeProductCommand(IOrderCloudClient elevatedOc, IMarketplaceBuyerCommand marketplaceBuyerCommand, IExchangeRatesCommand exchangeRatesCommand, IMarketplaceProductCommand marketplaceProductCommand)
		{
			_oc = elevatedOc;
			_marketplaceBuyerCommand = marketplaceBuyerCommand;
			_exchangeRatesCommand = exchangeRatesCommand;
			_marketplaceProductCommand = marketplaceProductCommand;
		}
		public async Task<SuperMarketplaceMeProduct> Get(string id, VerifiedUserContext user)
		{
			var _product = _oc.Me.GetProductAsync<MarketplaceMeProduct>(id, user.AccessToken);
			var _specs = _oc.Me.ListSpecsAsync(id, null, null, user.AccessToken);
			var _variants = _oc.Products.ListVariantsAsync<MarketplaceVariant>(id, null, null, null, 1, 100, null);
			var _images = _marketplaceProductCommand.GetProductImages(id, user);
			var _attachments = _marketplaceProductCommand.GetProductAttachments(id, user);
			var unconvertedSuperMarketplaceProduct = new SuperMarketplaceMeProduct 
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

		private async Task<SuperMarketplaceMeProduct> ApplyBuyerPricing(SuperMarketplaceMeProduct superMarketplaceProduct, VerifiedUserContext user)
		{
			var defaultMarkupMultiplierRequest = GetDefaultMarkupMultiplier(user);
			var exchangeRatesRequest = GetExchangeRates(user);

			var defaultMarkupMultiplier = await defaultMarkupMultiplierRequest;
			var exchangeRates = await exchangeRatesRequest;

			var markedupProduct = ApplyBuyerProductPricing(superMarketplaceProduct.Product, defaultMarkupMultiplier, exchangeRates);
			var productCurrency = superMarketplaceProduct.Product.xp.Currency;
			var markedupSpecs = ApplySpecMarkups(superMarketplaceProduct.Specs.ToList(), defaultMarkupMultiplier, productCurrency, exchangeRates);
		
			superMarketplaceProduct.Product = markedupProduct;
			superMarketplaceProduct.Specs = markedupSpecs;
			return superMarketplaceProduct;
		}

		private List<Spec> ApplySpecMarkups(List<Spec> specs, decimal defaultMarkupMultiplier, CurrencySymbol? productCurrency, List<OrderCloudIntegrationsConversionRate> exchangeRates)
		{
			return specs.Select(spec =>
			{
				spec.Options = spec.Options.Select(option =>
				{
					if (option.PriceMarkup != null)
					{
						var unconvertedMarkup = option.PriceMarkup ?? 0;
						option.PriceMarkup = ConvertPrice(unconvertedMarkup, productCurrency, exchangeRates);
					}
					return option;
				}).ToList();
				return spec;
			}).ToList();
		}

		public async Task<ListPageWithFacets<MarketplaceMeProduct>> List(ListArgs<MarketplaceMeProduct> args, VerifiedUserContext user)
		{
			var searchText = args.Search ?? "";
			var meProductsRequest = searchText.Length > 0 ? _oc.Me.ListProductsAsync<MarketplaceMeProduct>(filters: args.ToFilterString(), page: args.Page, search: searchText, accessToken: user.AccessToken) : _oc.Me.ListProductsAsync<MarketplaceMeProduct>(filters: args.ToFilterString(), page: args.Page, accessToken: user.AccessToken);

			var defaultMarkupMultiplierRequest = GetDefaultMarkupMultiplier(user);
			var exchangeRatesRequest = GetExchangeRates(user);

			var meProducts = await meProductsRequest;
			var defaultMarkupMultiplier = await defaultMarkupMultiplierRequest;
			var exchangeRates = await exchangeRatesRequest;

			meProducts.Items = meProducts.Items.Select(product => ApplyBuyerProductPricing(product, defaultMarkupMultiplier, exchangeRates)).ToList();

			return meProducts;
		}

		private MarketplaceMeProduct ApplyBuyerProductPricing(MarketplaceMeProduct product, decimal defaultMarkupMultiplier, List<OrderCloudIntegrationsConversionRate> exchangeRates)
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
						var convertedPrice = ConvertPrice(markedupPrice, product.xp.Currency, exchangeRates);
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
						priceBreak.Price = ConvertPrice(priceBreak.Price, CurrencySymbol.USD, exchangeRates);
						return priceBreak;
					}).ToList();
				}
			}
			return product;
		}

		private decimal ConvertPrice(decimal defaultPrice, CurrencySymbol? productCurrency, List<OrderCloudIntegrationsConversionRate> exchangeRates)
		{
			var exchangeRateForProduct = exchangeRates.Find(e => e.Currency == productCurrency).Rate;
			return defaultPrice * (decimal)exchangeRateForProduct;
		}

		private async Task<decimal> GetDefaultMarkupMultiplier(VerifiedUserContext user)
		{
			var buyer = await _marketplaceBuyerCommand.Get(user.BuyerID);

			// must convert markup to decimal before division to prevent rouding error
			var markupPercent = (decimal)buyer.Markup.Percent / 100;
			var markupMultiplier = markupPercent + 1;
			return markupMultiplier;
		}

		private async Task<List<OrderCloudIntegrationsConversionRate>> GetExchangeRates(VerifiedUserContext user)
		{
			var buyerUserGroups = await _oc.Me.ListUserGroupsAsync<MarketplaceLocationUserGroup>(opts => opts.AddFilter(u => u.xp.Type == "BuyerLocation"), user.AccessToken);
			var currency = buyerUserGroups.Items.First(u => u.xp.Currency != null).xp.Currency;
			Require.That(currency != null, new ErrorCode("Exchange Rate Error", 400, "Exchange Rate Not Defined For User"));

			// ideally shouldn't need to do this, to get around problem with optional type
			CurrencySymbol currencyRequired = currency.Reserialize<CurrencySymbol>();
			var exchangeRates = await _exchangeRatesCommand.Get(new ListArgs<OrderCloudIntegrationsConversionRate>() { }, currencyRequired);

			return exchangeRates.Items.ToList();
		}
		
	}
}
