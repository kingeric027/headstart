using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Constants;
using Marketplace.Common.Extensions;
using Marketplace.Common.Models;
using Marketplace.Common.Models.Marketplace;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using Microsoft.EntityFrameworkCore.Design;
using ordercloud.integrations.avalara;
using ordercloud.integrations.easypost;
using ordercloud.integrations.exchangerates;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands
{
    public interface ICheckoutIntegrationCommand
    {
        Task<ShipEstimateResponse> GetRatesAsync(MarketplaceOrderCalculatePayload orderCalculatePayload);
        Task<MarketplaceOrderCalculateResponse> CalculateOrder(MarketplaceOrderCalculatePayload orderCalculatePayload);
        Task<MarketplaceOrderCalculateResponse> CalculateOrder(string orderID, VerifiedUserContext user);
        Task<ShipEstimateResponse> GetRatesAsync(string orderID);
    }

    public class CheckoutIntegrationCommand : ICheckoutIntegrationCommand
    {
        private readonly IAvalaraCommand _avalara;
        private readonly IEasyPostShippingService _shippingService;
        private readonly IExchangeRatesCommand _exchangeRates;
        private readonly IOrderCloudClient _oc;
        private readonly SelfEsteemBrandsShippingProfiles _profiles;
        private readonly AppSettings _settings;
  
        public CheckoutIntegrationCommand(IAvalaraCommand avalara, IExchangeRatesCommand exchangeRates, IOrderCloudClient orderCloud, IEasyPostShippingService shippingService, AppSettings settings)
        {
			_avalara = avalara;
            _exchangeRates = exchangeRates;
            _oc = orderCloud;
            _shippingService = shippingService;
            _settings = settings;
            _profiles = new SelfEsteemBrandsShippingProfiles(_settings);
        }

        public async Task<ShipEstimateResponse> GetRatesAsync(MarketplaceOrderCalculatePayload orderCalculatePayload)
        {
            return await this.GetRatesAsync(orderCalculatePayload.OrderWorksheet, orderCalculatePayload.ConfigData);
        }

        public async Task<ShipEstimateResponse> GetRatesAsync(string orderID)
        {
            var order = await _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Incoming, orderID);
            return await this.GetRatesAsync(order);
        }

        private async Task<ShipEstimateResponse> GetRatesAsync(MarketplaceOrderWorksheet worksheet, CheckoutIntegrationConfiguration config = null)
        {
            if (config != null && config.ExcludePOProductsFromShipping)
            {
                worksheet.LineItems = worksheet.LineItems.Where(li => li.Product.xp.ProductType != ProductType.PurchaseOrder).ToList();
            }
            var groupedLineItems = worksheet.LineItems.GroupBy(li => new AddressPair { ShipFrom = li.ShipFromAddress, ShipTo = li.ShippingAddress }).ToList();
            var shipResponse = (await _shippingService.GetRates(groupedLineItems, _profiles)).Reserialize<MarketplaceShipEstimateResponse>(); // include all accounts at this stage so we can save on order worksheet and analyze 

            // Certain suppliers use certain shipping accounts. This filters available rates based on those accounts.  
            for (var i = 0; i < groupedLineItems.Count; i++)
            {
                var supplierID = groupedLineItems[i].First().SupplierID;
                var methods = shipResponse.ShipEstimates[i].ShipMethods.Where(s => s.xp.CarrierAccountID == _profiles.FirstOrDefault(supplierID).CarrierAccountID);
                var cheapestMethods = WhereRateIsCheapestOfItsKind(methods);
                shipResponse.ShipEstimates[i].ShipMethods = cheapestMethods.Select(s =>
                {
                    // apply a 75% markup to keyfob shipments https://four51.atlassian.net/browse/SEB-1260
                    if (groupedLineItems[i].Any(li => li.Product.xp.ProductType == ProductType.PurchaseOrder))
                    {
                        s.Cost = Math.Round(s.Cost * (decimal)1.75, 2);
                    } else
                    {
                        s.Cost = Math.Min((s.xp.OriginalCost * _profiles.ShippingProfiles.First(p => p.CarrierAccountID == s.xp?.CarrierAccountID).Markup), s.xp.ListRate);
                    }
                    return s;
                }).ToList();
            }
            var buyerCurrency = worksheet.Order.xp.Currency ?? CurrencySymbol.USD;


            if (buyerCurrency != CurrencySymbol.USD) // shipper currency is USD
                shipResponse.ShipEstimates = await ConvertShippingRatesCurrency(shipResponse.ShipEstimates, CurrencySymbol.USD, buyerCurrency);
            shipResponse.ShipEstimates = CheckForEmptyRates(shipResponse.ShipEstimates, _settings.EasyPostSettings.NoRatesFallbackCost, _settings.EasyPostSettings.NoRatesFallbackTransitDays);
            shipResponse.ShipEstimates = UpdateFreeShippingRates(shipResponse.ShipEstimates, _settings.EasyPostSettings.FreeShippingTransitDays);
            shipResponse.ShipEstimates = await ApplyFreeShipping(worksheet, shipResponse.ShipEstimates);
            shipResponse.ShipEstimates = FilterSlowerRatesWithHighCost(shipResponse.ShipEstimates);
            
            return shipResponse;
        }

        public static IEnumerable<MarketplaceShipMethod> WhereRateIsCheapestOfItsKind(IEnumerable<MarketplaceShipMethod> methods)
		{
            return methods
                .GroupBy(method => method.EstimatedTransitDays)
                .Select(kind => kind.OrderBy(method => method.Cost).First());
        }

        private async Task<List<MarketplaceShipEstimate>> ConvertShippingRatesCurrency(IList<MarketplaceShipEstimate> shipEstimates, CurrencySymbol shipperCurrency, CurrencySymbol buyerCurrency)
		{
            var rates = (await _exchangeRates.Get(buyerCurrency)).Rates;
            var conversionRate = rates.Find(r => r.Currency == shipperCurrency).Rate;
            return shipEstimates.Select(estimate =>
            {
                estimate.ShipMethods = estimate.ShipMethods.Select(method =>
                {
                    method.xp.OriginalCurrency = shipperCurrency;
                    method.xp.OrderCurrency = buyerCurrency;
                    method.xp.ExchangeRate = conversionRate;
                    if (conversionRate != null) method.Cost /= (decimal) conversionRate;
                    return method;
                }).ToList();
                return estimate;
            }).ToList();
        }

        private async Task<List<MarketplaceShipEstimate>> ApplyFreeShipping(MarketplaceOrderWorksheet orderWorksheet, IList<MarketplaceShipEstimate> shipEstimates)
        {
            var supplierIDs = orderWorksheet.LineItems.Select(li => li.SupplierID);
            var suppliers = await _oc.Suppliers.ListAsync<MarketplaceSupplier>(filters: $"ID={string.Join("|", supplierIDs)}");
            var updatedEstimates = new List<MarketplaceShipEstimate>();

            foreach(var estimate in shipEstimates)
            {
                //  get supplier and supplier subtotal
                var supplierID = orderWorksheet.LineItems.First(li => li.ID == estimate.ShipEstimateItems.FirstOrDefault()?.LineItemID).SupplierID;
                var supplier = suppliers.Items.FirstOrDefault(s => s.ID == supplierID);
                var supplierLineItems = orderWorksheet.LineItems.Where(li => li.SupplierID == supplier?.ID);
                var supplierSubTotal = supplierLineItems.Select(li => li.LineSubtotal).Sum();
                if (supplier?.xp?.FreeShippingThreshold != null && supplier.xp?.FreeShippingThreshold < supplierSubTotal) // free shipping for this supplier
                {
                    foreach (var method in estimate.ShipMethods)
                    {
                        // free shipping on ground shipping or orders where we weren't able to calculate a shipping rate
                        if (method.Name.Contains("GROUND") || method.ID == "NO_SHIPPING_RATES")
                        {
                            method.xp.FreeShippingApplied = true;
                            method.xp.FreeShippingThreshold = supplier.xp.FreeShippingThreshold;
                            method.xp.CostBeforeDiscount = method.Cost; // do we need this? xp.OriginalCost already exists
                            method.Cost = 0;
                        }
                    }
                }
                updatedEstimates.Add(estimate);
                
            }
            return updatedEstimates;
        }

        public static IList<MarketplaceShipEstimate> FilterSlowerRatesWithHighCost(IList<MarketplaceShipEstimate> estimates)
        {
            // filter out rate estimates with slower transit days and higher costs than faster transit days
            // ex: 3 days for $20 vs 1 day for $10. Filter out the 3 days option

            var result = estimates.Select(estimate =>
            {
                var methodsList = estimate.ShipMethods.OrderBy(m => m.EstimatedTransitDays).ToList();
                var filtered = new List<MarketplaceShipMethod>();
                for (var i = methodsList.Count - 1; i >= 0; i--)
                {
                    var method = methodsList[i];
                    var fasterMethods = methodsList.GetRange(0, i);
                    var existsFasterCheaperRate = fasterMethods.Any(m => m.Cost < method.Cost);
                    if (!existsFasterCheaperRate)
                    {
                        filtered.Add(method);
                    }
                }
                filtered.Reverse(); // reorder back to original since we looped backwards
                estimate.ShipMethods = filtered;
                return estimate;
            }).ToList();

            return result;
        }

        public static IList<MarketplaceShipEstimate> CheckForEmptyRates(IList<MarketplaceShipEstimate> estimates, decimal noRatesCost, int noRatesTransitDays)
        {
            // if there are no rates for a set of line items then return a mocked response so user can check out
            // this order will additionally get marked as needing attention

            foreach (var shipEstimate in estimates)
            {
                if (!shipEstimate.ShipMethods.Any())
                {
                    shipEstimate.ShipMethods = new List<MarketplaceShipMethod>()
                    {
                        new MarketplaceShipMethod
                        {
                            ID = "NO_SHIPPING_RATES",
                            Name = "No shipping rates",
                            Cost = noRatesCost,
                            EstimatedTransitDays = noRatesTransitDays,
                            xp = new ShipMethodXP
                            {
                                OriginalCost = noRatesCost
                            }
                        }
                    };
                }
            }
            return estimates;
        }

        public static IList<MarketplaceShipEstimate> UpdateFreeShippingRates(IList<MarketplaceShipEstimate> estimates, int freeShippingTransitDays)
        {
            foreach (var shipEstimate in estimates)
            {
                if (shipEstimate.ID == ShippingConstants.FreeShipping)
                {
                    foreach (var method in shipEstimate.ShipMethods)
                    {
                        method.ID = ShippingConstants.FreeShipping;
                        method.Cost = 0;
                        method.EstimatedTransitDays = freeShippingTransitDays;
                    }
                }
            }
            return estimates;
        }

        public async Task<MarketplaceOrderCalculateResponse> CalculateOrder(string orderID, VerifiedUserContext user)
        {
            var worksheet = await _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Incoming, orderID, user.AccessToken);
            return await this.CalculateOrder(new MarketplaceOrderCalculatePayload()
            {
                ConfigData = null,
                OrderWorksheet = worksheet
            });
        }

        public async Task<MarketplaceOrderCalculateResponse> CalculateOrder(MarketplaceOrderCalculatePayload orderCalculatePayload)
        {
            if(orderCalculatePayload.OrderWorksheet.Order.xp != null && orderCalculatePayload.OrderWorksheet.Order.xp.OrderType == OrderType.Quote)
            {
                // quote orders do not have tax cost associated with them
                return new MarketplaceOrderCalculateResponse();
            } else
            {
                var totalTax = await _avalara.GetEstimateAsync(orderCalculatePayload.OrderWorksheet.Reserialize<OrderWorksheet>());

                return new MarketplaceOrderCalculateResponse
                {
                    TaxTotal = totalTax.totalTax ?? 0,
                    xp = new OrderCalculateResponseXp()
                    {
                        TaxResponse = totalTax
                    }
                };
            }
        }
    }
}

