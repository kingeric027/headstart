using Marketplace.Common.Services.FreightPop;
using OrderCloud.SDK;
using System.Collections.Generic;
using System.Linq;
using Marketplace.Common.Services.FreightPop.Models;
using Marketplace.Models.Models.Marketplace;
using Marketplace.Models;
using Marketplace.Common.Services.ShippingIntegration.Mappers;

namespace Marketplace.Common.Services.ShippingIntegration
{
    public static class AdditionalDetailsMapper
    {
        public static AdditionalDetails Map(List<MarketplaceLineItem> lineItems, Address<BuyerAddressXP> shipToAddress, Address<SupplierAddressXP> shipFromAddress)
        {
            return new AdditionalDetails
            {
                Accessorials = AccessorialMapper.Map(lineItems, shipToAddress, shipFromAddress)
            };
        }
    }
}