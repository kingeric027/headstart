using Marketplace.Common.Models.Marketplace;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models;
using ordercloud.integrations.exchangerates;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Services
{
    public interface IOrderCalcService
    {
        decimal GetCreditCardTotal(MarketplaceOrderWorksheet worksheet);
        decimal GetPurchaseOrderTotal(MarketplaceOrderWorksheet worksheet);
    }

    public class OrderCalcService : IOrderCalcService
    {
        private readonly IOrderCloudClient _oc;
        public OrderCalcService(IOrderCloudClient oc)
        {
            _oc = oc;
        }

        public decimal GetCreditCardTotal(MarketplaceOrderWorksheet worksheet)
        {
            var purchaseOrderTotal = GetPurchaseOrderTotal(worksheet);
            return worksheet.Order.Total - purchaseOrderTotal;
        }

        public decimal GetPurchaseOrderTotal(MarketplaceOrderWorksheet worksheet)
        {
            return worksheet.LineItems
                .Where(li => li.Product.xp.ProductType == ProductType.PurchaseOrder)
                .Select(li => li.LineTotal)
                .Sum();
        }
    }
}
    