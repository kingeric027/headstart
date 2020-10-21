using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dynamitey;
using Marketplace.Common.Models.Marketplace;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
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
    }

    public class CheckoutIntegrationCommand : ICheckoutIntegrationCommand
    {
        private readonly IAvalaraCommand _avalara;
        private readonly IEasyPostShippingService _shippingService;
        private readonly IExchangeRatesCommand _exchangeRates;
        private readonly IOrderCloudClient _oc;
        private readonly AppSettings _settings;
        public CheckoutIntegrationCommand(IAvalaraCommand avalara, IExchangeRatesCommand exchangeRates, IOrderCloudClient orderCloud, IEasyPostShippingService shippingService, AppSettings settings)
        {
			_avalara = avalara;
            _exchangeRates = exchangeRates;
            _oc = orderCloud;
            _shippingService = shippingService;
            _settings = settings;
        }

        public async Task<ShipEstimateResponse> GetRatesAsync(MarketplaceOrderCalculatePayload orderCalculatePayload)
        {
            var worksheet = orderCalculatePayload.OrderWorksheet;
            var excludePOProducts = orderCalculatePayload.ConfigData.ExcludePOProductsFromShipping;
            if (excludePOProducts)
			{
                worksheet.LineItems = worksheet.LineItems.Where(li => li.Product.xp.ProductType != ProductType.PurchaseOrder).ToList(); ;
            }

            var groupedLineItems = orderCalculatePayload.OrderWorksheet.LineItems.GroupBy(li => new AddressPair { ShipFrom = li.ShipFromAddress, ShipTo = li.ShippingAddress }).ToList();
            var accounts = new [] { _settings.EasyPostSettings.ProvisionFedexAccountId, _settings.EasyPostSettings.SEBDistributionFedexAccountId, _settings.EasyPostSettings.SMGFedexAccountId };
            var shipResponse = await _shippingService.GetRates(groupedLineItems, accounts); // include all accounts at this stage so we can save on order worksheet and analyze 

            // Certain suppliers use certain shipping accounts. This filters available rates based on those accounts.  
            for (int i = 0; i < groupedLineItems.Count; i++)
            {
                var supplierID = groupedLineItems[i].First().SupplierID;
                var methods = shipResponse.ShipEstimates[i].ShipMethods.Where(s => s.xp.CarrierAccountID == GetShippingAccountForSupplier(supplierID));
                shipResponse.ShipEstimates[i].ShipMethods = WhereRateIsCheapestOfItsKind(methods).Select(s =>
                {
                    if (s.xp.CarrierAccountID == _settings.EasyPostSettings.SEBDistributionFedexAccountId)
                    {
                        s.Cost = s.Cost * (decimal)1.1; //  Apply markup for the SEBDistributionFedexAccount
                    }
                    else if (s.xp.CarrierAccountID == _settings.EasyPostSettings.SMGFedexAccountId)
                    {
                        s.Cost = s.Cost * (decimal)1.4;
                    }
                    return s;
                }).ToList();
            }
            var buyerCurrency = worksheet.Order.xp.Currency ?? CurrencySymbol.USD;

            if (buyerCurrency != CurrencySymbol.USD) // shipper currency is USD
            {
                shipResponse.ShipEstimates = await ConvertShippingRatesCurrency(shipResponse.ShipEstimates, CurrencySymbol.USD, buyerCurrency); 
            }

            shipResponse.ShipEstimates = await ApplyFreeShipping(worksheet, shipResponse.ShipEstimates);

            return shipResponse;
        }

        private string GetShippingAccountForSupplier(string supplierID)
		{
            if (supplierID == _settings.OrderCloudSettings.ProvisionSupplierID)
			{
                return _settings.EasyPostSettings.ProvisionFedexAccountId;

            } else if (supplierID == _settings.OrderCloudSettings.SEBDistributionSupplierID) 
            {
                return _settings.EasyPostSettings.SEBDistributionFedexAccountId;
            } else
			{
                return _settings.EasyPostSettings.SMGFedexAccountId;
            }
		}

        public static IEnumerable<ShipMethod> WhereRateIsCheapestOfItsKind(IEnumerable<ShipMethod> methods)
		{
            return methods
                .GroupBy(method => method.EstimatedTransitDays)
                .Select(kind => kind.OrderBy(method => method.Cost).First());
        }

        private async Task<List<ShipEstimate>> ConvertShippingRatesCurrency(IList<ShipEstimate> shipEstimates, CurrencySymbol shipperCurrency, CurrencySymbol buyerCurrency)
		{
            var rates = (await _exchangeRates.Get(buyerCurrency)).Rates;
            var conversionRate = rates.Find(r => r.Currency == shipperCurrency).Rate;
            return shipEstimates.Select(estimate =>
            {
                estimate.ShipMethods = estimate.ShipMethods.Select(method =>
                {
                    var convertedCost = method.Cost / (decimal)conversionRate;
                    method.xp = new
                    {
                        OriginalShipCost = method.Cost,
                        OriginalCurrency = shipperCurrency.ToString(),
                        ExchangeRate = conversionRate,
                        OrderCurrency = buyerCurrency.ToString()
                    };
                    method.Cost = convertedCost;
                    return method;
                }).ToList();
                return estimate;
            }).ToList();
        }

        private async Task<List<ShipEstimate>> ApplyFreeShipping(MarketplaceOrderWorksheet orderWorksheet, IList<ShipEstimate> shipEstimates)
        {
            var supplierIDs = orderWorksheet.LineItems.Select(li => li.SupplierID);
            var suppliers = await _oc.Suppliers.ListAsync<MarketplaceSupplier>(filters: $"ID={string.Join("|", supplierIDs)}");
            var updatedEstimates = new List<ShipEstimate>();

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
                        if (method.Name.Contains("GROUND")) //  free shipping on ground shipping
                        {
                            method.xp.FreeShippingApplied = true;
                            method.xp.FreeShippingThreshold = supplier.xp.FreeShippingThreshold;
                            method.xp.CostBeforeDiscount = method.Cost;
                            method.Cost = 0;
                        }
                    }
                }
                updatedEstimates.Add(estimate);
                
            }
            return updatedEstimates;
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

