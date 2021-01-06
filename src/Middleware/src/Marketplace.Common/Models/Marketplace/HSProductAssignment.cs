using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class HSProductAssignment : ProductAssignment, IHSObject
    {
        public string ID { get; set; }
    }
}
