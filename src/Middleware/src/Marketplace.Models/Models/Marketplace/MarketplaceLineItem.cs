using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using Marketplace.Helpers.Attributes;

namespace Marketplace.Models.Models.Marketplace
{
    [SwaggerModel]
	public class MarketplaceLineItem : LineItem<LineItemXp, MarketplaceLineItemProduct, LineItemVariant, MarketplaceAddressBuyer, MarketplaceAddressSupplier> { }

    [SwaggerModel]
	public class LineItemXp { }
}
