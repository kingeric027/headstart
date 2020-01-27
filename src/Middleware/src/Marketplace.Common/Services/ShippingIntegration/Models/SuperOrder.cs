using System.Collections.Generic;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration
{
    public class SuperOrder
    {
        [Required]
        public Order Order { get; set; }
        [Required]
        public List<LineItem> LineItems { get; set; }
    }
}
