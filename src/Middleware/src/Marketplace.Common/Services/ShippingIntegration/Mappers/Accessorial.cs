using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using ordercloud.integrations.freightpop;
using OrderCloud.SDK;
using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class AccessorialMapper
    {
        public static List<Accessorial> Map(List<MarketplaceLineItem> lineItems, Address<BuyerAddressXP> shipToAddress, Address<SupplierAddressXP> shipFromAddress)
        {
            var accessorials = new List<Accessorial> { };
            accessorials= AddAccessorials(accessorials, lineItems);
            accessorials = AddAccessorials(accessorials, shipToAddress);
            accessorials = AddAccessorials(accessorials, shipFromAddress);
            return accessorials;
        }

        private static List<Accessorial> AddAccessorials(List<Accessorial> accessorials, List<MarketplaceLineItem> lineItems)
        {
            foreach (var lineItem in lineItems)
            {
                if (lineItem.Product.xp != null && lineItem.Product.xp.Accessorials != null && lineItem.Product.xp.Accessorials.Count() > 0)
                {
                    accessorials.AddRange(lineItem.Product.xp.Accessorials.Select(accessorial => (Accessorial)((int)accessorial)).ToList());
                }
            }
            return accessorials;
        }

        private static List<Accessorial> AddAccessorials(List<Accessorial> accessorials, Address<SupplierAddressXP> address)
        {
            if (address.xp != null && address.xp.Accessorials != null && address.xp.Accessorials.Count() > 0)
            {
                accessorials.AddRange(address.xp.Accessorials.Select(accessorial => (Accessorial)((int)accessorial)).ToList());
            }
            return accessorials;
        }

        private static List<Accessorial> AddAccessorials(List<Accessorial> accessorials, Address<BuyerAddressXP> address)
        {
            if (address.xp != null && address.xp.Accessorials != null && address.xp.Accessorials.Count() > 0)
            {
                accessorials.AddRange(address.xp.Accessorials.Select(accessorial => (Accessorial)((int)accessorial)).ToList());
            }
            return accessorials;
        }
    }
}