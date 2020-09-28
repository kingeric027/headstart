using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Services.ShippingIntegration.Mappers;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models.Exceptions;
using Marketplace.Models.Extended;
using OrderCloud.SDK;
using static Marketplace.Models.ErrorCodes;
using ordercloud.integrations.avalara;
using ordercloud.integrations.library;
using ordercloud.integrations.freightpop;
using ordercloud.integrations.exchangerates;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;

namespace Marketplace.Common.Services.ShippingIntegration
{
    public interface IOCShippingIntegration
    {
        Task<ShipEstimateResponse> GetRatesAsync(OrderCalculatePayload orderCalculatePayload);
        Task<OrderCalculateResponse> CalculateOrder(OrderCalculatePayload<MarketplaceOrderWorksheet> orderCalculatePayload);
    }

    public class OCShippingIntegration : IOCShippingIntegration
    {
        readonly IFreightPopService _freightPopService;
        private readonly IAvalaraCommand _avalara;
        private readonly IExchangeRatesCommand _exchangeRates;
        private readonly IOrderCloudClient _oc;
        public OCShippingIntegration(IFreightPopService freightPopService, IAvalaraCommand avalara, IExchangeRatesCommand exchangeRates, IOrderCloudClient orderCloud)
        {
            _freightPopService = freightPopService;
			_avalara = avalara;
            _exchangeRates = exchangeRates;
            _oc = orderCloud;
        }

        public async Task<ShipEstimateResponse> GetRatesAsync(OrderCalculatePayload orderCalculatePayload)
        {

            var orderWorksheet = orderCalculatePayload.OrderWorksheet;
            var lineItemsForShippingEstimates = GetLineItemsToIncludeInShipping(orderCalculatePayload.OrderWorksheet.LineItems, orderCalculatePayload.ConfigData);

            var proposedShipmentRequests = ShipmentEstimateRequestsMapper.Map(lineItemsForShippingEstimates);
            proposedShipmentRequests = proposedShipmentRequests.Select(proposedShipmentRequest =>
            {
                proposedShipmentRequest.RateResponseTask = _freightPopService.GetRatesAsync(proposedShipmentRequest.RateRequestBody);
                return proposedShipmentRequest;
            }).ToList();

            var tasks = proposedShipmentRequests.Select(p => p.RateResponseTask);
            await Task.WhenAll(tasks);
            CurrencySymbol orderCurrency = (CurrencySymbol)Enum.Parse(typeof(CurrencySymbol), orderWorksheet.Order.xp.Currency);
            var rates = (await _exchangeRates.Get(orderCurrency)).Rates;
            var shipEstimates = proposedShipmentRequests.Select(proposedShipmentRequest => ShipmentEstimateMapper.Map(proposedShipmentRequest, orderCurrency, rates)).ToList();
            var shipEstimatesWithFreeShippingApplied = await ApplyFreeShipping(orderWorksheet, shipEstimates);

            return new ShipEstimateResponse()
            {
                ShipEstimates = shipEstimatesWithFreeShippingApplied as IList<ShipEstimate>
            };
        }

        private async Task<List<ShipEstimate>> ApplyFreeShipping(OrderWorksheet orderWorksheet, List<ShipEstimate> shipEstimates)
        {
            var supplierIDs = orderWorksheet.LineItems.Select(li => li.SupplierID);
            var suppliers = await _oc.Suppliers.ListAsync<MarketplaceSupplier>(filters: $"ID={string.Join("|", supplierIDs)}");
            var updatedEstimates = new List<ShipEstimate>();

            foreach(var estimate in shipEstimates)
            {
                //  get supplier and supplier subtotal
                var supplierID = orderWorksheet.LineItems.First(li => li.ID == estimate.ShipEstimateItems.FirstOrDefault().LineItemID).SupplierID;
                var supplier = suppliers.Items.Where(supplier => supplier.ID == supplierID).FirstOrDefault();
                var supplierLineItems = orderWorksheet.LineItems.Where(li => li.SupplierID == supplier.ID);
                var supplierSubTotal = supplierLineItems.Select(li => li.LineSubtotal).Sum();
                if (supplier.xp?.FreeShippingThreshold != null && supplier.xp?.FreeShippingThreshold < supplierSubTotal) // free shipping for this supplier
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


        private List<LineItem> GetLineItemsToIncludeInShipping(IList<LineItem> lineItems, CheckoutIntegrationConfiguration configData)
        {
            return configData.ExcludePOProductsFromShipping ? 
                lineItems.Where(li => li.Product.xp.ProductType != ProductType.PurchaseOrder.ToString()).ToList() : 
                lineItems.ToList();
        }

        public async Task<OrderCalculateResponse> CalculateOrder(OrderCalculatePayload<MarketplaceOrderWorksheet> orderCalculatePayload)
        {
            if(orderCalculatePayload.OrderWorksheet.Order.xp != null && orderCalculatePayload.OrderWorksheet.Order.xp.OrderType == OrderType.Quote)
            {
                // quote orders do not have tax cost associated with them
                return new OrderCalculateResponse();
            } else
            {
                var totalTax = await _avalara.GetEstimateAsync(orderCalculatePayload.OrderWorksheet.Reserialize<OrderWorksheet>());

                return new OrderCalculateResponse
                {
                    TaxTotal = totalTax,
                };
            }
        }

        private List<string> GetProductsWithInvalidDimensions(IList<LineItem> lineItems)
        {
            return lineItems.Where(lineItem =>
            {
                return !(lineItem.Product.ShipHeight > 0 &&
                lineItem.Product.ShipLength > 0 &&
                lineItem.Product.ShipWeight > 0 &&
                lineItem.Product.ShipWidth > 0);
            }).Select(lineItem => lineItem.Product.ID).ToList();
        }
    }
}

