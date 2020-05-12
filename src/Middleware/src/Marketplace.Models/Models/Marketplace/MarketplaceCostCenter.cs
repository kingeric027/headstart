using ordercloud.integrations.openapispec;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class MarketplaceCostCenter : CostCenter<CostCenterXp>, IMarketplaceObject
    {
        
    }

    [SwaggerModel]
    public class CostCenterXp
    {
    }
}
