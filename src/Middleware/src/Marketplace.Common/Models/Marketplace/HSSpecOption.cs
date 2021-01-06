using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class HSSpecOption : SpecOption<SpecOptionXp>, IHSObject
    {
    }

    [SwaggerModel]
    public class SpecOptionXp
    {
        public string Description { get; set; }
        public string SpecID { get; set; }
    }
}
