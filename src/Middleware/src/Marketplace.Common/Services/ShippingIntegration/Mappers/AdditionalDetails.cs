using OrderCloud.SDK;
using System.Collections.Generic;
using System.Linq;
using ordercloud.integrations.freightpop;
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