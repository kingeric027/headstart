using ordercloud.integrations.library;

namespace Marketplace.Common.Models.Marketplace.Extended
{
    [SwaggerModel]
    public class SupplierProductType
    {
        public bool Standard { get; set; }
        public bool Quote { get; set; }
        public bool PurchaseOrder { get; set; }
    }
}
