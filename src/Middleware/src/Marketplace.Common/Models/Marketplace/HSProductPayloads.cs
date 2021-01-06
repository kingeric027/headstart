using Headstart.Models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Headstart.Common.Models.Marketplace
{
    public class HSProductUpdatePayload : WebhookPayloads.Products.Patch<dynamic, HSProduct> { }

    public class HSProductCreatePayload : WebhookPayloads.Products.Create<dynamic, HSProduct> { }
}
