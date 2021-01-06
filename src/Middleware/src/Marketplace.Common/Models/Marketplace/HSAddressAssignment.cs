using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class HSAddressAssignment : AddressAssignment, IHSObject
    {
        public string ID { get; set; }
    }
}
