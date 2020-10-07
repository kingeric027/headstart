using Marketplace.Models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Models.Marketplace
{
    public class MarketplaceProductUpdatePayload : WebhookPayloads.Products.Patch<dynamic, MarketplaceProduct> { }

    public class MarketplaceProductCreatePayload : WebhookPayloads.Products.Create<dynamic, MarketplaceProduct> { }
}
